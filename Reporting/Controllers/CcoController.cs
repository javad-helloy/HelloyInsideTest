using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Inside.Analytics;
using Inside.Extenssions;
using Inside.GoogleService;
using Inside.membership;
using Inside.Time;
using InsideModel.Models;
using InsideModel.repositories;
using InsideReporting.Helpers;
using InsideReporting.Models;
using InsideReporting.Models.Layout;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;

namespace InsideReporting.Controllers
{
    public class CcoController : AuthenticationController
    {
        private readonly IRepository<Client> clientRepository;
        private readonly IRepository<Contact> contactRepository;
        private readonly IServerTime serverTime;
        private IIdentityMembershipProvider userManager;
        private readonly InsideContext insideContext;
        private readonly IGoogleAnalyticsApi gaService;

        public CcoController(
            IRepository<Client> clientRepository,
            IRepository<Contact> contactRepository, 
            IServerTime serverTime,
            IIdentityMembershipProvider userManager, InsideContext insideContext, IGoogleAnalyticsApi gaService)
            : base(userManager)
        {
            this.clientRepository = clientRepository;
            this.contactRepository = contactRepository;
        this.serverTime = serverTime;
            this.userManager = userManager;
            this.insideContext = insideContext;
            this.gaService = gaService;
        }

        // GET: Cco
        [Authorize(Roles = "consultant")]
        [AuthorizeClientAccess]
        public ActionResult Latest(int clientId)
        {
            RouteValueDictionary values = new RouteValueDictionary();
            values.Add("clientid", clientId);

            var requestTime = serverTime.RequestStarted;

            values.Add("startDate", requestTime.AddMonths(-1).ToString("yyyy-MM-dd"));
            values.Add("endDate", requestTime.ToString("yyyy-MM-dd"));

            return RedirectToAction("Report", values);
        }

        // GET: Cco
        [Authorize(Roles = "consultant")]
        [AuthorizeClientAccess]
        public ActionResult Report(int clientId, DateTime startDate, DateTime endDate)
        {
            var client = clientRepository.First(c => c.Id == clientId);
            var clientViewModel = new ClientViewModel(client);
            var model = new CcoReportViewModel(userManager.GetRoles(User.Identity.GetUserId()), clientViewModel);

            model.Today = serverTime.RequestStarted;
            model.OnMonth = model.Today.AddMonths(-1);
            model.OneQuarter = model.Today.AddMonths(-3);
            model.OneYear = model.Today.AddMonths(-12);
            model.StartDate = startDate;
            model.EndDate = endDate;

            var tableId = client.AnalyticsTableId;

            var query = new AnalyticQuery
            {
                TabelId = tableId,
                StartDate = startDate,
                EndDate = endDate,
            };

            query.Metrics.AddMany("ga:adClicks", "ga:adCost");
            query.Dimenssions.AddMany("ga:keyword");
            query.MaxResults = 500;
            query.Filters.AddMany("ga:campaign!~Retargeting", "ga:campaign!~Remarketing", "ga:campaign!~Display");
            query.Sort = "-ga:adCost";
            var keywordRows = gaService.Get<KeyWordReport>(query);
            
            var keywordFinder = new Dictionary<string, KeyWordReport>();
            keywordRows.ForEach(k =>
            {
                model.KeyWords.Add(k);
                keywordFinder.Add(k.Keyword, k);
            });

            var searchPhrasesLeads = SearchPhrasesLeads(clientId, startDate, endDate);
            foreach (var searchPhraseLead in searchPhrasesLeads)
            {
                if (!string.IsNullOrEmpty(searchPhraseLead) && keywordFinder.ContainsKey(searchPhraseLead))
                {
                    keywordFinder[searchPhraseLead].NumLead = keywordFinder[searchPhraseLead].NumLead + 1;
                }
            }


            var searchPhrasesNotLeads = SearchPhrasesNotLeads(clientId, startDate, endDate);
            foreach (var searchPhraseNotLead in searchPhrasesNotLeads)
            {
                if (!string.IsNullOrEmpty(searchPhraseNotLead) && keywordFinder.ContainsKey(searchPhraseNotLead))
                {
                    keywordFinder[searchPhraseNotLead].NumNotALead = keywordFinder[searchPhraseNotLead].NumNotALead + 1;
                }
            }

            var searchPhrasesContacts = SearchPhrasesContacts(clientId, startDate, endDate);
            foreach (var searchPhraseContact in searchPhrasesContacts)
            {
                if (!string.IsNullOrEmpty(searchPhraseContact) && keywordFinder.ContainsKey(searchPhraseContact))
                {
                    keywordFinder[searchPhraseContact].Contacts = keywordFinder[searchPhraseContact].Contacts + 1;
                }
            }

            return View(model);
        }

