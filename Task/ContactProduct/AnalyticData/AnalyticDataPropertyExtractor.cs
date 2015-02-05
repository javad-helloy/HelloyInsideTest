using System;
using System.Collections.Generic;
using System.Linq;
using Inside.Analytics;
using Inside.ContactProductService;
using Inside.Extenssions;
using Inside.GoogleService;
using InsideModel.Models;
using InsideModel.repositories;
using Newtonsoft.Json;

namespace Task.ContactProduct.AnalyticData
{
    public class AnalyticDataPropertyExtractor : IAnalyticDataPropertyExtractor
    {
        private readonly IRepository<Client> clientRepository;
        private readonly IRepository<Contact> contactRepository;
        private readonly IProductService contactProductService;
        private readonly IGoogleAnalyticsApi gaService;

        public AnalyticDataPropertyExtractor(
            IRepository<Client> clientRepository,
            IRepository<Contact> contactRepository,
            IProductService contactProductService,
            IGoogleAnalyticsApi gaService)
        {

            this.clientRepository = clientRepository;
            this.contactRepository = contactRepository;
            this.contactProductService = contactProductService;
            this.gaService = gaService;
        }

        public void UpdateContact(int clientId, DateTime fromDate, DateTime toDate) 
        {

            var clientForAnalytics = clientRepository.Where(l => l.Id == clientId).Single();

            var exceptions = new List<Exception>();

            if (!clientForAnalytics.IsActive)
            {
                throw new InvalidOperationException("Client Not Active");
            }

            if (String.IsNullOrEmpty(clientForAnalytics.AnalyticsTableId))
            {
                throw new InvalidOperationException("No Analytic Table Id for Client:"+clientForAnalytics.Id);
            }

            var query = new AnalyticQuery
            {
                TabelId = clientForAnalytics.AnalyticsTableId,
                StartDate = fromDate,
                EndDate = toDate,
            };

            query.Metrics.AddMany("ga:uniqueEvents");
            query.Dimenssions.AddMany("ga:eventAction","ga:eventLabel","ga:keyword","ga:source","ga:medium","ga:campaign","ga:eventCategory");
            query.MaxResults = 10000;

            var dataFeed = gaService.Get<ProductAnalyticData>(query);

            if (dataFeed != null)
            {
                foreach (var analyticForContact in dataFeed)
                {
                    try
                    {
                        SetContactPropertiesForAnalyticData(analyticForContact);
                    }
                    catch (Exception ex)
                    {
                        exceptions.Add(ex);

                    }
                }

            }

            if (exceptions.Count > 0)
            {
                throw new AggregateException(exceptions);
            }
        }


