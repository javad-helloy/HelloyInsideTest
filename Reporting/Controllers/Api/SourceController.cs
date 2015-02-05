using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web.Http;
using System.Web.Mvc;
using Inside.Analytics;
using Inside.ContactProductService;
using Inside.Extenssions;
using Inside.GoogleService;
using Inside.HelloyIndex;
using Inside.membership;
using InsideModel.Models;
using InsideModel.repositories;
using InsideReporting.Helpers;
using Microsoft.AspNet.Identity;
using Ninject.Infrastructure.Language;

namespace InsideReporting.Controllers.Api
{
    public class SourceController : ApiAuthenticationController
    {
         private readonly IRepository<Client> clienRepository;
        private readonly IProductService contactProductService;
        private IIdentityMembershipProvider userManager;
        private readonly IGoogleAnalyticsApi gaService;
        private readonly IContactIndexCalculator contactIndexCalculator;


        public SourceController(
            IRepository<Client> clienRepository, 
            IProductService contactProductService,
            IIdentityMembershipProvider userManager,
            IGoogleAnalyticsApi gaService,
            IContactIndexCalculator contactIndexCalculator)
            : base(userManager)
        {
            //this.container = container;
            this.clienRepository = clienRepository;
            this.contactProductService = contactProductService;
            this.userManager = userManager;
            this.gaService = gaService;
            this.contactIndexCalculator = contactIndexCalculator;
        }
        //
        // GET: /Source/GetSourceOverTime
        
        [System.Web.Http.Route("api/client/{clientId}/source/visitor")]
        [AuthorizeClientAPIAccess]
        public IHttpActionResult GetVisitorsOverTime(int clientId, DateTime startDate, DateTime endDate, string aggregateType)
        {
            var tableId = clienRepository.Where(c => c.Id == clientId).Single().AnalyticsTableId;
            string dimentions = "";
            var aggregateFormat = "";
            if (aggregateType == "Daily")
            {

                dimentions =  "ga:date" ;
                aggregateFormat = "yyyyMMdd";
            }
            else if (aggregateType == "Monthly")
            {
                dimentions =  "ga:yearmonth" ;
                aggregateFormat = "yyyyMM";
            }
            else if (aggregateType == "Yearly")
            {
                dimentions = "ga:year" ;
                aggregateFormat = "yyyy";
            }

            var query = new AnalyticQuery
            {
                TabelId = tableId,
                StartDate = startDate,
                EndDate = endDate,
            };

            query.Metrics.AddMany("ga:visitors");
            query.Dimenssions.AddMany(dimentions);

            var response = gaService.Get<VisitorsOverTimeGaData>(query);

            var sourceByDay = response
                .Select(row => new VisitorStats
                        {
                            date = DateTime.ParseExact(row.AggregationDate, aggregateFormat, CultureInfo.InvariantCulture),
                            visitors = row.Visitors
                        })
                        .ToList();

            if (userManager.IsInRole(User.Identity.GetUserId(), "demo"))
            {
                var r = new Random();
                sourceByDay = response
                .Select(dataEntry => new VisitorStats
                {
                    date = DateTime.ParseExact(dataEntry.AggregationDate, aggregateFormat, CultureInfo.InvariantCulture),
                    visitors = r.Next(0, 100)
                })
                .ToList();
            }

            if (sourceByDay.Any())
            {
                return Ok(sourceByDay);
            }
            else
            {
                return NotFound();
            }
        }

