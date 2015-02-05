using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Google.Apis.Analytics.v3;
using Google.GData.Analytics;
using Helpers.test;
using Inside.Analytics;
using Inside.ContactProductService;
using Inside.Extenssions;
using Inside.GoogleService;
using InsideModel.Models;
using InsideModel.repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Task.ContactProduct.AnalyticData;

namespace BackgroundWorker.Test.Analytics
{
    [TestClass]
    public class AnalyticDataPropertyExtractorTest
    {
        [TestMethod]
        public void CanCreate()
        {
            var clientRepository = new Mock<IRepository<Client>>();
            var contactRepository = new Mock<IRepository<Contact>>();
            var gaService = new Mock<IGoogleAnalyticsApi>();
            var contactProductService = new Mock<IProductService>();

            var dataExtractor = new AnalyticDataPropertyExtractor(
                clientRepository.Object,
                contactRepository.Object,
                contactProductService.Object,
                gaService.Object);
        }

        [TestMethod]
        public void UpdateCorrespondingContactWithValidDataForCalls()
        {
            var clientRepository = new LocalRepository<Client>();
            var contactRepository = new LocalRepository<Contact>();
            var contactProductService = new Mock<IProductService>();
            var gaService = new Mock<IGoogleAnalyticsApi>();

            var dataEntries = new List<ProductAnalyticData>();
            var dataEntry1 = new ProductAnalyticData();
            
            dataEntry1.ContactAction = "Action" ;
            dataEntry1.ContactLabel = "\"not set\", +468123456:first-call, 7533700";
            dataEntry1.ContactSearchPhrase= "SearchPhrase";
            dataEntry1.ContactSource= "Source";
            dataEntry1.ContactMedium = "Medium";
            dataEntry1.ContactCampaign = "Campaign";
            dataEntry1.ContactCategory = "Calls";
            dataEntries.Add(dataEntry1);

            var startDate = new DateTime(2014, 01, 01);
            var endDate = new DateTime(2014, 01, 02);

           

            var contact = new Contact()
            {
                Id = 1,
                ClientId = 1,
            };
            contact.Property.Add(new ContactProperty()
            {
                Type = "CTMId",
                Value = "7533700"
            });

            var contact2 = new Contact()
            {
                Id = 2,
                ClientId = 1,
            };
            contact2.Property.Add(new ContactProperty()
            {
                Type = "CTMId",
                Value = "1111111"
            });
            

            contactRepository.Add(contact);
            contactRepository.Add(contact2);

            var client = new Client() { Id = 1, IsActive = true, AnalyticsTableId = "ga:123456" };
            client.Leads.Add(contact);
            client.Leads.Add(contact2);

            clientRepository.Add(client);

            var analyticQuery = new AnalyticQuery();
            analyticQuery.Dimenssions.AddMany("ga:eventAction","ga:eventLabel","ga:keyword","ga:source","ga:medium","ga:campaign","ga:eventCategory");
            analyticQuery.TabelId = clientRepository.All().First().AnalyticsTableId;
            analyticQuery.StartDate = startDate;
            analyticQuery.EndDate = endDate;
            analyticQuery.Metrics.Add("ga:uniqueEvents");
            
            gaService.Setup(
               ga =>
                   ga.Get<ProductAnalyticData>(It.Is<AnalyticQuery>(
                            aq =>
                                !aq.Dimenssions.Except(analyticQuery.Dimenssions).Any() &&
                                !aq.Metrics.Except(analyticQuery.Metrics).Any() &&
                                aq.StartDate == analyticQuery.StartDate &&
                                aq.EndDate == analyticQuery.EndDate &&
                                aq.TabelId == analyticQuery.TabelId))).Returns(dataEntries);

            var dataExtractor = new AnalyticDataPropertyExtractor(
                clientRepository,
                contactRepository,
                contactProductService.Object,
                gaService.Object);

            dataExtractor.UpdateContact(client.Id, startDate, endDate );

            Assert.AreEqual(2,contactRepository.All().Count());
            Assert.AreEqual("SearchPhrase", contactRepository.Where(c => c.Id == 1).Single().SearchPhrase);
            Assert.AreEqual("Campaign", contactRepository.Where(c => c.Id == 1).Single().Campaign);
            Assert.AreEqual("Medium", contactRepository.Where(c => c.Id == 1).Single().Medium);
            Assert.AreEqual("Source", contactRepository.Where(c => c.Id == 1).Single().Source);
        }