        public void SetContactPropertiesForAnalyticData(ProductAnalyticData productAnalyticForContact)
        {
            var contactAction = productAnalyticForContact.ContactAction;
            var contactLabel = productAnalyticForContact.ContactLabel;
            var contactCategory = productAnalyticForContact.ContactCategory;
            
            if (contactCategory.Equals("Calls"))
            {
                var apiAccessId = contactLabel.Split(',')[2];
                if (apiAccessId == null)
                {
                    throw new Exception("No CTM Api Access Id Found");
                }
                
                var matchingContactForUpdateCredentials =
                    contactRepository.Where(l => l.Property.Any(lp => lp.Type == "CTMId" && lp.Value == apiAccessId.Trim()));

                if (matchingContactForUpdateCredentials.Any())
                {
                    var contactToUpdate = matchingContactForUpdateCredentials.Single();
                    AddPropertiesToContact(contactToUpdate, productAnalyticForContact);
                }
            }
            else if (contactCategory == "HelloyContact")
            {
                if (contactAction == "Mail")
                {
                    dynamic apiAccessIdLabel = JsonConvert.DeserializeObject(contactLabel);
                    string apiAccessId = apiAccessIdLabel.mandrillId;

                    if (apiAccessId == null)
                    {
                        throw new Exception("No Mandrill Api Access Id Found");
                    }
                    var matchingContactForUpdateCredentials =
                        contactRepository.Where(l => l.Property.Any(lp => lp.Type == "MandrillId" && lp.Value == apiAccessId.Trim()));

                    if (matchingContactForUpdateCredentials.Any())
                    {
                        var contactToUpdate = matchingContactForUpdateCredentials.Single();
                        AddPropertiesToContact(contactToUpdate, productAnalyticForContact);
                    }
                }
                else if (contactAction == "Chat")
                {
                    dynamic apiAccessIdLabel = JsonConvert.DeserializeObject(contactLabel);
                    string apiAccessId = apiAccessIdLabel.chat_id;

                    if (apiAccessId == null)
                    {
                        throw new Exception("No LiveChat Api Access Id Found");
                    }
                    var matchingContactForUpdateCredentials =
                        contactRepository.Where(l => l.Property.Any(lp => lp.Type == "LiveChatId" && lp.Value == apiAccessId.Trim()));

                    if (matchingContactForUpdateCredentials.Any())
                    {
                        var contactToUpdate = matchingContactForUpdateCredentials.Single();
                        AddPropertiesToContact(contactToUpdate, productAnalyticForContact);
                    }
                }
                else if (contactAction == "Form")
                {
                    dynamic apiAccessIdLabel = JsonConvert.DeserializeObject(contactLabel);
                    string apiAccessId = apiAccessIdLabel.formPostId;

                    if (apiAccessId == null)
                    {
                        throw new Exception("No Form Access Id Found");
                    }
                    var matchingContactForUpdateCredentials =
                        contactRepository.Where(l => l.Property.Any(lp => lp.Type == "FormPostId" && lp.Value == apiAccessId.Trim()));

                    if (matchingContactForUpdateCredentials.Any())
                    {
                        var contactToUpdate = matchingContactForUpdateCredentials.Single();
                        AddPropertiesToContact(contactToUpdate, productAnalyticForContact);
                    }
                }
            }
        }

        public void AddPropertiesToContact(Contact contactToUpdate, ProductAnalyticData productAnalyticForContact)
        {
            var contactSearchPhrase = productAnalyticForContact.ContactSearchPhrase;
            var contactSource = productAnalyticForContact.ContactSource;
            var contactMedium = productAnalyticForContact.ContactMedium;
            var contactCampaign = productAnalyticForContact.ContactCampaign;

            if (IsPropertyValid(contactCampaign)) { contactToUpdate.Campaign= contactCampaign; }
            if (IsPropertyValid(contactSearchPhrase)) { contactToUpdate.SearchPhrase= contactSearchPhrase; }
            if (IsPropertyValid(contactSource)) { contactToUpdate.Source= contactSource; }
            if (IsPropertyValid(contactMedium)) { contactToUpdate.Medium= contactMedium; }

            if (contactProductService.IsValidProduct(contactCampaign, contactMedium))
            {
                var product = contactProductService.GetProduct(contactCampaign, contactMedium);
                contactToUpdate.Product= product;   
            }
            

            contactRepository.SaveChanges();
        }

        public bool IsPropertyValid(string property)
        {
            return !(string.IsNullOrEmpty(property) || property.ToLower().Contains("not set") || property.ToLower().Contains("not provided") ||
                    property.ToLower().Contains("none"));
        }
    }

    public class ProductAnalyticData
    {
        [AnalyticsMapping(Source = AnalyticsDataSource.Dimension, Name = "ga:eventAction")]
        public string ContactAction { get; set; }

        [AnalyticsMapping(Source = AnalyticsDataSource.Dimension, Name = "ga:eventLabel")]
        public string ContactLabel { get; set; }

        [AnalyticsMapping(Source = AnalyticsDataSource.Dimension, Name = "ga:eventCategory")]
        public string ContactCategory { get; set; }

        [AnalyticsMapping(Source = AnalyticsDataSource.Dimension, Name = "ga:keyword")]
        public string ContactSearchPhrase { get; set; }

        [AnalyticsMapping(Source = AnalyticsDataSource.Dimension, Name = "ga:source")]
        public string ContactSource { get; set; }

        [AnalyticsMapping(Source = AnalyticsDataSource.Dimension, Name = "ga:medium")]
        public string ContactMedium { get; set; }

        [AnalyticsMapping(Source = AnalyticsDataSource.Dimension, Name = "ga:campaign")]
        public string ContactCampaign { get; set; }
    }
}