        //[OutputCache(Duration = 60*60)]
        [AuthorizeClientAPIAccess]
        [System.Web.Http.Route("api/client/{clientId}/source/searchkeywords")]
        public IHttpActionResult GetSearchKeywords(int clientId, DateTime startDate, DateTime endDate)
        {

            if (userManager.IsInRole(User.Identity.GetUserId(), "demo"))
            {
               
                var demoKeywords = new List<SearchDetailsRow>();
                AddKeywordDemoData(demoKeywords);

                return Ok(demoKeywords.OrderByDescending(k=>k.ContactCollection.IndexValue));
            }

            var client = clienRepository.Where(c => c.Id == clientId).Single();
            var tableId = client.AnalyticsTableId;

            var query = new AnalyticQuery
            {
                TabelId = tableId,
                StartDate = startDate,
                EndDate = endDate,
            };

            query.Metrics.AddMany("ga:adClicks","ga:adCost","ga:impressions");
            query.Dimenssions.AddMany("ga:keyword");
            query.MaxResults = 50;
            query.Filters.AddMany("ga:campaign!~(display)|(retargeting)|(Display)|(Retargeting)|(remarketing)|(Remarketing)","ga:medium==cpc");
            query.Sort = "-ga:adCost";

            var keywordFeed = gaService.Get<SearchDetailsRow>(query).ToList();
            
            foreach (var searchDetailsRow in keywordFeed)
            {
                var row = searchDetailsRow;
                var contactsForKeyword =
                    client.Leads.Where(
                        c =>c.SearchPhrase != null && c.SearchPhrase.Equals(row.Keyword) 
                            && c.Date >= startDate && c.Date <= endDate);

                var cost = searchDetailsRow.AdCost.HasValue ? (decimal) searchDetailsRow.AdCost : 0m;
                var clicks = searchDetailsRow.AdClicks.HasValue ? (int)searchDetailsRow.AdClicks : 0;
                searchDetailsRow.ContactCollection = new ContactCollection(searchDetailsRow.Name, cost, clicks, contactsForKeyword.ToList());
                searchDetailsRow.NumContacts = contactsForKeyword.Count();
            }

            if (keywordFeed.Any())
            {
                contactIndexCalculator.SetIndexValues(keywordFeed.Select(k => k.ContactCollection));   
            }

            return Ok(keywordFeed.OrderByDescending(k=>k.ContactCollection.IndexValue));
        }

        [OutputCache(Duration = 60 * 60)]
        [AuthorizeClientAPIAccess]
        [System.Web.Http.Route("api/client/{clientId}/source/campaignsdetails")]
        public IHttpActionResult GetCampaignsDetail(int clientId, DateTime startDate, DateTime endDate)
        {
           
            if (userManager.IsInRole(User.Identity.GetUserId(), "demo"))
            {
                var demoCampaigns = new List<CampaignDetails>();
                var random = new Random(DateTime.Now.Second);

                demoCampaigns.Add(new CampaignDetails()
                {
                    Campaign = "Kampanj",
                    AdClicksMetric = random.Next(0,1000),
                    AdCost = random.NextDouble()*10000,
                    SourceMedium = "google/cpc",
                    Impressions = random.Next(0, 10000),
                    Product = "Search",
                    NumContacts = random.Next(0,200),
                    ContactCollection = new ContactCollection("Sökord 1", 0, 0, new List<Contact>())
                    {
                        IndexValue = 1
                    }
                });

                demoCampaigns.Add(new CampaignDetails()
                {
                    Campaign = "Kampanj",
                    AdClicksMetric = random.Next(0, 500),
                    AdCost = random.NextDouble() * 3000,
                    Impressions = random.Next(0, 10000),
                    SourceMedium = "Bing/cpc",
                    Product = "Search",
                    NumContacts = random.Next(0, 50),
                    ContactCollection = new ContactCollection("Sökord 1", 0, 0, new List<Contact>())
                    {
                        IndexValue = 0.7m
                    }
                });

                demoCampaigns.Add(new CampaignDetails()
                {
                    Campaign = "Kampanj Retargeting",
                    AdClicksMetric = random.Next(0, 500),
                    AdCost = random.NextDouble() * 1000,
                    Impressions = random.Next(0, 10000),
                    SourceMedium = "Adroll/cpc",
                    Product = "Retargeting",
                    NumContacts = random.Next(0, 50),
                    ContactCollection = new ContactCollection("Sökord 1", 0, 0, new List<Contact>())
                    {
                        IndexValue = 0.6m
                    }
                });

                return Ok(demoCampaigns);
            }

            var client = clienRepository.Where(c => c.Id == clientId).Single();
            var tableId = client.AnalyticsTableId;

            var query = new AnalyticQuery
            {
                TabelId = tableId,
                StartDate = startDate,
                EndDate = endDate,
            };

            query.Metrics.AddMany("ga:adClicks", "ga:adCost", "ga:impressions", "ga:sessions");
            query.Dimenssions.AddMany("ga:sourceMedium", "ga:campaign");
            query.MaxResults = 50;
            query.Filters.AddMany("ga:campaign!=(not set)");
            query.Sort = "-ga:adCost";

            var campaignFeed = gaService.Get<CampaignDetails>(query);

            var campaigns = new List<CampaignDetails>();

            foreach (var campaignDetails in campaignFeed)
            {

                var row = campaignDetails;
                var contactsForCampaign =
                    client.Leads.Where(
                        c =>c.Campaign != null && c.Campaign.Equals(row.Campaign) 
                            && c.Date >= startDate && c.Date <= endDate);

                var cost = row.AdCost.HasValue ? (decimal)row.AdCost : 0m;
                var clicks = row.AdClicks.HasValue ? (int)row.AdClicks : 0;
                campaignDetails.ContactCollection = new ContactCollection(campaignDetails.Campaign, cost, clicks, contactsForCampaign.ToList());
                campaignDetails.NumContacts = contactsForCampaign.Count();

                if (!contactProductService.IsValidProduct(campaignDetails.Campaign, campaignDetails.Medium)) continue;

                campaignDetails.Product = contactProductService.GetProduct(campaignDetails.Campaign, campaignDetails.Medium);

                if (campaignDetails.AdClicks.HasValue && campaignDetails.AdClicks.Value > 0 &&
                    !string.IsNullOrEmpty(campaignDetails.Product))
                {
                    campaigns.Add(campaignDetails);    
                }

                
            }

            if (campaigns.Any())
            {
                contactIndexCalculator.SetIndexValues(campaigns.Select(k => k.ContactCollection));    
            }
            return Ok(campaigns.OrderByDescending(c => c.ContactCollection.IndexValue).ToEnumerable());
        }

