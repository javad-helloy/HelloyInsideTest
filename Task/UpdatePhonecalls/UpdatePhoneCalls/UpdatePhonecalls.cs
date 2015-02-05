using System;
using System.Collections.Generic;
using System.Linq;
using Inside.AutoRating;
using Inside.ExternalData;
using InsideModel.Models;
using InsideModel.repositories;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;

namespace Task.UpdatePhonecalls.UpdatePhoneCalls
{
    public class UpdatePhonecalls: IUpdatePhonecalls
    {
        private readonly IRepository<Client> clientRepository;
        private readonly IRepository<Contact> contactRepository;
        private readonly IExternalDataProvider externalDataProvider;
        private readonly IContactAutoRating contactAutoRating;


        public UpdatePhonecalls(
            IRepository<Client> clientRepository,
            IRepository<Contact> contactRepository,
            IExternalDataProvider externalDataProvider,
            IContactAutoRating contactAutoRating)
        {
            this.clientRepository = clientRepository;
            this.contactRepository = contactRepository;
            this.externalDataProvider = externalDataProvider;
            this.contactAutoRating = contactAutoRating;
        }

        public bool CanPerformTask(string taskType)
        {
            return taskType == TaskType.UpdatePhonecalls;
        }

        public void PerformTask(InsideModel.Models.Task taskMessage)
        {
            var updatePhoneCallsMessage =
                JsonConvert.DeserializeObject<UpdatePhonecallsForClientsTaskMessage>(taskMessage.Message);

            var callTrackingMetricsId =
                clientRepository.Single(c => c.Id == updatePhoneCallsMessage.ClientId).CallTrackingMetricId;
            if(callTrackingMetricsId == null) throw new Exception("No Call tracking metrics Id found for client:"+updatePhoneCallsMessage.ClientId);

            string pageUrl = null;
            CtmData phoneData;
            do
            {
                string phoneDataString;
                if (pageUrl == null)
                {
                     phoneDataString = externalDataProvider.GetPhoneData((int) callTrackingMetricsId,
                        updatePhoneCallsMessage.StartDate, updatePhoneCallsMessage.EndDate);
                }
                else
                {
                     phoneDataString = externalDataProvider.GetPhoneData(pageUrl);
                }

                if (phoneDataString.IsNullOrWhiteSpace()) return;

                phoneData = JsonConvert.DeserializeObject<CtmData>(phoneDataString);
                foreach (var call in phoneData.calls)
                {
                    var hasCallInDb = contactRepository.Where(l => l.Property.Any(lp => lp.Type == "CTMId" && lp.Value == call.id.ToString())).Any();
                   
                    if (hasCallInDb)
                    {
                        var contactToUpdate =
                            contactRepository.Single(
                                l => l.Property.Any(lp => lp.Type == "CTMId" && lp.Value == call.id.ToString()));


                        externalDataProvider.MapPhoneDataToContact(call, contactToUpdate);
                        contactAutoRating.SetAutoRating(contactToUpdate);
                    }
                    else
                    {
                        var hasMatchingClient = clientRepository.Where(c => c.CallTrackingMetricId == call.account_id).Any();
                        if (!hasMatchingClient)
                        {
                            throw new Exception("Recived ctm webhook data. Found no matching client for given account id: " + call.account_id);
                        }
                        var contact = externalDataProvider.MapPhoneDataToContact(call);
                        contactAutoRating.SetAutoRating(contact);
                        contactRepository.Add(contact);
                    }
                }
                contactRepository.SaveChanges();
                pageUrl = phoneData.next_page;
            } while (!phoneData.next_page.IsNullOrWhiteSpace());

        }
    }

    public class CtmData
    {
        public CtmData()
        {
            calls = new List<CallTrackingMetricsWebhookData>();
        }
        public IList<CallTrackingMetricsWebhookData> calls { get; set; }
        public string next_page { get; set; }
        public string previous_page { get; set; }
        public int total_pages { get; set; }
        public int page { get; set; }
    }
}
