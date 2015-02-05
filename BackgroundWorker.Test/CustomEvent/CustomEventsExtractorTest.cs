using System;
using System.Collections.Generic;
using System.Linq;
using Google.Apis.Analytics.v3;
using Google.GData.Analytics;
using Helpers.test;
using Inside.Analytics;
using Inside.AutoRating;
using Inside.ContactProductService;
using Inside.Extenssions;
using Inside.GoogleService;
using Inside.Time;
using InsideModel.Models;
using InsideModel.repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Task.ContactProduct.AnalyticData;
using Task.ImportCustomEvents;


namespace BackgroundWorker.Test.CustomEvent
{
    [TestClass]
    public class CustomEventsExtractorTest
    {
        [TestMethod]
        public void CanCreate()
        {
            var clientRepositoryMoq = new Mock<IRepository<Client>>();
            var contactRepositoryMoq = new Mock<IRepository<Contact>>();
            var contactProductService = new Mock<IProductService>();
            var contactAutoRating = new Mock<IContactAutoRating>();
            var gaService = new Mock<IGoogleAnalyticsApi>();

            var customEventsExtractor = new CustomEventsExtractor(
                clientRepositoryMoq.Object, 
                contactRepositoryMoq.Object,
                new ServerTime(),
                contactProductService.Object,
                 contactAutoRating.Object,
                 gaService.Object);
        }

        [TestMethod]
        public void NoEntriesNoInsertionInDataBase()
        {
            var clientRepositoryMoq = new LocalRepository<Client>();
            var contactRepositoryMoq = new Mock<IRepository<Contact>>();
            var contactProductService = new Mock<IProductService>();
            var contactAutoRating = new Mock<IContactAutoRating>();
            var gaService = new Mock<IGoogleAnalyticsApi>();

            var client = new Client(){Id = 1, IsActive = true, AnalyticsTableId = "ga:123456"};
            clientRepositoryMoq.Add(client);

            var startDate = new DateTime(2014, 01, 01);
            var endDate = new DateTime(2014, 01, 03);

            var analyticQuery = new AnalyticQuery();
            analyticQuery.Dimenssions.AddMany("ga:eventAction", "ga:eventLabel", "ga:keyword", "ga:sourceMedium", "ga:dateHour", "ga:minute", "ga:campaign");
            analyticQuery.TabelId = clientRepositoryMoq.All().First().AnalyticsTableId;
            analyticQuery.StartDate = startDate;
            analyticQuery.EndDate = endDate;
            analyticQuery.Metrics.AddMany("ga:uniqueEvents", "ga:eventValue");
            analyticQuery.Filters.Add("ga:eventCategory==Helloy Event");

            gaService.Setup(
               ga =>
                   ga.Get<EventAnalyticData>(It.Is<AnalyticQuery>(
                            aq =>
                                !aq.Dimenssions.Except(analyticQuery.Dimenssions).Any() &&
                                !aq.Metrics.Except(analyticQuery.Metrics).Any() &&
                                !aq.Filters.Except(analyticQuery.Filters).Any() &&
                                aq.StartDate == analyticQuery.StartDate &&
                                aq.EndDate == analyticQuery.EndDate &&
                                aq.TabelId == analyticQuery.TabelId))).Returns((IEnumerable<EventAnalyticData>)null);

            var customEventsExtractor = new CustomEventsExtractor(
                clientRepositoryMoq, 
                contactRepositoryMoq.Object,
                new ServerTime(),
                contactProductService.Object,
                 contactAutoRating.Object,
                 gaService.Object);

            
            customEventsExtractor.ImportEvents(client.Id, startDate,endDate);
            contactRepositoryMoq.Verify(l => l.SaveChanges(), Times.Never);
            contactRepositoryMoq.Verify(l => l.Add(It.IsAny<Contact>()), Times.Never);

        }

