using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.SessionState;
using Inside.AutoRating;
using Inside.ContactService;
using Inside.ExternalData;
using Inside.Time;
using InsideModel.Models;
using InsideModel.repositories;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;


namespace InsideReporting.Controllers.External
{
    [SessionState(SessionStateBehavior.ReadOnly)]
    public class WebhookController : Controller
    {
        private readonly IRepository<Contact> contactRepository;
        private readonly IRepository<Client> clientRepository;
        private readonly IContactService contactService;
        private readonly IServerTime serverTime;
        private readonly IExternalDataProvider externalDataProvider;
        private readonly IContactAutoRating contactAutoRating;


        public WebhookController(IRepository<Contact> contactRepository, 
            IRepository<Client> clientRepository,
            IContactService contactService,
            IServerTime serverTime,
            IExternalDataProvider externalDataProvider,
            IContactAutoRating contactAutoRating)
        {
            this.contactRepository = contactRepository;
            this.clientRepository = clientRepository;
            this.contactService = contactService;
            this.serverTime = serverTime;
            this.externalDataProvider = externalDataProvider;
            this.contactAutoRating = contactAutoRating;
        }

        [HttpPost]
        public ActionResult CallTrackingMetricWebhook()
        {
            Request.InputStream.Seek(0, SeekOrigin.Begin);
            string jsonData = new StreamReader(Request.InputStream).ReadToEnd();

            CallTrackingMetricsWebhookData data = null;

            try
            {
                data = JsonConvert.DeserializeObject<CallTrackingMetricsWebhookData>(jsonData);
            }
            catch (Exception e)
            {
                Trace.TraceError("Failed to parse jsondata from CallTrackingMetrics.", new object[] { e, jsonData });
                throw;
            }

            var contactId = data.id.ToString();

            var hasCallInDb =
                contactRepository.Where(
                    l =>
                        l.Property.Any(
                            lp => lp.Type == "CTMId" && lp.Value == contactId)).Any();
                
            Contact phoneCall;

            if (hasCallInDb)
            {
                var callTrackingMetricsId = data.id.ToString();
                phoneCall = contactRepository.Where(l => l.Property.Any(lp => lp.Type == "CTMId" && lp.Value == callTrackingMetricsId)).First();
                externalDataProvider.MapPhoneDataToContact(data, phoneCall);
                contactAutoRating.SetAutoRating(phoneCall);
            }
            else
            {
                var hasMatchingClient = clientRepository.Where(c => c.CallTrackingMetricId == data.account_id).Any();
                if (!hasMatchingClient)
                {
                    Trace.TraceError("Recived ctm webhook data. Found no matching client for given account id: " + data.account_id);
                    return Json("Ok", JsonRequestBehavior.AllowGet);
                }
                phoneCall = externalDataProvider.MapPhoneDataToContact(data);
                contactAutoRating.SetAutoRating(phoneCall);
                contactRepository.Add(phoneCall);
            }

            contactRepository.SaveChanges();

            var isPhoneCallNewerThanTenMinutes = phoneCall.Date > serverTime.RequestStarted.AddMinutes(-10);
            if (!hasCallInDb && isPhoneCallNewerThanTenMinutes)
            {
                contactService.NotifyClientsForNewContactWithEmail(phoneCall.Id);
                contactService.NotifyClientsForNewContactWithPhoneNotification(phoneCall.Id);
                contactService.NotifyClientsForNewContactWithSmsNotification(phoneCall.Id);
            }

            return Json("ok", JsonRequestBehavior.AllowGet);
        }
        //
        // POST: /Email/MandrillContentWebhook
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult MandrillContentWebhook(MadrillContentPost postData)
        {
            Trace.WriteLine("recived mandrill content content:" + postData.ToString());
            Contact message = null;
            var alreadyInDb = contactRepository.Where(l => l.Property.Any(lp => lp.Type == "MandrillId" && lp.Value == postData.id)).Any();

            if (alreadyInDb)
            {
                Trace.WriteLine("found mandrill Contact for id:" + postData.id);
                message =
                    contactRepository.Where(
                        l => l.Property.Any(lp => lp.Type == "MandrillId" && lp.Value == postData.id))
                        .First();
            }
            else
            {
                Trace.WriteLine("creating new Contact for id:" + postData.id);
                var hasMatchingClient = clientRepository.Where(c => c.EmailAddress.Contains(postData.to)).Any();
                if (!hasMatchingClient)
                {
                    Trace.TraceError("Recived ctm webhook data. Found no matching client for email: " + postData.to);
                    return Json("Ok", JsonRequestBehavior.AllowGet);
                }

                message = new Contact();
                message.LeadType = "Email";
                message.Date = serverTime.RequestStarted;
                message.ClientId = clientRepository.Where(c => c.EmailAddress.Contains(postData.to)).First().Id;
            }

            if (!postData.id.IsNullOrWhiteSpace())
            {
                message.SetPropertyValue("MandrillId", postData.id);
            }
            if (!postData.html.IsNullOrWhiteSpace())
            {
                message.SetPropertyValue("Html", postData.html);
            }
            if (!postData.text.IsNullOrWhiteSpace())
            {
                message.SetPropertyValue("Text", postData.text);
            }

            contactAutoRating.SetAutoRating(message);

            if (!alreadyInDb)
            {
                contactRepository.Add(message);
            }

            try
            {
                contactRepository.SaveChanges();
            }
            catch (Exception saveChangesException)
            {
                Trace.WriteLine("failed to save mandrill content for id:" + postData.id);
                try
                {
                    contactRepository.Reload(message);
                    message =
                        contactRepository.Where(
                            l => l.Property.Any(lp => lp.Type == "MandrillId" && lp.Value == postData.id)).First();

                    if (!postData.html.IsNullOrWhiteSpace())
                    {
                        message.SetPropertyValue("Html", postData.html);
                    }
                    if (!postData.text.IsNullOrWhiteSpace())
                    {
                        message.SetPropertyValue("Text", postData.text);
                    }

                    contactAutoRating.SetAutoRating(message);

                    Trace.Write("Updated exisiting entity with: " + postData.id);
                    contactRepository.SaveChanges();
                }
                catch (Exception updateExistingException)
                {
                    var sw = new StringBuilder();
                    sw.AppendLine("Failed to update existing email with id: " + postData.id);
                    sw.AppendLine("With data html: " + postData.html);
                    sw.AppendLine("With data text: " + postData.text);
                    sw.AppendLine("Exception: " + updateExistingException);
                    Trace.TraceError(sw.ToString());
                    throw;
                }
            }

            if (!alreadyInDb)
            {
                contactService.NotifyClientsForNewContactWithEmail(message.Id);
                contactService.NotifyClientsForNewContactWithPhoneNotification(message.Id);
                contactService.NotifyClientsForNewContactWithSmsNotification(message.Id);
            }

            return Json("ok", JsonRequestBehavior.AllowGet);
        }