        [TestMethod]
        public void UpdateContactWithValidDataForCallsAndIgnoreInvalidData()
        {
            var clientRepository = new LocalRepository<Client>();
            var contactRepository = new LocalRepository<Contact>();
            var gaService = new Mock<IGoogleAnalyticsApi>();
            var contactProductService = new Mock<IProductService>();

            var dataEntries = new List<ProductAnalyticData>();
            var dataEntry1 = new ProductAnalyticData();

            dataEntry1.ContactAction = "Action";
            dataEntry1.ContactLabel = "\"not set\", +468123456:first-call, 7533700";
            dataEntry1.ContactSearchPhrase = "(not set)";
            dataEntry1.ContactSource = "Source";
            dataEntry1.ContactMedium = "Medium";
            dataEntry1.ContactCampaign = "";
            dataEntry1.ContactCategory = "Calls";
            dataEntries.Add(dataEntry1);

            var startDate = new DateTime(2014, 01, 01);
            var endDate = new DateTime(2014, 01, 02);

            

            var contact = new Contact()
            {
                Id = 1,
                ClientId = 1,
            };
            contact.Property.Add(new ContactProperty()
            {
                Type = "CTMId",
                Value = "7533700"
            });

            contactRepository.Add(contact);
            
            var client = new Client() { Id = 1, IsActive = true, AnalyticsTableId = "ga:123456" };
            client.Leads.Add(contact);
            
            clientRepository.Add(client);

            var analyticQuery = new AnalyticQuery();
            analyticQuery.Dimenssions.AddMany("ga:eventAction", "ga:eventLabel", "ga:keyword", "ga:source", "ga:medium", "ga:campaign", "ga:eventCategory");
            analyticQuery.TabelId = clientRepository.All().First().AnalyticsTableId;
            analyticQuery.StartDate = startDate;
            analyticQuery.EndDate = endDate;
            analyticQuery.Metrics.Add("ga:uniqueEvents");

            gaService.Setup(
               ga =>
                   ga.Get<ProductAnalyticData>(It.Is<AnalyticQuery>(
                            aq =>
                                !aq.Dimenssions.Except(analyticQuery.Dimenssions).Any() &&
                                !aq.Metrics.Except(analyticQuery.Metrics).Any() &&
                                aq.StartDate == analyticQuery.StartDate &&
                                aq.EndDate == analyticQuery.EndDate &&
                                aq.TabelId == analyticQuery.TabelId))).Returns(dataEntries);

            var dataExtractor = new AnalyticDataPropertyExtractor(
                clientRepository,
                contactRepository,
                contactProductService.Object,
                gaService.Object);

            dataExtractor.UpdateContact(client.Id, startDate, endDate);

            Assert.AreEqual(1, contactRepository.All().Count());
            
            Assert.AreEqual("Medium", contactRepository.Where(c => c.Id == 1).Single().Medium);
            Assert.AreEqual("Source", contactRepository.Where(c => c.Id == 1).Single().Source);
            Assert.IsNull(contactRepository.Where(c => c.Id == 1).Single().SearchPhrase);
            Assert.IsNull(contactRepository.Where(c => c.Id == 1).Single().Campaign);
        }
        [TestMethod]
        public void UpdateCorrespondingContactWithValidDataForEmail()
        {
            var clientRepository = new LocalRepository<Client>();
            var contactRepository = new LocalRepository<Contact>();
            var gaService = new Mock<IGoogleAnalyticsApi>();
            var contactProductService = new Mock<IProductService>();

            var dataEntries = new List<ProductAnalyticData>();
            var dataEntry1 = new ProductAnalyticData();

            dataEntry1.ContactAction = "Mail";
            dataEntry1.ContactLabel = "{\"mandrillId\": \"MockMandrillId\"}";
            dataEntry1.ContactSearchPhrase = "SearchPhrase";
            dataEntry1.ContactSource = "Source";
            dataEntry1.ContactMedium = "Medium";
            dataEntry1.ContactCampaign = "Campaign";
            dataEntry1.ContactCategory = "HelloyContact";
            dataEntries.Add(dataEntry1);

            var startDate = new DateTime(2014, 01, 01);
            var endDate = new DateTime(2014, 01, 02);

           

            var contact = new Contact()
            {
                Id = 1,
                ClientId = 1,
            };
            contact.Property.Add(new ContactProperty()
            {
                Type = "MandrillId",
                Value = "MockMandrillId"
            });

            var contact2 = new Contact()
            {
                Id = 2,
                ClientId = 1,
            };
            contact2.Property.Add(new ContactProperty()
            {
                Type = "MandrillId",
                Value = "WrongMandrillId"
            });


            contactRepository.Add(contact);
            contactRepository.Add(contact2);

            var client = new Client() { Id = 1, IsActive = true, AnalyticsTableId = "ga:123456" };
            client.Leads.Add(contact);
            client.Leads.Add(contact2);

            clientRepository.Add(client);

            var analyticQuery = new AnalyticQuery();
            analyticQuery.Dimenssions.AddMany("ga:eventAction", "ga:eventLabel", "ga:keyword", "ga:source", "ga:medium", "ga:campaign", "ga:eventCategory");
            analyticQuery.TabelId = clientRepository.All().First().AnalyticsTableId;
            analyticQuery.StartDate = startDate;
            analyticQuery.EndDate = endDate;
            analyticQuery.Metrics.Add("ga:uniqueEvents");

            gaService.Setup(
               ga =>
                   ga.Get<ProductAnalyticData>(It.Is<AnalyticQuery>(
                            aq =>
                                !aq.Dimenssions.Except(analyticQuery.Dimenssions).Any() &&
                                !aq.Metrics.Except(analyticQuery.Metrics).Any() &&
                                aq.StartDate == analyticQuery.StartDate &&
                                aq.EndDate == analyticQuery.EndDate &&
                                aq.TabelId == analyticQuery.TabelId))).Returns(dataEntries);

            var dataExtractor = new AnalyticDataPropertyExtractor(
                clientRepository,
                contactRepository,
                contactProductService.Object,
                gaService.Object);

            dataExtractor.UpdateContact(client.Id, new DateTime(2014, 01, 01), new DateTime(2014, 01, 02));

            Assert.AreEqual(2, contactRepository.All().Count());
            Assert.AreEqual("SearchPhrase", contactRepository.Where(c => c.Id == 1).Single().SearchPhrase);
            Assert.AreEqual("Campaign", contactRepository.Where(c => c.Id == 1).Single().Campaign);
            Assert.AreEqual("Medium", contactRepository.Where(c => c.Id == 1).Single().Medium);
            Assert.AreEqual("Source", contactRepository.Where(c => c.Id == 1).Single().Source);
        }