        [TestMethod]
        public void CreateLeadForMoreThanOneUniqueEventIgnoreNullProperties()
        {
            var clientRepositoryMoq = new LocalRepository<Client>();
            var contactRepository = new LocalRepository<Contact>();
            var contactProductService = new Mock<IProductService>();
            var contactAutoRating = new Mock<IContactAutoRating>();
            var gaService = new Mock<IGoogleAnalyticsApi>();

            var dataEntries = new List<EventAnalyticData>();
            var dataEntry1 = new EventAnalyticData();

            dataEntry1.NumberOfUniqueEvents = 2;
            dataEntry1.RatingValue = 1;
            dataEntry1.EventAction = "Test Action";
            dataEntry1.EventLabel = "Test Lable";
            dataEntry1.EventSearchPhrase = "";
            dataEntry1.EventSourceMedium = "Source/Medium";
            dataEntry1.EventCampaign = "Campaign";
            dataEntry1.EventDateHour = "2014010100";
            dataEntry1.EventMinute = "00";
            dataEntries.Add(dataEntry1);

            var startDate = new DateTime(2014, 01, 01);
            var endDate = new DateTime(2014, 01, 03);
            var client = new Client() { Id = 1, IsActive = true, AnalyticsTableId = "ga:123456" };
            clientRepositoryMoq.Add(client);

            var analyticQuery = new AnalyticQuery();
            analyticQuery.Dimenssions.AddMany("ga:eventAction", "ga:eventLabel", "ga:keyword", "ga:sourceMedium", "ga:dateHour", "ga:minute", "ga:campaign");
            analyticQuery.TabelId = clientRepositoryMoq.All().First().AnalyticsTableId;
            analyticQuery.StartDate = startDate;
            analyticQuery.EndDate = endDate;
            analyticQuery.Metrics.AddMany("ga:uniqueEvents", "ga:eventValue");
            analyticQuery.Filters.Add("ga:eventCategory==Helloy Event");

            gaService.Setup(
               ga =>
                   ga.Get<EventAnalyticData>(It.Is<AnalyticQuery>(
                            aq =>
                                !aq.Dimenssions.Except(analyticQuery.Dimenssions).Any() &&
                                !aq.Metrics.Except(analyticQuery.Metrics).Any() &&
                                !aq.Filters.Except(analyticQuery.Filters).Any() &&
                                aq.StartDate == analyticQuery.StartDate &&
                                aq.EndDate == analyticQuery.EndDate &&
                                aq.TabelId == analyticQuery.TabelId))).Returns(dataEntries);
            
            var customEventsExtractor = new CustomEventsExtractor(
                clientRepositoryMoq, 
                contactRepository,
                new ServerTime(),
                contactProductService.Object,
                 contactAutoRating.Object,
                 gaService.Object);

            contactProductService.Setup(cp => cp.IsValidProduct("Campaign", "Medium")).Returns(true);
            contactProductService.Setup(cp => cp.GetProduct("Campaign", "Medium")).Returns("Product");

            
            customEventsExtractor.ImportEvents(client.Id, startDate, endDate);

            Assert.AreEqual(2, contactRepository.All().Count());
            
            Assert.AreEqual("Campaign", contactRepository.All().First().Campaign);
            Assert.AreEqual("Medium", contactRepository.All().First().Medium);
            Assert.AreEqual("Source", contactRepository.All().First().Source);
            Assert.AreEqual("Test Action", contactRepository.All().First().Property.Single(lp => lp.Type == "Action").Value);
            Assert.AreEqual("Test Lable", contactRepository.All().First().Property.Single(lp => lp.Type == "Label").Value);
            Assert.AreEqual("Product", contactRepository.All().First().Product);

            Assert.IsFalse(contactRepository.All().First().Property.Any(lp => lp.Type == "SearchPhrase"));

            
            Assert.AreEqual("Campaign", contactRepository.All().Last().Campaign);
            Assert.AreEqual("Medium", contactRepository.All().Last().Medium);
            Assert.AreEqual("Source", contactRepository.All().Last().Source);
            Assert.AreEqual("Test Action", contactRepository.All().Last().Property.Single(lp => lp.Type == "Action").Value);
            Assert.AreEqual("Test Lable", contactRepository.All().Last().Property.Single(lp => lp.Type == "Label").Value);
            Assert.AreEqual("Product", contactRepository.All().Last().Product);

            Assert.IsFalse(contactRepository.All().Last().Property.Any(lp => lp.Type == "SearchPhrase"));
        }