       //
        // GET: /Email/ManrillWebhook
        [HttpPost]
        public ActionResult ManrillWebhook(string mandrill_events)
        {

            mandrill_events = mandrill_events.Replace("\"event\"", "\"eventName\"");

            var messages = JsonConvert.DeserializeObject<IList<MandrillEvent>>(mandrill_events);
            Contact message = null;

            foreach (var mandrillMessage in messages)
            {
                var id = mandrillMessage.msg._id;
                var messageAlreadyInDatabase =
                    contactRepository.Where(l => l.Property.Any(lp => lp.Type == "MandrillId" && lp.Value == id)).Any();

                if (messageAlreadyInDatabase)
                {
                    message =
                        contactRepository.Where(l => l.Property.Any(lp => lp.Type == "MandrillId" && lp.Value == id))
                            .First();
                }
                else
                {
                    string toEmail = mandrillMessage.msg.email;

                    var hasMatchingClient = clientRepository.Where(c => c.EmailAddress.Contains(toEmail)).Any();

                    if (!hasMatchingClient)
                    {
                        Trace.TraceError("Recived ctm webhook data. Found no matching client for email: " + toEmail);
                        return Json("Ok", JsonRequestBehavior.AllowGet);
                    }

                    message = new Contact();
                    message.LeadType = "Email";
                    message.ClientId = clientRepository.Where(c => c.EmailAddress.Contains(toEmail)).First().Id;
                }

                MapMandrillData(message, mandrillMessage);

                contactAutoRating.SetAutoRating(message);

                if (!messageAlreadyInDatabase)
                {
                    contactRepository.Add(message);
                }

                try
                {
                    contactRepository.SaveChanges();
                }
                catch (Exception e)
                {
                    contactRepository.Reload(message);
                    Trace.TraceInformation("Failed to import mandrill emails - retrying");

                    try
                    {
                        MapMandrillData(message, mandrillMessage);
                        contactAutoRating.SetAutoRating(message);
                        Trace.TraceInformation("Succeded to import mandrill emails");
                        contactRepository.SaveChanges();


                    }
                    catch (Exception retryException)
                    {
                        var rsw = new StringBuilder();
                        rsw.AppendLine("Failed to import mandrill emails");
                        rsw.AppendLine("With data: " + mandrill_events);
                        rsw.AppendLine("Exception: " + retryException);
                        Trace.TraceError(rsw.ToString());
                        throw;

                    }
                }

                if (!messageAlreadyInDatabase)
                {
                    contactService.NotifyClientsForNewContactWithEmail(message.Id);
                    contactService.NotifyClientsForNewContactWithPhoneNotification(message.Id);
                    contactService.NotifyClientsForNewContactWithSmsNotification(message.Id);
                }
            }

            return Json("ok", JsonRequestBehavior.AllowGet);
        }

