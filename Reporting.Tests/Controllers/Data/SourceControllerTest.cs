using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Results;
using Helpers.test;
using Inside.ContactProductService;
using Inside.Extenssions;
using Inside.GoogleService;
using Inside.HelloyIndex;
using Inside.membership;
using InsideModel.Models;
using InsideModel.repositories;
using InsideReporting.Controllers.Api;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace InsideReporting.Tests.Controllers.Data
{
    [TestClass]
    public class SourceControllerTest
    {
        [TestMethod]
        public void CanCreate()
        {
            var clientRepositoryMoq = new Mock<IRepository<Client>>();
           var contactProductService = new Mock<IProductService>();
            var userManager = new Mock<IIdentityMembershipProvider>();
            var gaService = new Mock<IGoogleAnalyticsApi>();
            var contactIndexCalculator = new Mock<IContactIndexCalculator>();
            var controller = new SourceController(clientRepositoryMoq.Object,
               contactProductService.Object,
                userManager.Object,
                gaService.Object,
                contactIndexCalculator.Object);
        }

       

        [TestMethod]
        public void CanGetKeywordsForSearch()
        {

            var gaService = new Mock<IGoogleAnalyticsApi>();
             var clientRepository = RepositoryHelper.RepositoryLocalWithOneClient;
            var clientInRepository = clientRepository.All().First();
            var contactProductService = new Mock<IProductService>();
            var userManager = new Mock<IIdentityMembershipProvider>();
            var contactIndexCalculator = new Mock<IContactIndexCalculator>();

            var startDate = new DateTime(2013, 1, 1);
            var endDate = new DateTime(2013, 1, 3);

            var mapRowResponse = new List<SearchDetailsRow>();

            var row1 = new SearchDetailsRow
            {
                Keyword = "Keyword1",
                AdClicks = 10,
                AdCost = 20,
                NumVisitors = 40,
            };

            var row2 = new SearchDetailsRow
            {
                Keyword = "Keyword2",
                AdClicks = 11,
                AdCost = 21,
                Impressions = 31,
                ContactCollection = new ContactCollection("Keyword2", 21, 31, new List<Contact>())
                {
                    IndexValue = 0.5m
                }
            };

            var contact1 = new Contact()
            {
                Date = startDate.AddDays(1),
                SearchPhrase = "Keyword1"
            };
            var contact2 = new Contact()
            {
                Date = startDate.AddDays(1),
                SearchPhrase = "Keyword2"
            };
            var contactNotInRange = new Contact()
            {
                Date = endDate.AddDays(1),
                SearchPhrase = "Keyword1"
            };
            clientRepository.All().First().Leads.Add(contact1);
            clientRepository.All().First().Leads.Add(contact2);
            clientRepository.All().First().Leads.Add(contactNotInRange);

            mapRowResponse.Add(row1);
            mapRowResponse.Add(row2);

            var analyticQuery = new AnalyticQuery();
            analyticQuery.Dimenssions.Add("ga:keyword");
            analyticQuery.TabelId = clientRepository.All().First().AnalyticsTableId;
            analyticQuery.StartDate = startDate;
            analyticQuery.EndDate = endDate;
            analyticQuery.Metrics.AddMany("ga:adClicks", "ga:adCost", "ga:impressions");
            analyticQuery.Filters.AddMany("ga:campaign!~(display)|(retargeting)|(Display)|(Retargeting)|(remarketing)|(Remarketing)","ga:medium==cpc");

            gaService.Setup(
               ga =>
                   ga.Get<SearchDetailsRow>(It.Is<AnalyticQuery>(
                            aq =>
                                !aq.Dimenssions.Except(analyticQuery.Dimenssions).Any() &&
                                !aq.Metrics.Except(analyticQuery.Metrics).Any() &&
                                !aq.Filters.Except(analyticQuery.Filters).Any() &&
                                aq.StartDate == analyticQuery.StartDate &&
                                aq.EndDate == analyticQuery.EndDate &&
                                aq.TabelId == analyticQuery.TabelId))).Returns(mapRowResponse);

            
            var controller = new SourceController(clientRepository,
               contactProductService.Object,
                userManager.Object,
                gaService.Object,
                contactIndexCalculator.Object);

            contactIndexCalculator.Setup(ic => ic.SetIndexValues(It.IsAny<IEnumerable<ContactCollection>>())).Callback(
                (IEnumerable<ContactCollection> contactColletions) =>
                {
                   
                    foreach (var contactCollection in contactColletions)
                    {
                        if (contactCollection.Id == "Keyword1")
                        {
                            contactCollection.IndexValue = 1.0m;
                        }
                        else
                        {
                            contactCollection.IndexValue = 0.5m;
                        }
                    }
                } );
           
            var result = controller.GetSearchKeywords(clientInRepository.Id, startDate,
                endDate) as OkNegotiatedContentResult<IOrderedEnumerable<SearchDetailsRow>>;


            Assert.AreEqual(2, result.Content.Count());

            Assert.AreEqual(10, result.Content.Single(kw => kw.Name == "Keyword1").AdClicks);
            Assert.AreEqual(20, result.Content.Single(kw => kw.Name == "Keyword1").AdCost);
            Assert.AreEqual(null, result.Content.Single(kw => kw.Name == "Keyword1").Impressions);
            Assert.AreEqual(40, result.Content.Single(kw => kw.Name == "Keyword1").NumVisitors);
            Assert.AreEqual(1, result.Content.Single(kw => kw.Name == "Keyword1").NumContacts);

            Assert.AreEqual(11, result.Content.Single(kw => kw.Name == "Keyword2").AdClicks);
            Assert.AreEqual(21, result.Content.Single(kw => kw.Name == "Keyword2").AdCost);
            Assert.AreEqual(31, result.Content.Single(kw => kw.Name == "Keyword2").Impressions);
            Assert.AreEqual(1, result.Content.Single(kw => kw.Name == "Keyword2").NumContacts);

        }

       

        [TestMethod]
        public void CanGetCampaignDetailsReturnsNoCampaignForNoProductAndNoAdClick()
        {
            var clientRepository = RepositoryHelper.RepositoryLocalWithOneClient;
            var gaService = new Mock<IGoogleAnalyticsApi>();

            var userManager = new Mock<IIdentityMembershipProvider>();
            var contactProductService = new Mock<IProductService>();

            var contactIndexCalculator = new Mock<IContactIndexCalculator>();
            var controller = new SourceController(clientRepository,
               contactProductService.Object,
                userManager.Object,
                gaService.Object,
                contactIndexCalculator.Object);

            var startDate = new DateTime(2014, 1, 1);
            var endDate = new DateTime(2014, 2, 1);

            var mapRowResponse = new List<CampaignDetails>();
            var row1 = new CampaignDetails
            {
                Campaign = "Campaign1",
                SourceMedium = "google/cpc",
                AdCost = 20,
                Sessions = 0
            };

            var row2 = new CampaignDetails
            {
                Campaign = "Campaign2",
                SourceMedium = "Adroll/cpc",
                AdCost = 30,
                AdClicksMetric = 10
            };

            mapRowResponse.Add(row1);
            mapRowResponse.Add(row2);

            var analyticQuery = new AnalyticQuery();
            analyticQuery.Dimenssions.AddMany("ga:sourceMedium","ga:campaign");
            analyticQuery.TabelId = clientRepository.All().First().AnalyticsTableId;
            analyticQuery.StartDate = startDate;
            analyticQuery.EndDate = endDate;
            analyticQuery.Metrics.AddMany("ga:adClicks", "ga:adCost", "ga:impressions", "ga:sessions");
            analyticQuery.Filters.Add("ga:campaign!=(not set)");

            gaService.Setup(
               ga =>
                   ga.Get<CampaignDetails>(It.Is<AnalyticQuery>(
                            aq =>
                                !aq.Dimenssions.Except(analyticQuery.Dimenssions).Any() &&
                                !aq.Metrics.Except(analyticQuery.Metrics).Any() &&
                                !aq.Filters.Except(analyticQuery.Filters).Any() &&
                                aq.StartDate == analyticQuery.StartDate &&
                                aq.EndDate == analyticQuery.EndDate &&
                                aq.TabelId == analyticQuery.TabelId))).Returns(mapRowResponse);
            
            contactProductService.Setup(cp => cp.IsValidProduct(It.IsAny<string>(), "cpc")).Returns(false);
            
            var result =
                controller.GetCampaignsDetail(1, startDate, endDate)
                 as OkNegotiatedContentResult<IEnumerable<CampaignDetails>>;

            Assert.AreEqual(0, result.Content.Count());
        }

        [TestMethod]
        public void CanGetCampaignDetailsWithServeralCampaignsIgnoreInvalidData()
        {
            var clientRepository = RepositoryHelper.RepositoryLocalWithOneClient;
            var gaService = new Mock<IGoogleAnalyticsApi>();

            var userManager = new Mock<IIdentityMembershipProvider>();
            var contactProductService = new Mock<IProductService>();

            var contactIndexCalculator = new Mock<IContactIndexCalculator>();
            var controller = new SourceController(clientRepository,
               contactProductService.Object,
                userManager.Object,
                gaService.Object,
                contactIndexCalculator.Object);

            var startDate = new DateTime(2014, 1, 1);
            var endDate = new DateTime(2014, 2, 1);

            var mapRowResponse = new List<CampaignDetails>();
            var row1 = new CampaignDetails
            {
                Campaign = "Campaign1",
                SourceMedium = "google/cpc",
                AdCost = 20,
                Sessions = 10
            };

            var row2 = new CampaignDetails
            {
                Campaign = "Campaign2",
                SourceMedium = "Adroll/cpc",
                AdCost = 30,
                AdClicksMetric = 20,
                Sessions = 0
            };
            var invalidRow = new CampaignDetails
            {
                Campaign = "Campaign1",
                SourceMedium = "google/direct",
                AdCost = 20,
                Sessions = 0
            };
            mapRowResponse.Add(row1);
            mapRowResponse.Add(row2);
            mapRowResponse.Add(invalidRow);

            var contact1 = new Contact()
            {
                Date = startDate.AddDays(1),
                Campaign = "Campaign1"
            };
            var contact2 = new Contact()
            {
                Date = startDate.AddDays(1),
                Campaign = "Campaign2"
            };
            var contactNotInRange = new Contact()
            {
                Date = endDate.AddDays(1),
                Campaign = "Campaign1"
            };
            clientRepository.All().First().Leads.Add(contact1);
            clientRepository.All().First().Leads.Add(contact2);
            clientRepository.All().First().Leads.Add(contactNotInRange);

            var analyticQuery = new AnalyticQuery();
            analyticQuery.Dimenssions.AddMany("ga:sourceMedium", "ga:campaign");
            analyticQuery.TabelId = clientRepository.All().First().AnalyticsTableId;
            analyticQuery.StartDate = startDate;
            analyticQuery.EndDate = endDate;
            analyticQuery.Metrics.AddMany("ga:adClicks", "ga:adCost", "ga:impressions", "ga:sessions");
            analyticQuery.Filters.Add("ga:campaign!=(not set)");

            gaService.Setup(
               ga =>
                   ga.Get<CampaignDetails>(It.Is<AnalyticQuery>(
                            aq =>
                                !aq.Dimenssions.Except(analyticQuery.Dimenssions).Any() &&
                                !aq.Metrics.Except(analyticQuery.Metrics).Any() &&
                                !aq.Filters.Except(analyticQuery.Filters).Any() &&
                                aq.StartDate == analyticQuery.StartDate &&
                                aq.EndDate == analyticQuery.EndDate &&
                                aq.TabelId == analyticQuery.TabelId))).Returns(mapRowResponse);
            
            contactProductService.Setup(cp => cp.IsValidProduct(It.IsAny<string>(), "cpc")).Returns(true);
            contactProductService.Setup(cp => cp.GetProduct(It.IsAny<string>(), "cpc")).Returns("Display");
            
            var result =
                controller.GetCampaignsDetail(1, startDate, endDate)
                 as OkNegotiatedContentResult<IEnumerable<CampaignDetails>>;

            Assert.AreEqual(2, result.Content.Count());

            Assert.AreEqual(10, result.Content.Single(c => c.Campaign == "Campaign1").AdClicks);
            Assert.AreEqual(10, result.Content.Single(c => c.Campaign == "Campaign1").Sessions);
            Assert.AreEqual(false, result.Content.Single(c => c.Campaign == "Campaign1").AdClicksMetric.HasValue);
            Assert.AreEqual(20, result.Content.Single(c => c.Campaign == "Campaign1").AdCost);
            Assert.AreEqual("Adwords", result.Content.Single(c => c.Campaign == "Campaign1").AdProvider);
            Assert.AreEqual("google", result.Content.Single(c => c.Campaign == "Campaign1").Source);
            Assert.AreEqual("cpc", result.Content.Single(c => c.Campaign == "Campaign1").Medium);
            Assert.AreEqual("Display", result.Content.Single(c => c.Campaign == "Campaign1").Product);
            Assert.AreEqual(1, result.Content.Single(c => c.Campaign == "Campaign1").NumContacts);

            Assert.AreEqual(20, result.Content.Single(c => c.Campaign == "Campaign2").AdClicks);
            Assert.AreEqual(0, result.Content.Single(c => c.Campaign == "Campaign2").Sessions);
            Assert.AreEqual(true, result.Content.Single(c => c.Campaign == "Campaign2").AdClicksMetric.HasValue);
            Assert.AreEqual(20, result.Content.Single(c => c.Campaign == "Campaign2").AdClicksMetric);
            Assert.AreEqual(30, result.Content.Single(c => c.Campaign == "Campaign2").AdCost);
            Assert.AreEqual("Adroll", result.Content.Single(c => c.Campaign == "Campaign2").AdProvider);
            Assert.AreEqual("Adroll", result.Content.Single(c => c.Campaign == "Campaign2").Source);
            Assert.AreEqual("cpc", result.Content.Single(c => c.Campaign == "Campaign2").Medium);
            Assert.AreEqual("Display", result.Content.Single(c => c.Campaign == "Campaign2").Product);
            Assert.AreEqual(1, result.Content.Single(c => c.Campaign == "Campaign2").NumContacts);
        }

        [TestMethod]
        public void CanGetVisitorsDaily()
        {
            var clientRepository = RepositoryHelper.RepositoryLocalWithOneClient;
            var gaService = new Mock<IGoogleAnalyticsApi>();

            var userManager = new Mock<IIdentityMembershipProvider>();
            var contactProductService = new Mock<IProductService>();

            var contactIndexCalculator = new Mock<IContactIndexCalculator>();
            var controller = new SourceController(clientRepository,
               contactProductService.Object,
                userManager.Object,
                gaService.Object,
                contactIndexCalculator.Object);

            var startDate = new DateTime(2014, 1, 1);
            var endDate = new DateTime(2014, 1, 2);

            var mapRowResponse = new List<VisitorsOverTimeGaData>();
            var row1 = new VisitorsOverTimeGaData
            {
                Date = "20140101",
                Visitors = 100
            };

            var row2 = new VisitorsOverTimeGaData
            {
                Date = "20140102",
                Visitors = 70
            };
            
            mapRowResponse.Add(row1);
            mapRowResponse.Add(row2);

            var analyticQuery = new AnalyticQuery();
            analyticQuery.Dimenssions.AddMany("ga:date");
            analyticQuery.TabelId = clientRepository.All().First().AnalyticsTableId;
            analyticQuery.StartDate = startDate;
            analyticQuery.EndDate = endDate;
            analyticQuery.Metrics.Add("ga:visitors");

            gaService.Setup(
               ga =>
                   ga.Get<VisitorsOverTimeGaData>(It.Is<AnalyticQuery>(
                            aq =>
                                !aq.Dimenssions.Except(analyticQuery.Dimenssions).Any() &&
                                !aq.Metrics.Except(analyticQuery.Metrics).Any() &&
                                !aq.Filters.Except(analyticQuery.Filters).Any() &&
                                aq.StartDate == analyticQuery.StartDate &&
                                aq.EndDate == analyticQuery.EndDate &&
                                aq.TabelId == analyticQuery.TabelId))).Returns(mapRowResponse);
            
            var result =
                controller.GetVisitorsOverTime(1, startDate, endDate,"Daily")
                 as OkNegotiatedContentResult<List<VisitorStats>>;

            Assert.AreEqual(2, result.Content.Count());

            Assert.AreEqual(100, result.Content.Single(r=>r.date==startDate).visitors);

            Assert.AreEqual(70, result.Content.Single(r => r.date == endDate).visitors);

        }

        [TestMethod]
        public void CanGetVisitorsMonthly()
        {
            var clientRepository = RepositoryHelper.RepositoryLocalWithOneClient;
            var gaService = new Mock<IGoogleAnalyticsApi>();

            var userManager = new Mock<IIdentityMembershipProvider>();
            var contactProductService = new Mock<IProductService>();

            var contactIndexCalculator = new Mock<IContactIndexCalculator>();
            var controller = new SourceController(clientRepository,
               contactProductService.Object,
                userManager.Object,
                gaService.Object,
                contactIndexCalculator.Object);

            var startDate = new DateTime(2014, 1, 1);
            var endDate = new DateTime(2014, 3, 1);

            var mapRowResponse = new List<VisitorsOverTimeGaData>();
            var row1 = new VisitorsOverTimeGaData
            {
                Date = "201401",
                Visitors = 100
            };

            var row2 = new VisitorsOverTimeGaData
            {
                Date = "201402",
                Visitors = 170
            };

            var row3 = new VisitorsOverTimeGaData
            {
                Date = "201403",
                Visitors = 70
            };

            mapRowResponse.Add(row1);
            mapRowResponse.Add(row2);
            mapRowResponse.Add(row3);

            var analyticQuery = new AnalyticQuery();
            analyticQuery.Dimenssions.AddMany("ga:yearmonth");
            analyticQuery.TabelId = clientRepository.All().First().AnalyticsTableId;
            analyticQuery.StartDate = startDate;
            analyticQuery.EndDate = endDate;
            analyticQuery.Metrics.Add("ga:visitors");

            gaService.Setup(
               ga =>
                   ga.Get<VisitorsOverTimeGaData>(It.Is<AnalyticQuery>(
                            aq =>
                                !aq.Dimenssions.Except(analyticQuery.Dimenssions).Any() &&
                                !aq.Metrics.Except(analyticQuery.Metrics).Any() &&
                                !aq.Filters.Except(analyticQuery.Filters).Any() &&
                                aq.StartDate == analyticQuery.StartDate &&
                                aq.EndDate == analyticQuery.EndDate &&
                                aq.TabelId == analyticQuery.TabelId))).Returns(mapRowResponse);
            
            var result =
                controller.GetVisitorsOverTime(1, startDate, endDate, "Monthly")
                 as OkNegotiatedContentResult<List<VisitorStats>>;

            Assert.AreEqual(3, result.Content.Count());

            Assert.AreEqual(100, result.Content.Single(r => r.date == startDate).visitors);

            Assert.AreEqual(170, result.Content.Single(r => r.date == startDate.AddMonths(1)).visitors);

            Assert.AreEqual(70, result.Content.Single(r => r.date == endDate).visitors);

        }


        [TestMethod]
        public void CanGetVisitorsYearly()
        {
            var clientRepository = RepositoryHelper.RepositoryLocalWithOneClient;
            var gaService = new Mock<IGoogleAnalyticsApi>();

            var userManager = new Mock<IIdentityMembershipProvider>();
            var contactProductService = new Mock<IProductService>();

            var contactIndexCalculator = new Mock<IContactIndexCalculator>();
            var controller = new SourceController(clientRepository,
               contactProductService.Object,
                userManager.Object,
                gaService.Object,
                contactIndexCalculator.Object);

            var startDate = new DateTime(2012, 1, 1);
            var endDate = new DateTime(2014, 1, 1);

            var mapRowResponse = new List<VisitorsOverTimeGaData>();
            var row1 = new VisitorsOverTimeGaData
            {
                Date = "2014",
                Visitors = 100
            };

            var row2 = new VisitorsOverTimeGaData
            {
                Date = "2013",
                Visitors = 170
            };

            var row3 = new VisitorsOverTimeGaData
            {
                Date = "2012",
                Visitors = 70
            };

            mapRowResponse.Add(row1);
            mapRowResponse.Add(row2);
            mapRowResponse.Add(row3);

            var analyticQuery = new AnalyticQuery();
            analyticQuery.Dimenssions.AddMany("ga:year");
            analyticQuery.TabelId = clientRepository.All().First().AnalyticsTableId;
            analyticQuery.StartDate = startDate;
            analyticQuery.EndDate = endDate;
            analyticQuery.Metrics.Add("ga:visitors");

            gaService.Setup(
               ga =>
                   ga.Get<VisitorsOverTimeGaData>(It.Is<AnalyticQuery>(
                            aq =>
                                !aq.Dimenssions.Except(analyticQuery.Dimenssions).Any() &&
                                !aq.Metrics.Except(analyticQuery.Metrics).Any() &&
                                !aq.Filters.Except(analyticQuery.Filters).Any() &&
                                aq.StartDate == analyticQuery.StartDate &&
                                aq.EndDate == analyticQuery.EndDate &&
                                aq.TabelId == analyticQuery.TabelId))).Returns(mapRowResponse);
            
            var result =
                controller.GetVisitorsOverTime(1, startDate, endDate, "Yearly")
                 as OkNegotiatedContentResult<List<VisitorStats>>;

            Assert.AreEqual(3, result.Content.Count());

            Assert.AreEqual(70, result.Content.Single(r => r.date == startDate).visitors);

            Assert.AreEqual(170, result.Content.Single(r => r.date == startDate.AddYears(1)).visitors);

            Assert.AreEqual(100, result.Content.Single(r => r.date == endDate).visitors);

        }
    }
}