        [TestMethod]
        public void CreateLeadForMoreThanOneEventWithSameDateDifferentTime()
        {
            var clientRepositoryMoq = new LocalRepository<Client>();
            var contactRepositoryMoq = new Mock<IRepository<Contact>>();
            var contactProductService = new Mock<IProductService>();
            var contactAutoRating = new Mock<IContactAutoRating>();

            var gaService = new Mock<IGoogleAnalyticsApi>();

            var dataEntries = new List<EventAnalyticData>();
            var dataEntry1 = new EventAnalyticData();

            dataEntry1.NumberOfUniqueEvents = 1;
            dataEntry1.RatingValue = 1;
            dataEntry1.EventAction = "Test Action";
            dataEntry1.EventLabel = "Test Lable";
            dataEntry1.EventSearchPhrase = "Some Search Phrase";
            dataEntry1.EventSourceMedium = "Source/Medium";
            dataEntry1.EventCampaign = "Campaign";
            dataEntry1.EventDateHour = "2014010100";
            dataEntry1.EventMinute = "00";
            dataEntries.Add(dataEntry1);

            var dataEntry2 = new EventAnalyticData();
            dataEntry2.NumberOfUniqueEvents = 1;
            dataEntry2.RatingValue = 1;
            dataEntry2.EventAction = "Test Action";
            dataEntry2.EventLabel = "Test Lable";
            dataEntry2.EventSearchPhrase = "Some Search Phrase";
            dataEntry2.EventSourceMedium = "Source/Medium";
            dataEntry2.EventCampaign = "Campaign";
            dataEntry2.EventDateHour = "2014010100";
            dataEntry2.EventMinute = "01"; //======> different Minute
            dataEntries.Add(dataEntry2);

            var startDate = new DateTime(2014, 01, 01);
            var endDate = new DateTime(2014, 01, 03);
            var client = new Client() { Id = 1, IsActive = true, AnalyticsTableId = "ga:123456" };
            clientRepositoryMoq.Add(client);

            var analyticQuery = new AnalyticQuery();
            analyticQuery.Dimenssions.AddMany("ga:eventAction", "ga:eventLabel", "ga:keyword", "ga:sourceMedium", "ga:dateHour", "ga:minute", "ga:campaign");
            analyticQuery.TabelId = clientRepositoryMoq.All().First().AnalyticsTableId;
            analyticQuery.StartDate = startDate;
            analyticQuery.EndDate = endDate;
            analyticQuery.Metrics.AddMany("ga:uniqueEvents", "ga:eventValue");
            analyticQuery.Filters.Add("ga:eventCategory==Helloy Event");

            gaService.Setup(
               ga =>
                   ga.Get<EventAnalyticData>(It.Is<AnalyticQuery>(
                            aq =>
                                !aq.Dimenssions.Except(analyticQuery.Dimenssions).Any() &&
                                !aq.Metrics.Except(analyticQuery.Metrics).Any() &&
                                !aq.Filters.Except(analyticQuery.Filters).Any() &&
                                aq.StartDate == analyticQuery.StartDate &&
                                aq.EndDate == analyticQuery.EndDate &&
                                aq.TabelId == analyticQuery.TabelId))).Returns(dataEntries);
            
            var customEventsExtractor = new CustomEventsExtractor(
                clientRepositoryMoq, 
                contactRepositoryMoq.Object,
                new ServerTime(),
                contactProductService.Object,
                 contactAutoRating.Object,
                 gaService.Object);

            customEventsExtractor.ImportEvents(client.Id, startDate, endDate);
            contactRepositoryMoq.Verify(l => l.SaveChanges(), Times.Exactly(1));
            contactRepositoryMoq.Verify(l => l.Add(It.IsAny<Contact>()), Times.Exactly(2));

        }