        [TestMethod]
        public void UpdateCorrespondingContactWithValidDataForFormPost()
        {
            var clientRepository = new LocalRepository<Client>();
            var contactRepository = new LocalRepository<Contact>();
            var gaService = new Mock<IGoogleAnalyticsApi>();
            var contactProductService = new Mock<IProductService>();

            var dataEntries = new List<ProductAnalyticData>();
            var dataEntry1 = new ProductAnalyticData();

            dataEntry1.ContactAction = "Form";
            dataEntry1.ContactLabel = "{\"formPostId\": \"FormPostId\"}";
            dataEntry1.ContactSearchPhrase = "SearchPhrase";
            dataEntry1.ContactSource = "Source";
            dataEntry1.ContactMedium = "Medium";
            dataEntry1.ContactCampaign = "Campaign";
            dataEntry1.ContactCategory = "HelloyContact";
            dataEntries.Add(dataEntry1);

            var startDate = new DateTime(2014, 01, 01);
            var endDate = new DateTime(2014, 01, 02);



            var contact = new Contact()
            {
                Id = 1,
                ClientId = 1,
            };
            contact.Property.Add(new ContactProperty()
            {
                Type = "FormPostId",
                Value = "FormPostId"
            });

            var contact2 = new Contact()
            {
                Id = 2,
                ClientId = 1,
            };
            contact2.Property.Add(new ContactProperty()
            {
                Type = "FormPostId",
                Value = "WrongFormPostId"
            });


            contactRepository.Add(contact);
            contactRepository.Add(contact2);

            var client = new Client() { Id = 1, IsActive = true, AnalyticsTableId = "ga:123456" };
            client.Leads.Add(contact);
            client.Leads.Add(contact2);

            clientRepository.Add(client);

            var analyticQuery = new AnalyticQuery();
            analyticQuery.Dimenssions.AddMany("ga:eventAction", "ga:eventLabel", "ga:keyword", "ga:source", "ga:medium", "ga:campaign", "ga:eventCategory");
            analyticQuery.TabelId = clientRepository.All().First().AnalyticsTableId;
            analyticQuery.StartDate = startDate;
            analyticQuery.EndDate = endDate;
            analyticQuery.Metrics.Add("ga:uniqueEvents");

            gaService.Setup(
               ga =>
                   ga.Get<ProductAnalyticData>(It.Is<AnalyticQuery>(
                            aq =>
                                !aq.Dimenssions.Except(analyticQuery.Dimenssions).Any() &&
                                !aq.Metrics.Except(analyticQuery.Metrics).Any() &&
                                aq.StartDate == analyticQuery.StartDate &&
                                aq.EndDate == analyticQuery.EndDate &&
                                aq.TabelId == analyticQuery.TabelId))).Returns(dataEntries);

            var dataExtractor = new AnalyticDataPropertyExtractor(
                clientRepository,
                contactRepository,
                contactProductService.Object,
                gaService.Object);

            dataExtractor.UpdateContact(client.Id, new DateTime(2014, 01, 01), new DateTime(2014, 01, 02));

            Assert.AreEqual(2, contactRepository.All().Count());
            Assert.AreEqual("SearchPhrase", contactRepository.Where(c => c.Id == 1).Single().SearchPhrase);
            Assert.AreEqual("Campaign", contactRepository.Where(c => c.Id == 1).Single().Campaign);
            Assert.AreEqual("Medium", contactRepository.Where(c => c.Id == 1).Single().Medium);
            Assert.AreEqual("Source", contactRepository.Where(c => c.Id == 1).Single().Source);
        }

        [TestMethod]
        public void UpdateContactWithValidDataForEmailsAndIgnoreInvalidData()
        {
            var clientRepository = new LocalRepository<Client>();
            var contactRepository = new LocalRepository<Contact>();
            var gaService = new Mock<IGoogleAnalyticsApi>();
            var contactProductService = new Mock<IProductService>();

            var dataEntries = new List<ProductAnalyticData>();
            var dataEntry1 = new ProductAnalyticData();

            dataEntry1.ContactAction = "Mail";
            dataEntry1.ContactLabel = "{\"mandrillId\": \"MockMandrillId\"}";
            dataEntry1.ContactSearchPhrase = "(not set)";
            dataEntry1.ContactSource = "Source";
            dataEntry1.ContactMedium = "Medium";
            dataEntry1.ContactCampaign = "";
            dataEntry1.ContactCategory = "HelloyContact";
            dataEntries.Add(dataEntry1);

            var startDate = new DateTime(2014, 01, 01);
            var endDate = new DateTime(2014, 01, 02);

            

            var contact = new Contact()
            {
                Id = 1,
                ClientId = 1,
            };
            contact.Property.Add(new ContactProperty()
            {
                Type = "MandrillId",
                Value = "MockMandrillId"
            });

            contactRepository.Add(contact);

            var client = new Client() { Id = 1, IsActive = true, AnalyticsTableId = "ga:123456" };
            client.Leads.Add(contact);

            clientRepository.Add(client);

            var analyticQuery = new AnalyticQuery();
            analyticQuery.Dimenssions.AddMany("ga:eventAction", "ga:eventLabel", "ga:keyword", "ga:source", "ga:medium", "ga:campaign", "ga:eventCategory");
            analyticQuery.TabelId = clientRepository.All().First().AnalyticsTableId;
            analyticQuery.StartDate = startDate;
            analyticQuery.EndDate = endDate;
            analyticQuery.Metrics.Add("ga:uniqueEvents");

            gaService.Setup(
               ga =>
                   ga.Get<ProductAnalyticData>(It.Is<AnalyticQuery>(
                            aq =>
                                !aq.Dimenssions.Except(analyticQuery.Dimenssions).Any() &&
                                !aq.Metrics.Except(analyticQuery.Metrics).Any() &&
                                aq.StartDate == analyticQuery.StartDate &&
                                aq.EndDate == analyticQuery.EndDate &&
                                aq.TabelId == analyticQuery.TabelId))).Returns(dataEntries);

            var dataExtractor = new AnalyticDataPropertyExtractor(
                clientRepository,
                contactRepository,
                contactProductService.Object,
                gaService.Object);

            dataExtractor.UpdateContact(client.Id, new DateTime(2014, 01, 01), new DateTime(2014, 01, 02));

            Assert.AreEqual(1, contactRepository.All().Count());
            

            Assert.AreEqual("Medium", contactRepository.Where(c => c.Id == 1).Single().Medium);
            Assert.AreEqual("Source", contactRepository.Where(c => c.Id == 1).Single().Source);
            Assert.IsFalse(contactRepository.Where(c => c.Id == 1).Single().Property.Any(lp => lp.Type == "SearchPhrase"));
            Assert.IsFalse(contactRepository.Where(c => c.Id == 1).Single().Property.Any(lp => lp.Type == "Campaign"));
        }