        private void AddKeywordDemoData(List<SearchDetailsRow> demoKeywords)
        {
            var random = new Random(DateTime.Now.Second);
            demoKeywords.Add(new SearchDetailsRow()
                {
                    Keyword = "Sökord 1",
                    AdCost = random.NextDouble()*100,
                    Impressions = random.Next(0,10000),
                    AdClicks = random.Next(0, 1000),
                    NumContacts = random.Next(0, 100),
                    ContactCollection = new ContactCollection("Sökord 1",  0, 0, new List<Contact>())
                    {
                        IndexValue = (decimal)random.NextDouble()
                    }
                });

                demoKeywords.Add(new SearchDetailsRow()
                {
                    Keyword = "Sökord 2",
                    AdCost = random.NextDouble() * 100,
                    Impressions = random.Next(0, 10000),
                    AdClicks = random.Next(0, 1000),
                    NumContacts = random.Next(0, 110),
                    ContactCollection = new ContactCollection("Sökord 1", 0, 0, new List<Contact>())
                    {
                        IndexValue = (decimal)random.NextDouble()
                    }

                });

                demoKeywords.Add(new SearchDetailsRow()
                {
                    Keyword = "Sökord 3",
                    AdCost = random.NextDouble() * 100,
                    Impressions = random.Next(0, 10000),
                    AdClicks = random.Next(0, 1000),
                    NumContacts = random.Next(0, 60),
                    ContactCollection = new ContactCollection("Sökord 1", 0, 0, new List<Contact>())
                    {
                        IndexValue = (decimal)random.NextDouble()
                    }

                });

                demoKeywords.Add(new SearchDetailsRow()
                {
                    Keyword = "Sökord 4",
                    AdCost = random.NextDouble() * 100,
                    Impressions = random.Next(0, 10000),
                    AdClicks = random.Next(0, 1000),
                    NumContacts = random.Next(0, 50),
                    ContactCollection = new ContactCollection("Sökord 1", 0, 0, new List<Contact>())
                    {
                        IndexValue = (decimal)random.NextDouble()
                    }

                });

                demoKeywords.Add(new SearchDetailsRow()
                {
                    Keyword = "Sökord 5",
                    AdCost = random.NextDouble() * 100,
                    Impressions = random.Next(0, 10000),
                    AdClicks = random.Next(0, 1000),
                    NumContacts = random.Next(0, 10),
                    ContactCollection = new ContactCollection("Sökord 1", 0, 0, new List<Contact>())
                    {
                        IndexValue = (decimal)random.NextDouble()
                    }

                });
        }
    }