        [TestMethod]
        public void DontCreateLeadForEventsWithSameDateAndTimeWithLeadInDatabase()
        {
            var clientRepositoryMoq = new LocalRepository<Client>();
            var contactRepositoryMoq = new LocalRepository<Contact>();
            var contactProductService = new Mock<IProductService>();
            var contactAutoRating = new Mock<IContactAutoRating>();
            var serverTime = new Mock<IServerTime>();
            var actionProperty = new ContactProperty()
            {
                ContactId = 1,
                Type = "Action",
                Value = "Test Action"
            };
            var lableProperty = new ContactProperty()
            {
                ContactId = 1,
                Type = "Label",
                Value = "Test Lable"
            };

            var leadEntry1 = new Contact()
            {
                Id = 1,
                ClientId = 1,
                LeadType = "Event",
                Date = new DateTime(2014, 01, 01, 00, 00, 00)
            };
            
            leadEntry1.Property.Add(actionProperty);
            leadEntry1.Property.Add(lableProperty);
            contactRepositoryMoq.Add(leadEntry1);

            var gaService = new Mock<IGoogleAnalyticsApi>();

            var dataEntries = new List<EventAnalyticData>();
            var dataEntry1 = new EventAnalyticData();

            dataEntry1.NumberOfUniqueEvents = 1;
            dataEntry1.RatingValue = 1;
            dataEntry1.EventAction = "Test Action";
            dataEntry1.EventLabel = "Test Lable";
            dataEntry1.EventSearchPhrase = "Some Search Phrase";
            dataEntry1.EventSourceMedium = "Source/Medium";
            dataEntry1.EventCampaign = "Campaign";
            dataEntry1.EventDateHour = "2014010101"; //==>anothe time zone
            dataEntry1.EventMinute = "00";
            dataEntries.Add(dataEntry1);

            var startDate = new DateTime(2014, 01, 01);
            var endDate = new DateTime(2014, 01, 03);
            var client = new Client() { Id = 1, IsActive = true, AnalyticsTableId = "ga:123456" };
            clientRepositoryMoq.Add(client);

            serverTime.Setup(st => st.ParseToServerTimeZoneFromStandardUser("20140101 01:00:00", "yyyyMMdd HH:mm:ss"))
                .Returns(leadEntry1.Date);

            var analyticQuery = new AnalyticQuery();
            analyticQuery.Dimenssions.AddMany("ga:eventAction", "ga:eventLabel", "ga:keyword", "ga:sourceMedium", "ga:dateHour", "ga:minute", "ga:campaign");
            analyticQuery.TabelId = clientRepositoryMoq.All().First().AnalyticsTableId;
            analyticQuery.StartDate = startDate;
            analyticQuery.EndDate = endDate;
            analyticQuery.Metrics.AddMany("ga:uniqueEvents", "ga:eventValue");
            analyticQuery.Filters.Add("ga:eventCategory==Helloy Event");

            gaService.Setup(
               ga =>
                   ga.Get<EventAnalyticData>(It.Is<AnalyticQuery>(
                            aq =>
                                !aq.Dimenssions.Except(analyticQuery.Dimenssions).Any() &&
                                !aq.Metrics.Except(analyticQuery.Metrics).Any() &&
                                !aq.Filters.Except(analyticQuery.Filters).Any() &&
                                aq.StartDate == analyticQuery.StartDate &&
                                aq.EndDate == analyticQuery.EndDate &&
                                aq.TabelId == analyticQuery.TabelId))).Returns(dataEntries);
            
            var customEventsExtractor = new CustomEventsExtractor(
                clientRepositoryMoq, 
                contactRepositoryMoq,
                serverTime.Object,
                contactProductService.Object,
                 contactAutoRating.Object,
                 gaService.Object);

            
            customEventsExtractor.ImportEvents(client.Id, startDate, endDate);

            Assert.AreEqual(contactRepositoryMoq.All().Count(),1);
        }