        [TestMethod]
        public void UpdateCorrespondingContactWithValidDataForChat()
        {
            var clientRepository = new LocalRepository<Client>();
            var contactRepository = new LocalRepository<Contact>();
            var gaService = new Mock<IGoogleAnalyticsApi>();
            var contactProductService = new Mock<IProductService>();

            var dataEntries = new List<ProductAnalyticData>();
            var dataEntry1 = new ProductAnalyticData();

            dataEntry1.ContactAction = "Chat";
            dataEntry1.ContactLabel = "{\"chat_id\": \"MockLiveChatId\"}";
            dataEntry1.ContactSearchPhrase = "SearchPhrase";
            dataEntry1.ContactSource = "Source";
            dataEntry1.ContactMedium = "Medium";
            dataEntry1.ContactCampaign = "Campaign";
            dataEntry1.ContactCategory = "HelloyContact";
            dataEntries.Add(dataEntry1);

            var startDate = new DateTime(2014, 01, 01);
            var endDate = new DateTime(2014, 01, 02);

           

            var contact = new Contact()
            {
                Id = 1,
                ClientId = 1,
            };
            contact.Property.Add(new ContactProperty()
            {
                Type = "LiveChatId",
                Value = "MockLiveChatId"
            });

            var contact2 = new Contact()
            {
                Id = 2,
                ClientId = 1,
            };
            contact2.Property.Add(new ContactProperty()
            {
                Type = "LiveChatId",
                Value = "WrongLiveChatId"
            });


            contactRepository.Add(contact);
            contactRepository.Add(contact2);

            var client = new Client() { Id = 1, IsActive = true, AnalyticsTableId = "ga:123456" };
            client.Leads.Add(contact);
            client.Leads.Add(contact2);

            clientRepository.Add(client);

            var analyticQuery = new AnalyticQuery();
            analyticQuery.Dimenssions.AddMany("ga:eventAction", "ga:eventLabel", "ga:keyword", "ga:source", "ga:medium", "ga:campaign", "ga:eventCategory");
            analyticQuery.TabelId = clientRepository.All().First().AnalyticsTableId;
            analyticQuery.StartDate = startDate;
            analyticQuery.EndDate = endDate;
            analyticQuery.Metrics.Add("ga:uniqueEvents");

            gaService.Setup(
               ga =>
                   ga.Get<ProductAnalyticData>(It.Is<AnalyticQuery>(
                            aq =>
                                !aq.Dimenssions.Except(analyticQuery.Dimenssions).Any() &&
                                !aq.Metrics.Except(analyticQuery.Metrics).Any() &&
                                aq.StartDate == analyticQuery.StartDate &&
                                aq.EndDate == analyticQuery.EndDate &&
                                aq.TabelId == analyticQuery.TabelId))).Returns(dataEntries);
            
            var dataExtractor = new AnalyticDataPropertyExtractor(
                clientRepository,
                contactRepository,
                contactProductService.Object,
                gaService.Object);

            dataExtractor.UpdateContact(client.Id, new DateTime(2014, 01, 01), new DateTime(2014, 01, 02));

            Assert.AreEqual(2, contactRepository.All().Count());
            
            Assert.AreEqual("SearchPhrase", contactRepository.Where(c => c.Id == 1).Single().SearchPhrase);
            Assert.AreEqual("Campaign", contactRepository.Where(c => c.Id == 1).Single().Campaign);
            Assert.AreEqual("Medium", contactRepository.Where(c => c.Id == 1).Single().Medium);
            Assert.AreEqual("Source", contactRepository.Where(c => c.Id == 1).Single().Source);
        }