        private static void MapMandrillData(Contact message, MandrillEvent mandrillMessage)
        {
            if (!mandrillMessage.msg.sender.IsNullOrWhiteSpace())
            {
                message.SetPropertyValue("FromEmail", mandrillMessage.msg.sender);
            }
            if (!mandrillMessage.msg.email.IsNullOrWhiteSpace())
            {
                message.SetPropertyValue("ToEmail", mandrillMessage.msg.email);
            }
            if (!mandrillMessage.msg._id.IsNullOrWhiteSpace())
            {
                message.SetPropertyValue("MandrillId", mandrillMessage.msg._id);
            }
            message.Date =
                (new DateTime(1970, 1, 1, 0, 0, 0, 0)).AddSeconds(int.Parse(mandrillMessage.ts)).ToLocalTime();

            if (!mandrillMessage.msg.state.IsNullOrWhiteSpace())
            {
                message.SetPropertyValue("State", mandrillMessage.msg.state);
            }
            if (!mandrillMessage.msg.subject.IsNullOrWhiteSpace())
            {
                message.SetPropertyValue("Subject", mandrillMessage.msg.subject);
            }
            if (mandrillMessage.msg.tags.Count > 0)
            {
                message.SetPropertyValue("Tags", string.Join(", ", mandrillMessage.msg.tags));
            }
            message.SetPropertyValue("Updated", DateTime.Now.ToString());

        }



    }

    public class MadrillContentPost
    {
        public override string ToString()
        {
            return "id: " + id + " html: " + html + " text: " + text + " to: " + to;
        }

        public string id { get; set; } 
        public string html { get; set; } 
        public string text { get; set; }
        public string to { get; set; } 
    }

    internal class EmailImportResult
    {
        public int imported_email { get; set; }
    }

    internal class MandrillEvent
    {
        public string eventName { get; set; }
        public MandrillMessage msg { get; set; }
        public string ts { get; set; }
    }

    internal class MandrillMessage
    {
        public string ts { get; set; }
        public string subject { get; set; }
        public string email { get; set; }
        public string sender { get; set; }
        public IList<string> tags { get; set; }
        public string state { get; set; }
        public string _id { get; set; }
    }
}