        [TestMethod]
        public void CreateLeadForEventValueCreateLeadInterAction()
        {
            var clientRepositoryMoq = new LocalRepository<Client>();
            var contactRepository = new LocalRepository<Contact>();
            var contactProductService = new Mock<IProductService>();
            var contactAutoRating = new Mock<IContactAutoRating>();

            var gaService = new Mock<IGoogleAnalyticsApi>();

            var dataEntries = new List<EventAnalyticData>();
            var dataEntry1 = new EventAnalyticData();

            dataEntry1.NumberOfUniqueEvents = 1;
            dataEntry1.RatingValue = 0;
            dataEntry1.EventAction = "Test Action";
            dataEntry1.EventLabel = "Test Lable";
            dataEntry1.EventSearchPhrase = "Some Search Phrase";
            dataEntry1.EventSourceMedium = "Source/Medium";
            dataEntry1.EventCampaign = "Campaign";
            dataEntry1.EventDateHour = "2014010100";
            dataEntry1.EventMinute = "00";
            dataEntries.Add(dataEntry1);

            var dataEntry2 = new EventAnalyticData();

            dataEntry2.NumberOfUniqueEvents = 1;
            dataEntry2.RatingValue = 1;
            dataEntry2.EventAction = "Test Action2";
            dataEntry2.EventLabel = "Test Lable2";
            dataEntry2.EventSearchPhrase = "Some Search Phrase2";
            dataEntry2.EventSourceMedium = "Source2/Medium";
            dataEntry2.EventCampaign = "Campaign";
            dataEntry2.EventDateHour = "2014010100";
            dataEntry2.EventMinute = "01";
            dataEntries.Add(dataEntry2);

            var startDate = new DateTime(2014, 01, 01);
            var endDate = new DateTime(2014, 01, 03);

            var client = new Client() { Id = 1, IsActive = true, AnalyticsTableId = "ga:123456" };
            clientRepositoryMoq.Add(client);

            var analyticQuery = new AnalyticQuery();
            analyticQuery.Dimenssions.AddMany("ga:eventAction", "ga:eventLabel", "ga:keyword", "ga:sourceMedium", "ga:dateHour", "ga:minute", "ga:campaign");
            analyticQuery.TabelId = clientRepositoryMoq.All().First().AnalyticsTableId;
            analyticQuery.StartDate = startDate;
            analyticQuery.EndDate = endDate;
            analyticQuery.Metrics.AddMany("ga:uniqueEvents", "ga:eventValue");
            analyticQuery.Filters.Add("ga:eventCategory==Helloy Event");

            gaService.Setup(
               ga =>
                   ga.Get<EventAnalyticData>(It.Is<AnalyticQuery>(
                            aq =>
                                !aq.Dimenssions.Except(analyticQuery.Dimenssions).Any() &&
                                !aq.Metrics.Except(analyticQuery.Metrics).Any() &&
                                !aq.Filters.Except(analyticQuery.Filters).Any() &&
                                aq.StartDate == analyticQuery.StartDate &&
                                aq.EndDate == analyticQuery.EndDate &&
                                aq.TabelId == analyticQuery.TabelId))).Returns(dataEntries);

            contactProductService.Setup(cp => cp.IsValidProduct("Campaign", "Medium")).Returns(true);
            contactProductService.Setup(cp => cp.GetProduct("Campaign", "Medium")).Returns("Product");

            var customEventsExtractor = new CustomEventsExtractor(
                clientRepositoryMoq, 
               contactRepository,
                new ServerTime(),
                contactProductService.Object,
                 contactAutoRating.Object,
                 gaService.Object);

            customEventsExtractor.ImportEvents(client.Id, startDate, endDate);
            
            
            Assert.AreEqual(2, contactRepository.All().Count());
            
            Assert.AreEqual("Campaign", contactRepository.All().First().Campaign);
            Assert.AreEqual("Medium", contactRepository.All().First().Medium);
            Assert.AreEqual("Source", contactRepository.All().First().Source);
            Assert.AreEqual("Test Action", contactRepository.All().First().Property.Single(lp => lp.Type == "Action").Value);
            Assert.AreEqual("Test Lable", contactRepository.All().First().Property.Single(lp => lp.Type == "Label").Value);
            Assert.AreEqual("Some Search Phrase", contactRepository.All().First().SearchPhrase);
            Assert.AreEqual("Product", contactRepository.All().First().Product);

            
            Assert.AreEqual("Campaign", contactRepository.All().Last().Campaign);
            Assert.AreEqual("Medium", contactRepository.All().Last().Medium);
            Assert.AreEqual("Source2", contactRepository.All().Last().Source);
            Assert.AreEqual("Test Action2", contactRepository.All().Last().Property.Single(lp => lp.Type == "Action").Value);
            Assert.AreEqual("Test Lable2", contactRepository.All().Last().Property.Single(lp => lp.Type == "Label").Value);
            Assert.AreEqual("Some Search Phrase2", contactRepository.All().Last().SearchPhrase);
            Assert.AreEqual("Product", contactRepository.All().Last().Product);
        }