        private IList<string> SearchPhrasesLeads(int clientId, DateTime startDate, DateTime endDate)
        {

            var query =
                @"SELECT [dbo].[ContactProperty].[Value]
                    FROM [dbo].[Contact]
                    Inner Join [dbo].[ContactProperty] on [dbo].[ContactProperty].[ContactId] = [dbo].[Contact].[Id]
                    Inner Join [dbo].[ContactInteraction] on [dbo].[ContactInteraction].[ContactId] = [dbo].[Contact].[Id]
	                AND [dbo].[ContactInteraction].[Type] = 'Rating' AND [dbo].[ContactInteraction].Value = 'Lead'
                    WHERE ClientId = " + clientId+" AND '" + startDate.ToString("yyyy-MM-dd") + "' <= [Date] AND [Date] <= '" + endDate.ToString("yyyy-MM-dd") + "' AND [dbo].[ContactProperty].[Type] = 'SearchPhrase'";

            var searchPhrases = insideContext.Database.SqlQuery<string>(query).ToList();

            return searchPhrases;
        }

        private IList<string> SearchPhrasesNotLeads(int clientId, DateTime startDate, DateTime endDate)
        {

            var query =
                @"SELECT [dbo].[ContactProperty].[Value]
                    FROM [dbo].[Contact]
                    Inner Join [dbo].[ContactProperty] on [dbo].[ContactProperty].[ContactId] = [dbo].[Contact].[Id]
                    Inner Join [dbo].[ContactInteraction] on [dbo].[ContactInteraction].[ContactId] = [dbo].[Contact].[Id]
	                AND [dbo].[ContactInteraction].[Type] = 'Rating' AND [dbo].[ContactInteraction].Value = 'Ej Lead'
                    WHERE ClientId = " + clientId + " AND '" + startDate.ToString("yyyy-MM-dd") + "' <= [Date] AND [Date] <= '" + endDate.ToString("yyyy-MM-dd") + "' AND [dbo].[ContactProperty].[Type] = 'SearchPhrase'";

            var searchPhrases = insideContext.Database.SqlQuery<string>(query).ToList();

            return searchPhrases;
        }

        private IList<string> SearchPhrasesContacts(int clientId, DateTime startDate, DateTime endDate)
        {

            var query =
                @"SELECT [dbo].[ContactProperty].[Value]
                    FROM [dbo].[Contact]
                    Inner Join [dbo].[ContactProperty] on [dbo].[ContactProperty].[ContactId] = [dbo].[Contact].[Id]
                    WHERE ClientId = " + clientId + " AND '" + startDate.ToString("yyyy-MM-dd") + "' <= [Date] AND [Date] <= '" + endDate.ToString("yyyy-MM-dd") + "' AND [dbo].[ContactProperty].[Type] = 'SearchPhrase'";

            var searchPhrases = insideContext.Database.SqlQuery<string>(query).ToList();

            return searchPhrases;
        }
    }

    public class CcoReportViewModel : LoggedInViewModel
    {
        public CcoReportViewModel(IList<string> roleList, ClientViewModel client)
        {
            Roles = roleList;
            Client = client;
            KeyWords = new List<KeyWordReport>();
            AddMenu();
        }

        public ClientViewModel Client { get; set; }
        public IList<KeyWordReport> KeyWords { get; private set; }
        public DateTime Today { get; set; }
        public DateTime OnMonth { get; set; }
        public DateTime OneQuarter { get; set; }
        public DateTime OneYear { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime StartDate { get; set; }
    }

    public class KeyWordReport
    {
        public KeyWordReport()
        {
            this.NumLead = 0;
            this.NumNotALead = 0;
            this.Contacts = 0;
        }

        [AnalyticsMapping(Source = AnalyticsDataSource.Dimension, Name = "ga:keyword")]
        public string Keyword { get; set; }

        [AnalyticsMapping(Source = AnalyticsDataSource.Metric, Name = "ga:adClicks")]
        public int Adclicks { get; set; }

        [AnalyticsMapping(Source = AnalyticsDataSource.Metric, Name = "ga:adCost")]
        public double Cost { get; set; }
        public int Contacts { get; set; }


        public double? CostPerContact
        {
            get
            {
                if (Contacts > 0){
                    return Cost/Contacts;
                }
                else
                {
                    return null;
                }
                
            }
        }

        public double? CostPerLead
        {
            get
            {
                if (NumLead > 0)
                {
                    return Cost / NumLead;
                }
                else
                {
                    return null;
                }

            }
        }

        public int NumLead { get; set; }
        public int NumNotALead { get; set; }
    }
}