        [TestMethod]
        public void UpdateContactWithValidDataForChatAndIgnoreInvalidData()
        {
            var clientRepository = new LocalRepository<Client>();
            var contactRepository = new LocalRepository<Contact>();
            var gaService = new Mock<IGoogleAnalyticsApi>();
            var contactProductService = new Mock<IProductService>();

            var dataEntries = new List<ProductAnalyticData>();
            var dataEntry1 = new ProductAnalyticData();

            dataEntry1.ContactAction = "Chat";
            dataEntry1.ContactLabel = "{\"chat_id\": \"MockLiveChatId\"}";
            dataEntry1.ContactSearchPhrase = "(not set)";
            dataEntry1.ContactSource = "Source";
            dataEntry1.ContactMedium = "Medium";
            dataEntry1.ContactCampaign = "";
            dataEntry1.ContactCategory = "HelloyContact";
            dataEntries.Add(dataEntry1);

            var startDate = new DateTime(2014, 01, 01);
            var endDate = new DateTime(2014, 01, 02);

            
            var contact = new Contact()
            {
                Id = 1,
                ClientId = 1,
            };
            contact.Property.Add(new ContactProperty()
            {
                Type = "LiveChatId",
                Value = "MockLiveChatId"
            });

            contactRepository.Add(contact);

            var client = new Client() { Id = 1, IsActive = true, AnalyticsTableId = "ga:123456" };
            client.Leads.Add(contact);

            clientRepository.Add(client);

            var analyticQuery = new AnalyticQuery();
            analyticQuery.Dimenssions.AddMany("ga:eventAction", "ga:eventLabel", "ga:keyword", "ga:source", "ga:medium", "ga:campaign", "ga:eventCategory");
            analyticQuery.TabelId = clientRepository.All().First().AnalyticsTableId;
            analyticQuery.StartDate = startDate;
            analyticQuery.EndDate = endDate;
            analyticQuery.Metrics.Add("ga:uniqueEvents");

            gaService.Setup(
               ga =>
                   ga.Get<ProductAnalyticData>(It.Is<AnalyticQuery>(
                            aq =>
                                !aq.Dimenssions.Except(analyticQuery.Dimenssions).Any() &&
                                !aq.Metrics.Except(analyticQuery.Metrics).Any() &&
                                aq.StartDate == analyticQuery.StartDate &&
                                aq.EndDate == analyticQuery.EndDate &&
                                aq.TabelId == analyticQuery.TabelId))).Returns(dataEntries);
            
            var dataExtractor = new AnalyticDataPropertyExtractor(
                clientRepository,
                contactRepository,
                contactProductService.Object,
                gaService.Object);

            dataExtractor.UpdateContact(client.Id, new DateTime(2014, 01, 01), new DateTime(2014, 01, 02));

            Assert.AreEqual(1, contactRepository.All().Count());
            

            Assert.AreEqual("Medium", contactRepository.Where(c => c.Id == 1).Single().Medium);
            Assert.AreEqual("Source", contactRepository.Where(c => c.Id == 1).Single().Source);
            Assert.IsNull(contactRepository.Where(c => c.Id == 1).Single().SearchPhrase);
            Assert.IsNull(contactRepository.Where(c => c.Id == 1).Single().Campaign);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void InactiveContactThrowsExceptionOnUpdateContact()
        {
            var clientRepository = new LocalRepository<Client>();
            var contactRepository = new Mock<IRepository<Contact>>();
            var gaService = new Mock<IGoogleAnalyticsApi>();
            var contactProductService = new Mock<IProductService>();

            var client = new Client() { Id = 1, IsActive = false, AnalyticsTableId = "ga:123456" };
            clientRepository.Add(client);

            var dataExtractor = new AnalyticDataPropertyExtractor(
                clientRepository,
                contactRepository.Object,
                contactProductService.Object,
                gaService.Object);

            dataExtractor.UpdateContact(client.Id, new DateTime(2014, 01, 01), new DateTime(2014, 01, 02));

        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void NoAnalyticTabelIdForContactThrowsExceptionOnUpdateContact()
        {
            var clientRepository = new LocalRepository<Client>();
            var contactRepository = new Mock<IRepository<Contact>>();
            var gaService = new Mock<IGoogleAnalyticsApi>();
            var contactProductService = new Mock<IProductService>();

            var client = new Client() { Id = 1, IsActive = true };
            clientRepository.Add(client);

            var dataExtractor = new AnalyticDataPropertyExtractor(
                clientRepository,
                contactRepository.Object,
                contactProductService.Object,
                gaService.Object);

            dataExtractor.UpdateContact(client.Id, new DateTime(2014, 01, 01), new DateTime(2014, 01, 02));

        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void DoesntModifyContactOnUpdateChatWithApiAccessIdNotProvided()
        {
            var clientRepository = new LocalRepository<Client>();
            var contactRepository = new LocalRepository<Contact>();
            var gaService = new Mock<IGoogleAnalyticsApi>();
            var contactProductService = new Mock<IProductService>();

            var dataEntries = new List<ProductAnalyticData>();
            var dataEntry1 = new ProductAnalyticData();

            dataEntry1.ContactAction = "Chat";
            dataEntry1.ContactLabel = "{\"invalid_chat_id\": \"InvalidChatId\"}";
            dataEntry1.ContactSearchPhrase = "(not set)";
            dataEntry1.ContactSource = "Source";
            dataEntry1.ContactMedium = "Medium";
            dataEntry1.ContactCampaign = "";
            dataEntry1.ContactCategory = "HelloyContact";
            dataEntries.Add(dataEntry1);

            var startDate = new DateTime(2014, 01, 01);
            var endDate = new DateTime(2014, 01, 02);

            

            var contact = new Contact()
            {
                Id = 1,
                ClientId = 1,
            };
            contact.Property.Add(new ContactProperty()
            {
                Type = "LiveChatId",
                Value = "MockLiveChatId"
            });

            contactRepository.Add(contact);

            var client = new Client() { Id = 1, IsActive = true, AnalyticsTableId = "ga:123456" };
            client.Leads.Add(contact);

            clientRepository.Add(client);

            var analyticQuery = new AnalyticQuery();
            analyticQuery.Dimenssions.AddMany("ga:eventAction", "ga:eventLabel", "ga:keyword", "ga:source", "ga:medium", "ga:campaign", "ga:eventCategory");
            analyticQuery.TabelId = clientRepository.All().First().AnalyticsTableId;
            analyticQuery.StartDate = startDate;
            analyticQuery.EndDate = endDate;
            analyticQuery.Metrics.Add("ga:uniqueEvents");

            gaService.Setup(
                ga =>
                    ga.Get<ProductAnalyticData>(
                        It.Is<AnalyticQuery>(
                            aq =>
                                !aq.Dimenssions.Except(analyticQuery.Dimenssions).Any() &&
                                !aq.Metrics.Except(analyticQuery.Metrics).Any() && 
                                aq.StartDate == analyticQuery.StartDate &&
                                aq.EndDate == analyticQuery.EndDate && 
                                aq.TabelId == analyticQuery.TabelId)))
                .Returns(dataEntries);
            
            var dataExtractor = new AnalyticDataPropertyExtractor(
                clientRepository,
                contactRepository,
                contactProductService.Object,
                gaService.Object);

            dataExtractor.UpdateContact(client.Id, new DateTime(2014, 01, 01), new DateTime(2014, 01, 02));

            Assert.AreEqual(1, contactRepository.Where(c => c.Id == 1).Single().Property.Count);
            Assert.AreEqual("LiveChatId", contactRepository.Where(c => c.Id == 1).Single().Property.Single().Type);
            Assert.AreEqual("MockLiveChatId", contactRepository.Where(c => c.Id == 1).Single().Property.Single().Value);
            
        }

        [TestMethod]
        public void UpdateCorrespondingContactWithValidDataForMultipleAnalyticData()
        {
            var clientRepository = new LocalRepository<Client>();
            var contactRepository = new LocalRepository<Contact>();
            var gaService = new Mock<IGoogleAnalyticsApi>();
            var contactProductService = new Mock<IProductService>();

            var dataEntries = new List<ProductAnalyticData>();
            var dataEntry1 = new ProductAnalyticData();

            dataEntry1.ContactAction = "Chat";
            dataEntry1.ContactLabel = "{\"chat_id\": \"MockLiveChatId\"}";
            dataEntry1.ContactSearchPhrase = "SearchPhrase";
            dataEntry1.ContactSource = "Source";
            dataEntry1.ContactMedium = "Medium";
            dataEntry1.ContactCampaign = "Campaign";
            dataEntry1.ContactCategory = "HelloyContact";
            dataEntries.Add(dataEntry1);

            var dataEntry2 = new ProductAnalyticData();

            dataEntry2.ContactAction = "Mail";
            dataEntry2.ContactLabel = "{\"mandrillId\": \"MockMandrillId\"}";
            dataEntry2.ContactSearchPhrase = "SearchPhrase2";
            dataEntry2.ContactSource = "Source2";
            dataEntry2.ContactMedium = "Medium2";
            dataEntry2.ContactCampaign = "Campaign2";
            dataEntry2.ContactCategory = "HelloyContact";
            dataEntries.Add(dataEntry2);

            var startDate = new DateTime(2014, 01, 01);
            var endDate = new DateTime(2014, 01, 02);

            

            var contact = new Contact()
            {
                Id = 1,
                ClientId = 1,
            };
            contact.Property.Add(new ContactProperty()
            {
                Type = "LiveChatId",
                Value = "MockLiveChatId"
            });

            var contact2 = new Contact()
            {
                Id = 2,
                ClientId = 1,
            };
            contact2.Property.Add(new ContactProperty()
            {
                Type = "MandrillId",
                Value = "MockMandrillId"
            });


            contactRepository.Add(contact);
            contactRepository.Add(contact2);

            var client = new Client() { Id = 1, IsActive = true, AnalyticsTableId = "ga:123456" };
            client.Leads.Add(contact);
            client.Leads.Add(contact2);

            clientRepository.Add(client);

            var analyticQuery = new AnalyticQuery();
            analyticQuery.Dimenssions.AddMany("ga:eventAction", "ga:eventLabel", "ga:keyword", "ga:source", "ga:medium", "ga:campaign", "ga:eventCategory");
            analyticQuery.TabelId = clientRepository.All().First().AnalyticsTableId;
            analyticQuery.StartDate = startDate;
            analyticQuery.EndDate = endDate;
            analyticQuery.Metrics.Add("ga:uniqueEvents");

            gaService.Setup(
               ga =>
                   ga.Get<ProductAnalyticData>(It.Is<AnalyticQuery>(
                            aq =>
                                !aq.Dimenssions.Except(analyticQuery.Dimenssions).Any() &&
                                !aq.Metrics.Except(analyticQuery.Metrics).Any() &&
                                aq.StartDate == analyticQuery.StartDate &&
                                aq.EndDate == analyticQuery.EndDate &&
                                aq.TabelId == analyticQuery.TabelId))).Returns(dataEntries);
            
            var dataExtractor = new AnalyticDataPropertyExtractor(
                clientRepository,
                contactRepository,
                contactProductService.Object,
                gaService.Object);

            dataExtractor.UpdateContact(client.Id, new DateTime(2014, 01, 01), new DateTime(2014, 01, 02));

            Assert.AreEqual(2, contactRepository.All().Count());
            Assert.AreEqual("SearchPhrase", contactRepository.Where(c => c.Id == 1).Single().SearchPhrase);
            Assert.AreEqual("Campaign", contactRepository.Where(c => c.Id == 1).Single().Campaign);
            Assert.AreEqual("Medium", contactRepository.Where(c => c.Id == 1).Single().Medium);
            Assert.AreEqual("Source", contactRepository.Where(c => c.Id == 1).Single().Source);

            Assert.AreEqual("SearchPhrase2", contactRepository.Where(c => c.Id == 2).Single().SearchPhrase);
            Assert.AreEqual("Campaign2", contactRepository.Where(c => c.Id == 2).Single().Campaign);
            Assert.AreEqual("Medium2", contactRepository.Where(c => c.Id == 2).Single().Medium);
            Assert.AreEqual("Source2", contactRepository.Where(c => c.Id == 2).Single().Source);
        }

        [TestMethod]
        public void EmptyContactRepositoryDoesntDoAnythingOnUpdateContact()
        {
            var clientRepository = new LocalRepository<Client>();
            var contactRepository = new Mock<IRepository<Contact>>();
            var gaService = new Mock<IGoogleAnalyticsApi>();
            var contactProductService = new Mock<IProductService>();

            var client = new Client() { Id = 1, IsActive = true, AnalyticsTableId = "ga:123456" };
            clientRepository.Add(client);

            var startDate = new DateTime(2014, 01, 01);
            var endDate = new DateTime(2014, 01, 02);

            var analyticQuery = new AnalyticQuery();
            analyticQuery.Dimenssions.AddMany("ga:eventAction", "ga:eventLabel", "ga:keyword", "ga:source", "ga:medium", "ga:campaign", "ga:eventCategory");
            analyticQuery.TabelId = clientRepository.All().First().AnalyticsTableId;
            analyticQuery.StartDate = startDate;
            analyticQuery.EndDate = endDate;
            analyticQuery.Metrics.Add("ga:uniqueEvents");

            gaService.Setup(
               ga =>
                   ga.Get<ProductAnalyticData>(It.Is<AnalyticQuery>(
                            aq =>
                                !aq.Dimenssions.Except(analyticQuery.Dimenssions).Any() &&
                                !aq.Metrics.Except(analyticQuery.Metrics).Any() &&
                                aq.StartDate == analyticQuery.StartDate &&
                                aq.EndDate == analyticQuery.EndDate &&
                                aq.TabelId == analyticQuery.TabelId))).Returns((IEnumerable<ProductAnalyticData>)null);

            var dataExtractor = new AnalyticDataPropertyExtractor(
                clientRepository,
                contactRepository.Object,
                contactProductService.Object,
                gaService.Object);

            
            dataExtractor.UpdateContact(client.Id, startDate, endDate);

            contactRepository.Verify(cr=>cr.SaveChanges(),Times.Never);

        }

        [TestMethod]
        public void UpdateContactWithValidDataForCallsAndProductEqualsToSearch()
        {
            var clientRepository = new LocalRepository<Client>();
            var contactRepository = new LocalRepository<Contact>();
            var gaService = new Mock<IGoogleAnalyticsApi>();
            var contactProductService = new Mock<IProductService>();

            var dataEntries = new List<ProductAnalyticData>();
            var dataEntry1 = new ProductAnalyticData();

            dataEntry1.ContactAction = "Action";
            dataEntry1.ContactLabel = "\"not set\", +468123456:first-call, 7533700";
            dataEntry1.ContactSearchPhrase = "SearchPhrase";
            dataEntry1.ContactSource = "Source";
            dataEntry1.ContactMedium = "Medium";
            dataEntry1.ContactCampaign = "Campaign";
            dataEntry1.ContactCategory = "Calls";
            dataEntries.Add(dataEntry1);

            var startDate = new DateTime(2014, 01, 01);
            var endDate = new DateTime(2014, 01, 02);

           
            var contact = new Contact()
            {
                Id = 1,
                ClientId = 1,
            };
            contact.Property.Add(new ContactProperty()
            {
                Type = "CTMId",
                Value = "7533700"
            });

            contactRepository.Add(contact);
            
            var client = new Client() { Id = 1, IsActive = true, AnalyticsTableId = "ga:123456" };
            client.Leads.Add(contact);
            
            clientRepository.Add(client);

            contactProductService.Setup(cp => cp.GetProduct("Campaign", "Medium")).Returns("Search");
            contactProductService.Setup(cp => cp.IsValidProduct("Campaign", "Medium")).Returns(true);

            var analyticQuery = new AnalyticQuery();
            analyticQuery.Dimenssions.AddMany("ga:eventAction", "ga:eventLabel", "ga:keyword", "ga:source", "ga:medium", "ga:campaign", "ga:eventCategory");
            analyticQuery.TabelId = clientRepository.All().First().AnalyticsTableId;
            analyticQuery.StartDate = startDate;
            analyticQuery.EndDate = endDate;
            analyticQuery.Metrics.Add("ga:uniqueEvents");

            gaService.Setup(
               ga =>
                   ga.Get<ProductAnalyticData>(It.Is<AnalyticQuery>(
                            aq =>
                                !aq.Dimenssions.Except(analyticQuery.Dimenssions).Any() &&
                                !aq.Metrics.Except(analyticQuery.Metrics).Any() &&
                                aq.StartDate == analyticQuery.StartDate &&
                                aq.EndDate == analyticQuery.EndDate &&
                                aq.TabelId == analyticQuery.TabelId))).Returns(dataEntries);
            
            var dataExtractor = new AnalyticDataPropertyExtractor(
                clientRepository,
                contactRepository,
                contactProductService.Object,
                gaService.Object);

            dataExtractor.UpdateContact(client.Id, new DateTime(2014, 01, 01), new DateTime(2014, 01, 02));

            Assert.AreEqual(1, contactRepository.All().Count());
            
            Assert.AreEqual("SearchPhrase", contactRepository.Where(c => c.Id == 1).Single().SearchPhrase);
            Assert.AreEqual("Campaign", contactRepository.Where(c => c.Id == 1).Single().Campaign);
            Assert.AreEqual("Medium", contactRepository.Where(c => c.Id == 1).Single().Medium);
            Assert.AreEqual("Source", contactRepository.Where(c => c.Id == 1).Single().Source);
            Assert.AreEqual("Search", contactRepository.Where(c => c.Id == 1).Single().Product);
        }

        [TestMethod]
        public void UpdateContactWithValidDataForCallsAndProductEqualsToRetargeting()
        {
            var clientRepository = new LocalRepository<Client>();
            var contactRepository = new LocalRepository<Contact>();
            var gaService = new Mock<IGoogleAnalyticsApi>();
            var contactProductService = new Mock<IProductService>();

            var dataEntries = new List<ProductAnalyticData>();
            var dataEntry1 = new ProductAnalyticData();

            dataEntry1.ContactAction = "Action";
            dataEntry1.ContactLabel = "\"not set\", +468123456:first-call, 7533700";
            dataEntry1.ContactSearchPhrase = "SearchPhrase";
            dataEntry1.ContactSource = "Source";
            dataEntry1.ContactMedium = "Medium";
            dataEntry1.ContactCampaign = "Campaign";
            dataEntry1.ContactCategory = "Calls";
            dataEntries.Add(dataEntry1);

            var startDate = new DateTime(2014, 01, 01);
            var endDate = new DateTime(2014, 01, 02);

           
            var contact = new Contact()
            {
                Id = 1,
                ClientId = 1,
            };
            contact.Property.Add(new ContactProperty()
            {
                Type = "CTMId",
                Value = "7533700"
            });

            contactRepository.Add(contact);

            var client = new Client() { Id = 1, IsActive = true, AnalyticsTableId = "ga:123456" };
            client.Leads.Add(contact);

            clientRepository.Add(client);

            contactProductService.Setup(cp => cp.GetProduct("Campaign", "Medium")).Returns("Retargeting");
            contactProductService.Setup(cp => cp.IsValidProduct("Campaign", "Medium")).Returns(true);

            var analyticQuery = new AnalyticQuery();
            analyticQuery.Dimenssions.AddMany("ga:eventAction", "ga:eventLabel", "ga:keyword", "ga:source", "ga:medium", "ga:campaign", "ga:eventCategory");
            analyticQuery.TabelId = clientRepository.All().First().AnalyticsTableId;
            analyticQuery.StartDate = startDate;
            analyticQuery.EndDate = endDate;
            analyticQuery.Metrics.Add("ga:uniqueEvents");

            gaService.Setup(
               ga =>
                   ga.Get<ProductAnalyticData>(It.Is<AnalyticQuery>(
                            aq =>
                                !aq.Dimenssions.Except(analyticQuery.Dimenssions).Any() &&
                                !aq.Metrics.Except(analyticQuery.Metrics).Any() &&
                                aq.StartDate == analyticQuery.StartDate &&
                                aq.EndDate == analyticQuery.EndDate &&
                                aq.TabelId == analyticQuery.TabelId))).Returns(dataEntries);
            
            var dataExtractor = new AnalyticDataPropertyExtractor(
                clientRepository,
                contactRepository,
                contactProductService.Object,
                gaService.Object);

            dataExtractor.UpdateContact(client.Id, new DateTime(2014, 01, 01), new DateTime(2014, 01, 02));

            Assert.AreEqual(1, contactRepository.All().Count());
            
            Assert.AreEqual("SearchPhrase", contactRepository.Where(c => c.Id == 1).Single().SearchPhrase);
            Assert.AreEqual("Campaign", contactRepository.Where(c => c.Id == 1).Single().Campaign);
            Assert.AreEqual("Medium", contactRepository.Where(c => c.Id == 1).Single().Medium);
            Assert.AreEqual("Source", contactRepository.Where(c => c.Id == 1).Single().Source);
            Assert.AreEqual("Retargeting", contactRepository.Where(c => c.Id == 1).Single().Product);
        }

        [TestMethod]
        public void UpdateContactWithValidDataForCallsAndProductEqualsToDisplay()
        {
            var clientRepository = new LocalRepository<Client>();
            var contactRepository = new LocalRepository<Contact>();
            var gaService = new Mock<IGoogleAnalyticsApi>();
            var contactProductService = new Mock<IProductService>();

            var dataEntries = new List<ProductAnalyticData>();
            var dataEntry1 = new ProductAnalyticData();

            dataEntry1.ContactAction = "Action";
            dataEntry1.ContactLabel = "\"not set\", +468123456:first-call, 7533700";
            dataEntry1.ContactSearchPhrase = "SearchPhrase";
            dataEntry1.ContactSource = "Source";
            dataEntry1.ContactMedium = "Medium";
            dataEntry1.ContactCampaign = "Campaign";
            dataEntry1.ContactCategory = "Calls";
            dataEntries.Add(dataEntry1);

            var startDate = new DateTime(2014, 01, 01);
            var endDate = new DateTime(2014, 01, 02);

            

            var contact = new Contact()
            {
                Id = 1,
                ClientId = 1,
            };
            contact.Property.Add(new ContactProperty()
            {
                Type = "CTMId",
                Value = "7533700"
            });

            contactRepository.Add(contact);

            var client = new Client() { Id = 1, IsActive = true, AnalyticsTableId = "ga:123456" };
            client.Leads.Add(contact);

            clientRepository.Add(client);

            contactProductService.Setup(cp => cp.GetProduct("Campaign", "Medium")).Returns("Display");
            contactProductService.Setup(cp => cp.IsValidProduct("Campaign", "Medium")).Returns(true);

            var analyticQuery = new AnalyticQuery();
            analyticQuery.Dimenssions.AddMany("ga:eventAction", "ga:eventLabel", "ga:keyword", "ga:source", "ga:medium", "ga:campaign", "ga:eventCategory");
            analyticQuery.TabelId = clientRepository.All().First().AnalyticsTableId;
            analyticQuery.StartDate = startDate;
            analyticQuery.EndDate = endDate;
            analyticQuery.Metrics.Add("ga:uniqueEvents");

            gaService.Setup(
               ga =>
                   ga.Get<ProductAnalyticData>(It.Is<AnalyticQuery>(
                            aq =>
                                !aq.Dimenssions.Except(analyticQuery.Dimenssions).Any() &&
                                !aq.Metrics.Except(analyticQuery.Metrics).Any() &&
                                aq.StartDate == analyticQuery.StartDate &&
                                aq.EndDate == analyticQuery.EndDate &&
                                aq.TabelId == analyticQuery.TabelId))).Returns(dataEntries);
            
            var dataExtractor = new AnalyticDataPropertyExtractor(
                clientRepository,
                contactRepository,
                contactProductService.Object,
                gaService.Object);

            dataExtractor.UpdateContact(client.Id, new DateTime(2014, 01, 01), new DateTime(2014, 01, 02));

            Assert.AreEqual(1, contactRepository.All().Count());
            
            Assert.AreEqual("SearchPhrase", contactRepository.Where(c => c.Id == 1).Single().SearchPhrase);
            Assert.AreEqual("Campaign", contactRepository.Where(c => c.Id == 1).Single().Campaign);
            Assert.AreEqual("Medium", contactRepository.Where(c => c.Id == 1).Single().Medium);
            Assert.AreEqual("Source", contactRepository.Where(c => c.Id == 1).Single().Source);
            Assert.AreEqual("Display", contactRepository.Where(c => c.Id == 1).Single().Product);
        }

        [TestMethod]
        public void UpdateContactWithValidDataForCallsAndInvalidProductDisregardsProductProperty()
        {
            var clientRepository = new LocalRepository<Client>();
            var contactRepository = new LocalRepository<Contact>();
            var gaService = new Mock<IGoogleAnalyticsApi>();
            var contactProductService = new Mock<IProductService>();

            var dataEntries = new List<ProductAnalyticData>();
            var dataEntry1 = new ProductAnalyticData();

            dataEntry1.ContactAction = "Action";
            dataEntry1.ContactLabel = "\"not set\", +468123456:first-call, 7533700";
            dataEntry1.ContactSearchPhrase = "SearchPhrase";
            dataEntry1.ContactSource = "Source";
            dataEntry1.ContactMedium = "Medium";
            dataEntry1.ContactCampaign = "Campaign";
            dataEntry1.ContactCategory = "Calls";
            dataEntries.Add(dataEntry1);

            var startDate = new DateTime(2014, 01, 01);
            var endDate = new DateTime(2014, 01, 02);

            

            var contact = new Contact()
            {
                Id = 1,
                ClientId = 1,
            };
            contact.Property.Add(new ContactProperty()
            {
                Type = "CTMId",
                Value = "7533700"
            });

            contactRepository.Add(contact);

            var client = new Client() { Id = 1, IsActive = true, AnalyticsTableId = "ga:123456" };
            client.Leads.Add(contact);

            clientRepository.Add(client);

            contactProductService.Setup(cp => cp.IsValidProduct("Campaign", "Medium")).Returns(false);

            var analyticQuery = new AnalyticQuery();
            analyticQuery.Dimenssions.AddMany("ga:eventAction", "ga:eventLabel", "ga:keyword", "ga:source", "ga:medium", "ga:campaign", "ga:eventCategory");
            analyticQuery.TabelId = clientRepository.All().First().AnalyticsTableId;
            analyticQuery.StartDate = startDate;
            analyticQuery.EndDate = endDate;
            analyticQuery.Metrics.Add("ga:uniqueEvents");

            gaService.Setup(
               ga =>
                   ga.Get<ProductAnalyticData>(It.Is<AnalyticQuery>(
                            aq =>
                                !aq.Dimenssions.Except(analyticQuery.Dimenssions).Any() &&
                                !aq.Metrics.Except(analyticQuery.Metrics).Any() &&
                                aq.StartDate == analyticQuery.StartDate &&
                                aq.EndDate == analyticQuery.EndDate &&
                                aq.TabelId == analyticQuery.TabelId))).Returns(dataEntries);
            
            var dataExtractor = new AnalyticDataPropertyExtractor(
                clientRepository,
                contactRepository,
                contactProductService.Object,
                gaService.Object);

            dataExtractor.UpdateContact(client.Id, new DateTime(2014, 01, 01), new DateTime(2014, 01, 02));

            Assert.AreEqual(1, contactRepository.All().Count());
            
            Assert.AreEqual("SearchPhrase", contactRepository.Where(c => c.Id == 1).Single().SearchPhrase);
            Assert.AreEqual("Campaign", contactRepository.Where(c => c.Id == 1).Single().Campaign);
            Assert.AreEqual("Medium", contactRepository.Where(c => c.Id == 1).Single().Medium);
            Assert.AreEqual("Source", contactRepository.Where(c => c.Id == 1).Single().Source);
            Assert.IsNull(contactRepository.Where(c => c.Id == 1).Single().Product);
        }
    }
}