        [TestMethod]
        public void CreateLeadForEventValueDayLightSaving()
        {
            var clientRepositoryMoq = new LocalRepository<Client>();
            var contactRepository = new LocalRepository<Contact>();
            var contactProductService = new Mock<IProductService>();
            var contactAutoRating = new Mock<IContactAutoRating>();

            var gaService = new Mock<IGoogleAnalyticsApi>();

            var dataEntries = new List<EventAnalyticData>();
            var dataEntry1 = new EventAnalyticData();

            dataEntry1.NumberOfUniqueEvents = 1;
            dataEntry1.RatingValue = 0;
            dataEntry1.EventAction = "Test Action";
            dataEntry1.EventLabel = "Test Lable";
            dataEntry1.EventSearchPhrase = "Some Search Phrase";
            dataEntry1.EventSourceMedium = "Source/Medium";
            dataEntry1.EventCampaign = "Campaign";
            dataEntry1.EventDateHour = "2014052703";
            dataEntry1.EventMinute = "00";
            dataEntries.Add(dataEntry1);

            var startDate = new DateTime(2014, 05, 26);
            var endDate = new DateTime(2014, 05, 28);

            Client client = new Client() { Id = 1, IsActive = true, AnalyticsTableId = "ga:123456" };
            clientRepositoryMoq.Add(client);

            var analyticQuery = new AnalyticQuery();
            analyticQuery.Dimenssions.AddMany("ga:eventAction", "ga:eventLabel", "ga:keyword", "ga:sourceMedium", "ga:dateHour", "ga:minute", "ga:campaign");
            analyticQuery.TabelId = clientRepositoryMoq.All().First().AnalyticsTableId;
            analyticQuery.StartDate = startDate;
            analyticQuery.EndDate = endDate;
            analyticQuery.Metrics.AddMany("ga:uniqueEvents", "ga:eventValue");
            analyticQuery.Filters.Add("ga:eventCategory==Helloy Event");

            gaService.Setup(
               ga =>
                   ga.Get<EventAnalyticData>(It.Is<AnalyticQuery>(
                            aq =>
                                !aq.Dimenssions.Except(analyticQuery.Dimenssions).Any() &&
                                !aq.Metrics.Except(analyticQuery.Metrics).Any() &&
                                !aq.Filters.Except(analyticQuery.Filters).Any() &&
                                aq.StartDate == analyticQuery.StartDate &&
                                aq.EndDate == analyticQuery.EndDate &&
                                aq.TabelId == analyticQuery.TabelId))).Returns(dataEntries);

            contactProductService.Setup(cp => cp.GetProduct("Campaign", "Medium")).Returns("Product");

            var customEventsExtractor = new CustomEventsExtractor(
                clientRepositoryMoq,
                contactRepository,
                new ServerTime(),
                contactProductService.Object,
                 contactAutoRating.Object,
                 gaService.Object);

            
            customEventsExtractor.ImportEvents(client.Id,startDate, endDate);

            Assert.AreEqual(1, contactRepository.All().Count());

            var createdContact = contactRepository.All().First();

            Assert.AreEqual(new DateTime(2014,05,27,1,0,0), createdContact.Date);
            Assert.AreEqual("Event", createdContact.LeadType);
            Assert.AreEqual(1, createdContact.ClientId);
        }

