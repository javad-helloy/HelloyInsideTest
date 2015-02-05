using System;
using System.Collections.Generic;
using System.Linq;
using Inside.Analytics;
using Inside.AutoRating;
using Inside.ContactProductService;
using Inside.Extenssions;
using Inside.GoogleService;
using Inside.Time;
using InsideModel.Models;
using InsideModel.repositories;
using Microsoft.Ajax.Utilities;

namespace Task.ImportCustomEvents
{
    public class CustomEventsExtractor : ICustomEventsExtractor
    {
        private readonly IRepository<Contact> contactRepository;
        private readonly IServerTime serverTime;
        private readonly IRepository<Client> clienRepository;
        private readonly IProductService contactProductService;
        private readonly IContactAutoRating contactAutoRating;
        private readonly IGoogleAnalyticsApi gaService;

        public CustomEventsExtractor(
            IRepository<Client> clienRepository,
            IRepository<Contact> contactRepository,
            IServerTime serverTime,
            IProductService contactProductService,
            IContactAutoRating contactAutoRating,
            IGoogleAnalyticsApi gaService)
        {
            this.clienRepository = clienRepository;
            this.contactRepository = contactRepository;
            this.serverTime = serverTime;
            this.contactProductService = contactProductService;
            this.contactAutoRating = contactAutoRating;
            this.gaService = gaService;
        }


        public void ImportEvents(int clientId, DateTime fromDate, DateTime toDate)
        {
            var exceptions = new List<Exception>();
            var clientInDb = clienRepository.Where(c => c.Id==clientId && c.IsActive );

            if (!clientInDb.Any())
            {
                return;
            }

            var client = clientInDb.Single();
            
            var analyticsId = client.AnalyticsTableId;

            if (string.IsNullOrEmpty(analyticsId))
            {
                throw new Exception("Client has no analytic table Id; client Id:" + clientId);
            }

            var query = new AnalyticQuery
            {
                TabelId = analyticsId,
                StartDate = fromDate,
                EndDate = toDate,
            };

            query.Metrics.AddMany("ga:uniqueEvents", "ga:eventValue");
            query.Dimenssions.AddMany("ga:eventAction", "ga:eventLabel", "ga:keyword", "ga:sourceMedium", "ga:dateHour", "ga:campaign", "ga:minute");
            query.MaxResults = 10000;
            query.Filters.AddMany("ga:eventCategory==Helloy Event");
            var dataFeed = gaService.Get<EventAnalyticData>(query);
            
            if (dataFeed != null && dataFeed.Any())
            {
                foreach (var Event in dataFeed)
                {
                    try
                    {
                        AddEvent(Event, clientId);
                    }
                    catch (Exception ex)
                    {
                        exceptions.Add(ex);
                    }
                }
                contactRepository.SaveChanges();
            }
            
            if (exceptions.Count > 0)
            {
                throw new AggregateException(exceptions);
            }
        }

        private void AddEvent(EventAnalyticData eventAnalyticData, int clientId)
        {
            var toServerTimeZoneFromStandardUser =
                serverTime.ParseToServerTimeZoneFromStandardUser(eventAnalyticData.EventDateString, "yyyyMMdd HH:mm:ss");
            var hasEventInDb =
                contactRepository.Where(
                    l =>
                        l.LeadType == "Event" && l.ClientId == clientId &&
                        l.Date == toServerTimeZoneFromStandardUser &&
                        l.Property.Any(lp => (lp.Type == "Action" && lp.Value == eventAnalyticData.EventAction)) &&
                        l.Property.Any(lp => (lp.Type == "Label" && lp.Value == eventAnalyticData.EventLabel))).Any();
            if (hasEventInDb)
            {
                return;
            }
            for (var i = 0; i < eventAnalyticData.NumberOfUniqueEvents; i++)
            {
                var eventContact = new Contact();
                eventContact.LeadType = "Event";
                eventContact.ClientId = clientId;
                eventContact.Date = toServerTimeZoneFromStandardUser;
                if (!eventAnalyticData.EventAction.IsNullOrWhiteSpace())
                {
                    eventContact.SetPropertyValue("Action", eventAnalyticData.EventAction);
                }
                if (!eventAnalyticData.EventLabel.IsNullOrWhiteSpace())
                {
                    eventContact.SetPropertyValue("Label", eventAnalyticData.EventLabel);
                }
                if (!eventAnalyticData.EventSearchPhrase.IsNullOrWhiteSpace() && 
                    !(
                        eventAnalyticData.EventSearchPhrase == "(not provided)" || 
                        eventAnalyticData.EventSearchPhrase == "(not set)"
                    ))
                {
                    eventContact.SearchPhrase=eventAnalyticData.EventSearchPhrase;
                }
                if (!eventAnalyticData.EventSource.IsNullOrWhiteSpace() && eventAnalyticData.EventSource != "(direct)")
                {
                    eventContact.Source= eventAnalyticData.EventSource;
                }
                if (!eventAnalyticData.EventMedium.IsNullOrWhiteSpace() && eventAnalyticData.EventMedium != "(none)")
                {
                    eventContact.Medium= eventAnalyticData.EventMedium;
                }
                if (!eventAnalyticData.EventCampaign.IsNullOrWhiteSpace() && eventAnalyticData.EventCampaign != "(not set)")
                {
                    eventContact.Campaign= eventAnalyticData.EventCampaign;
                }

                if (contactProductService.IsValidProduct(eventAnalyticData.EventCampaign,
                    eventAnalyticData.EventMedium))
                {
                    var product = contactProductService.GetProduct(eventAnalyticData.EventCampaign,eventAnalyticData.EventMedium);
                    eventContact.Product= product;
                    
                }
               
                contactAutoRating.SetAutoRating(eventContact);
                contactRepository.Add(eventContact);
            }
        }
    }

    public class EventAnalyticData
    {
        [AnalyticsMapping(Source = AnalyticsDataSource.Metric, Name = "ga:uniqueEvents")]
        public int NumberOfUniqueEvents { get; set; }

        [AnalyticsMapping(Source = AnalyticsDataSource.Dimension, Name = "ga:eventAction")]
        public string EventAction { get; set; }

        [AnalyticsMapping(Source = AnalyticsDataSource.Dimension, Name = "ga:eventLabel")]
        public string EventLabel { get; set; }

        [AnalyticsMapping(Source = AnalyticsDataSource.Dimension, Name = "ga:keyword")]
        public string EventSearchPhrase { get; set; }

        [AnalyticsMapping(Source = AnalyticsDataSource.Dimension, Name = "ga:sourceMedium")]
        public string EventSourceMedium { get; set; }

        public string EventSource
        {
            get { return EventSourceMedium.Split('/')[0].Trim(); }
        }

        public string EventMedium
        {
            get { return EventSourceMedium.Split('/')[1].Trim(); }
        }

        [AnalyticsMapping(Source = AnalyticsDataSource.Dimension, Name = "ga:campaign")]
        public string EventCampaign { get; set; }

        [AnalyticsMapping(Source = AnalyticsDataSource.Dimension, Name = "ga:dateHour")]
        public string EventDateHour { get; set; }

        [AnalyticsMapping(Source = AnalyticsDataSource.Dimension, Name = "ga:minute")]
        public string EventMinute { get; set; }

        public string EventDateString
        {
            get
            {
                return EventDateHour.Substring(0, 8) + " " + EventDateHour.Substring(8, 2) + ":" + EventMinute + ":00";
            }
        }

        [AnalyticsMapping(Source = AnalyticsDataSource.Metric, Name = "ga:eventValue")]
        public int RatingValue { get; set; }

    }
}

