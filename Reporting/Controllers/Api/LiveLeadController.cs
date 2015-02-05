using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Http;
using Inside.Analytics;
using Inside.Extenssions;
using Inside.GoogleService;
using Inside.membership;
using InsideModel.Models;
using InsideModel.repositories;
using Newtonsoft.Json.Converters;

namespace InsideReporting.Controllers.Api
{
    public class LiveLeadController : ApiAuthenticationController
    {
        private readonly IRepository<Contact> contactRepository;
        private readonly IGoogleAnalyticsApi gaService;

        public LiveLeadController(IIdentityMembershipProvider userManager, 
                                  IRepository<Contact> contactRepository,
                                  IGoogleAnalyticsApi gaService)
            : base(userManager)
        {
            this.contactRepository = contactRepository;
            this.gaService = gaService;
        }

        [Authorize(Roles = "consultant")]
        [Route("api/liveleads/latest")]
        [HttpGet]
        public IHttpActionResult Latest(int take = 7)
        {
            var contacts = contactRepository.All().Take(take).OrderByDescending(l => l.Date).AsNoTracking().ToList();

            var contactsData = contacts.Select(l => new ContactsDto()
            {
                id = l.Id,
                date = l.Date,
                clientName = l.Client.Name,
                type = l.LeadType,
                longitude = l.Client.Longitude.HasValue ? l.Client.Longitude.Value : 18.06491m,
                latitude = l.Client.Latitude.HasValue ? l.Client.Latitude.Value : 59.32893000000001m
            });

            var converter = new IsoDateTimeConverter();
            converter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss G\\MT";
            return Ok(contactsData);
        }

        //
        // GET: /LiveLeads/Latest
        [Authorize(Roles = "consultant")]
        [Route("api/liveleads/contactsthisweek")]
        [HttpGet]
        public IHttpActionResult ContactsThisWeek()
        {
            var now = DateTime.Now.Date;

            var firstDAyOfWeek = now;
            DateTime tommorw = now.AddDays(1);

            while (firstDAyOfWeek.DayOfWeek != DayOfWeek.Monday)
            {
                firstDAyOfWeek = firstDAyOfWeek.AddDays(-1);
            }

            var numContacts = contactRepository.Where(l =>
                firstDAyOfWeek <= l.Date &&
                l.Date < tommorw).Count();

            var contactForWeek = new ContactsForWeek();
            contactForWeek.numContacts = numContacts;

            return Ok(contactForWeek);
        }

        //
        // GET: /LiveLeads/Latest
        [Authorize(Roles = "consultant")]
        [Route("api/liveleads/contactsalltime")]
        [HttpGet]
        public IHttpActionResult ContactsAllTime()
        {
            int totalCount = GetTotalCount();
            return Ok(new { numContacts = totalCount });
        }

        [Route("api/liveleads/iscake")]
        [Authorize(Roles = "consultant")]
        [HttpGet]
        public IHttpActionResult IsCake()
        {
            var totalCount = GetTotalCount();
            var isCake = totalCount >= 1000000;
            var leftToCake = Math.Max(1000000 - totalCount, 0);
            return Ok(new { isCake, leftToCake });
        }

        private int GetTotalCount()
        {
            int totalCount = contactRepository.All().Count();
            return totalCount;
        }

        [Authorize(Roles = "consultant")]
        [Route("api/liveleads/loggedinthisweek")]
        [HttpGet]
        public IHttpActionResult LoggedInClientsThisWeek()
        {
            var now = DateTime.Now.Date;
            var firstDAyOfWeek = now;
            DateTime tommorw = now.AddDays(1);
            while (firstDAyOfWeek.DayOfWeek != DayOfWeek.Monday)
            {
                firstDAyOfWeek = firstDAyOfWeek.AddDays(-1);
            }

            var query = new AnalyticQuery
            {
                TabelId = "ga:77407066",
                StartDate = firstDAyOfWeek,
                EndDate = now,
            };

            query.Metrics.AddMany("ga:visitors");
            query.MaxResults = 500;
            query.Filters.AddMany("ga:customVarValue3==client");

            var result = gaService.Get<NumLoginsThisWeek>(query);

            if (result.Count() > 0)
            {
                var numLoginsThisWeek = result.First().Visitors;
                return Ok(new { numLogins = numLoginsThisWeek });
            }
            else
            {
                return Ok(new { numLogins = 0 });
            }
        }
    }
    public class ContactsDto
    {
        public int id { get; set; }
        public DateTime date { get; set; }
        public String clientName { get; set; }
        public String type { get; set; }
        public decimal longitude { get; set; }
        public decimal latitude { get; set; }
    }

    public class ContactsForWeek
    {
        public int numContacts { get; set; }
        public int numLeads { get; set; }
    }



    public class NumLoginsThisWeek
    {
        [AnalyticsMapping(Source = AnalyticsDataSource.Metric, Name = "ga:visitors")]
        public string Visitors { get; set; }
    }
}