        [TestMethod]
        public void DefaultAnalyticsValuesAreNotImportad()
        {
            var clientRepositoryMoq = new LocalRepository<Client>();
            var contactRepository = new LocalRepository<Contact>();
            var contactProductService = new Mock<IProductService>();
            var contactAutoRating = new Mock<IContactAutoRating>();

            var gaService = new Mock<IGoogleAnalyticsApi>();

            var dataEntries = new List<EventAnalyticData>();
            var dataEntry1 = new EventAnalyticData();

            dataEntry1.NumberOfUniqueEvents = 1;
            dataEntry1.RatingValue = 0;
            dataEntry1.EventAction = "Test Action";
            dataEntry1.EventLabel = "Test Lable";
            dataEntry1.EventSearchPhrase = "Some Search Phrase";
            dataEntry1.EventSourceMedium = "SourceValue / MediumValue";
            dataEntry1.EventCampaign = "Campaign 1";
            dataEntry1.EventDateHour = "2014010100";
            dataEntry1.EventMinute = "00";
            dataEntries.Add(dataEntry1);

            var dataEntry2 = new EventAnalyticData();

            dataEntry2.NumberOfUniqueEvents = 1;
            dataEntry2.RatingValue = 1;
            dataEntry2.EventAction = "Test Action2";
            dataEntry2.EventLabel = "Test Lable2";
            dataEntry2.EventSearchPhrase = "(not provided)";
            dataEntry2.EventSourceMedium = "(direct) / (none)";
            dataEntry2.EventCampaign = "(not set)";
            dataEntry2.EventDateHour = "2014010100";
            dataEntry2.EventMinute = "01";
            dataEntries.Add(dataEntry2);

            var startDate = new DateTime(2014, 01, 01);
            var endDate = new DateTime(2014, 01, 03);
            var client = new Client() { Id = 1, IsActive = true, AnalyticsTableId = "ga:123456" };
            clientRepositoryMoq.Add(client);

            var analyticQuery = new AnalyticQuery();
            analyticQuery.Dimenssions.AddMany("ga:eventAction", "ga:eventLabel", "ga:keyword", "ga:sourceMedium", "ga:dateHour", "ga:minute", "ga:campaign");
            analyticQuery.TabelId = clientRepositoryMoq.All().First().AnalyticsTableId;
            analyticQuery.StartDate = startDate;
            analyticQuery.EndDate = endDate;
            analyticQuery.Metrics.AddMany("ga:uniqueEvents", "ga:eventValue");
            analyticQuery.Filters.Add("ga:eventCategory==Helloy Event");

            gaService.Setup(
               ga =>
                   ga.Get<EventAnalyticData>(It.Is<AnalyticQuery>(
                            aq =>
                                !aq.Dimenssions.Except(analyticQuery.Dimenssions).Any() &&
                                !aq.Metrics.Except(analyticQuery.Metrics).Any() &&
                                !aq.Filters.Except(analyticQuery.Filters).Any() &&
                                aq.StartDate == analyticQuery.StartDate &&
                                aq.EndDate == analyticQuery.EndDate &&
                                aq.TabelId == analyticQuery.TabelId))).Returns(dataEntries);

            var customEventsExtractor = new CustomEventsExtractor(
                clientRepositoryMoq,
                contactRepository,
                new ServerTime(),
                contactProductService.Object,
                 contactAutoRating.Object,
                 gaService.Object);



            contactProductService.Setup(ps => ps.IsValidProduct("Campaign 1", "MediumValue")).Returns(true);
            contactProductService.Setup(ps => ps.GetProduct("Campaign 1", "MediumValue")).Returns("Product1");

            customEventsExtractor.ImportEvents(client.Id, startDate, endDate);

            var contact1 = contactRepository.All().ElementAt(0);
            var contact2 = contactRepository.All().ElementAt(1);


            Assert.AreEqual("Some Search Phrase", contact1.SearchPhrase);
            Assert.AreEqual("SourceValue", contact1.Source);
            Assert.AreEqual("MediumValue", contact1.Medium);
            Assert.AreEqual("Campaign 1", contact1.Campaign);
            Assert.AreEqual("Product1", contact1.Product);

            Assert.IsNull(contact2.SearchPhrase);
            Assert.IsNull(contact2.Source);
            Assert.IsNull(contact2.Medium);
            Assert.IsNull(contact2.Campaign);

        }
    }
}