    public class VisitorsOverTimeGaData
    {
        [AnalyticsMapping(Source = AnalyticsDataSource.Dimension, Name = "ga:date")]
        public string Date { get; set; }

        [AnalyticsMapping(Source = AnalyticsDataSource.Dimension, Name = "ga:yearmonth")]
        public string YearMonth { get; set; }

        [AnalyticsMapping(Source = AnalyticsDataSource.Dimension, Name = "ga:year")]
        public string Year { get; set; }

        public string AggregationDate
        {
            get
            {
                if (!string.IsNullOrEmpty(Date))
                {
                    return Date;
                }
                if (!string.IsNullOrEmpty(YearMonth))
                {
                    return YearMonth;
                }
                if (!string.IsNullOrEmpty(Year))
                {
                    return Year;
                }

                return "";
            }
        }

        [AnalyticsMapping(Source = AnalyticsDataSource.Metric, Name = "ga:visitors")]
        public int Visitors { get; set; }

    }

    public class CampaignDetails
    {
        [AnalyticsMapping(Source = AnalyticsDataSource.Dimension, Name = "ga:campaign")]
        public string Campaign { get; set; }

        [AnalyticsMapping(Source = AnalyticsDataSource.Dimension, Name = "ga:sourceMedium")]
        public string SourceMedium { get; set; }

        [AnalyticsMapping(Source = AnalyticsDataSource.Metric, Name = "ga:impressions")]
        public int? Impressions { get; set; }

        [AnalyticsMapping(Source = AnalyticsDataSource.Metric, Name = "ga:adClicks")]
        public int? AdClicksMetric { get; set; }

        [AnalyticsMapping(Source = AnalyticsDataSource.Metric, Name = "ga:adCost")]
        public double? AdCost { get; set; }

        [AnalyticsMapping(Source = AnalyticsDataSource.Metric, Name = "ga:sessions")]
        public int Sessions { get; set; }

        public int? AdClicks
        {
            get
            {
                if (!AdClicksMetric.HasValue && Sessions > 0)
                {
                    return Sessions;
                }
                
                return AdClicksMetric;
            }
        }
        public string AdProvider
        {
            get
            {
                if (Source == "google")
                {
                    return "Adwords";
                }

                return Source;
            }
        }

        public string Source
        {
            get
            {
                return SourceMedium.Split('/')[0].Trim();
            }
        }

        public string Medium
        {
            get
            {
                return SourceMedium.Split('/')[1].Trim();
            }
        }

        public string Product { get; set; }
        public ContactCollection ContactCollection { get; set; }
        public int NumContacts { get; set; }
    }

    public class SearchDetailsRow
    {
        [AnalyticsMapping(Source = AnalyticsDataSource.Dimension, Name = "ga:keyword")]
        public string Keyword { get; set; }

        public string Name
        {
            get { return Keyword; }
        }

        [AnalyticsMapping(Source = AnalyticsDataSource.Metric, Name = "ga:adClicks")]
        public int? AdClicks { get; set; }

        [AnalyticsMapping(Source = AnalyticsDataSource.Metric, Name = "ga:adCost")]
        public double? AdCost { get; set; }

        [AnalyticsMapping(Source = AnalyticsDataSource.Metric, Name = "ga:impressions")]
        public double? Impressions { get; set; }

        [AnalyticsMapping(Source = AnalyticsDataSource.Metric, Name = "ga:visitors")]
        public int NumVisitors { get; set; }

        public ContactCollection ContactCollection { get; set; }
        public int NumContacts { get; set; }
    }

   
    public class VisitorStats
    {
        public DateTime date { get; set; }
        public int visitors { get; set; }
    }   
}