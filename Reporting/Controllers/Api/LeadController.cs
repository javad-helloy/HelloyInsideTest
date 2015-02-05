using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Inside.membership;
using Inside.Time;
using InsideModel.Models;
using InsideModel.repositories;
using InsideReporting.Controllers.Api;
using InsideReporting.Helpers;
using Task = InsideModel.Models.Task;

namespace InsideReporting.Controllers
{
    
    public class LeadController : ApiAuthenticationController
    {
        private IRepository<Contact> contactRepository;
        private readonly IServerTime serverTime;

        public LeadController(IRepository<Contact> contactRepository, IIdentityMembershipProvider userManager, IServerTime serverTime
        ) : base(userManager)
        {
            this.contactRepository = contactRepository;
            this.serverTime = serverTime;
        }

        [Route("api/client/{clientId}/contact/{contactId}")]
        [AuthorizeClientAPIAccess]
        public IHttpActionResult GetLead(int clientId, int contactId)
        {

            if (!contactRepository.Any(l => l.Id == contactId && l.ClientId == clientId))
            {
                return NotFound();
            }

            var lead = contactRepository.Where(l => l.Id == contactId && l.ClientId == clientId).First();
            return Ok(lead);
        }

        [Route("api/client/{clientId}/contact/", Name = "ListContacts")]
        [AuthorizeClientAPIAccess]
        public object GetContacts(int clientId, int page = 1, string type = null, string labels = null, string product = null, bool filterNew = false, string rating = null, string status = null, string tagName = null)
        {
            int pageSize = 20;

            if (page < 1)
            {
                return BadRequest("Page moste be large or equal to 1");
            }

            var contacts = contactRepository.Where(c => c.ClientId == clientId)
                .AsNoTracking()
                .Include(l => l.Interaction)
                .Include(l => l.Property)
                .Include(l=>l.Tags);

            if (filterNew)
            {
                contacts = contacts.Where(l => !l.Interaction.Any() && l.RatingScore==null && l.ReadStatus==null);
            }

            if (!string.IsNullOrEmpty(product))
            {
                contacts = contacts.Where(c => c.Product == product);
            }

            if (!string.IsNullOrEmpty(type) && type.ToLower().Contains("phone"))
            {
                contacts = contacts.Where(c => c.LeadType == "Phone");
            }

            if (!string.IsNullOrEmpty(type) && type.ToLower().Contains("chat"))
            {
                contacts = contacts.Where(c => c.LeadType == "Chat");
            }

            if (!string.IsNullOrEmpty(type) && type.ToLower().Contains("email"))
            {
                contacts = contacts.Where(c => c.LeadType == "Email");
            }

            if (!string.IsNullOrEmpty(type) && type.ToLower().Contains("event"))
            {
                contacts = contacts.Where(c => c.LeadType == "Event");
            }

            if (!string.IsNullOrEmpty(rating))
            {
                contacts = contacts.Where(c => c.Interaction.Any(p => p.Type == "Rating" && p.Value == rating));
            }

            if (!string.IsNullOrEmpty(status))
            {
                contacts = contacts.Where(c => c.Interaction.Any(p => p.Type == "Status" && p.Value == status));
            }

            if (!string.IsNullOrEmpty(tagName))
            {
                contacts = contacts.Where(c => c.Tags.Any(t => t.Name == tagName));
            }

            return GetPageResult(contacts.OrderByDescending(c => c.Date), page , pageSize);
        }

        [Route("api/client/{clientId}/lead/aggregateType")]
        [AuthorizeClientAPIAccess]
        public IHttpActionResult GetLeadTypeAggregateMonthly(int clientId, bool fillInMissing = true)
        {

            var monthlyAggregates = new List<MonthlyAggregateByType>();

            var typeAggregates = contactRepository.Where(l => l.ClientId == clientId)
                .AsNoTracking()
                .GroupBy(l => new { l.Date.Year, l.Date.Month, l.LeadType, l.RatingScore })
                .Select(
                    gb =>
                        new
                        {
                            Year = gb.Key.Year,
                            Month = gb.Key.Month,
                            Type = gb.Key.LeadType,
                            Score = gb.Key.RatingScore,
                            Count = gb.Count(),

                        });

            foreach (var typeAggregate in typeAggregates)
            {
                var hasAggregateForMonth =
                    monthlyAggregates.Any(ma => ma.Year == typeAggregate.Year && ma.Month == typeAggregate.Month);

                if (!hasAggregateForMonth)
                {
                    monthlyAggregates.Add(new MonthlyAggregateByType()
                    {
                        Year = typeAggregate.Year,
                        Month = typeAggregate.Month,
                    });
                }

                var aggregateToAdd = monthlyAggregates.Single(ma => ma.Year == typeAggregate.Year && ma.Month == typeAggregate.Month);

                if (typeAggregate.Type == "Phone")
                {
                    aggregateToAdd.Count.Phone += typeAggregate.Count;
                }
                else if (typeAggregate.Type == "Email")
                {
                    aggregateToAdd.Count.Email += typeAggregate.Count;
                }
                else if (typeAggregate.Type == "Chat")
                {
                    aggregateToAdd.Count.Chat += typeAggregate.Count;
                }
                else if (typeAggregate.Type == "Event")
                {
                    aggregateToAdd.Count.Event += typeAggregate.Count;
                }

                if (typeAggregate.Score == 1)
                {
                    aggregateToAdd.Scores.NumOfOnes += typeAggregate.Count;
                }
                else if (typeAggregate.Score == 2)
                {
                    aggregateToAdd.Scores.NumOfTwos += typeAggregate.Count;
                }
                else if (typeAggregate.Score == 3)
                {
                    aggregateToAdd.Scores.NumOfThrees += typeAggregate.Count;
                }
                else if (typeAggregate.Score == 4)
                {
                    aggregateToAdd.Scores.NumOfFours += typeAggregate.Count;
                }
                else if (typeAggregate.Score == 5)
                {
                    aggregateToAdd.Scores.NumOfFives += typeAggregate.Count;
                }
            }

            var endOfCurrentMonth = new DateTime(serverTime.RequestStarted.Year, serverTime.RequestStarted.Month, 1);
            endOfCurrentMonth = endOfCurrentMonth.AddMonths(1).AddDays(-1);
            if (monthlyAggregates.Any())
            {
                var minDate = monthlyAggregates.Min(ma => new DateTime(ma.Year, ma.Month, 1));

                if (fillInMissing)
                {
                    for (var missingDate = minDate;
                        missingDate <= endOfCurrentMonth;
                        missingDate = missingDate.AddMonths(1))
                    {
                        if (!monthlyAggregates.Any(ag => ag.Year == missingDate.Year && ag.Month == missingDate.Month))
                        {
                            monthlyAggregates.Add(new MonthlyAggregateByType()
                            {
                                Year = missingDate.Year,
                                Month = missingDate.Month
                            });
                        }
                    }
                }
            }
            else
            {
                monthlyAggregates.Add(new MonthlyAggregateByType()
                {
                    Year = endOfCurrentMonth.Year,
                    Month = endOfCurrentMonth.Month
                });
            }

            return Ok(monthlyAggregates.OrderByDescending(c => c.Year).ThenByDescending(c => c.Month));
        }
       
        [Route("api/client/{clientId}/lead/aggregate")]
        [AuthorizeClientAPIAccess]
        public IHttpActionResult GetLeadAggregate(int clientId, bool fillInMissing = true)
        {
            
            var contacts = contactRepository.Where(l => l.ClientId == clientId ).AsNoTracking();
            
            var contactAggregates = contacts.GroupBy(l => new {l.Date.Year, l.Date.Month})
                .Select(gb => new MonthlyAggregate
                            {
                                Year = gb.Key.Year,
                                Month = gb.Key.Month,
                                ContactCount = gb.Count(),
                                
                            });

            
            var aggregates = contactAggregates.ToList();

            var endOfCurrentMonth = new DateTime(serverTime.RequestStarted.Year, serverTime.RequestStarted.Month, 1);
            endOfCurrentMonth = endOfCurrentMonth.AddMonths(1).AddDays(-1);
            if (aggregates.Any())
            {
                var minDate = new DateTime(aggregates.Last().Year, aggregates.Last().Month, 1);
                if (fillInMissing)
                {
                    for (var missingDate = minDate; missingDate <= endOfCurrentMonth; missingDate = missingDate.AddMonths(1))
                    {
                        if (aggregates.Count(ag => ag.Year == missingDate.Year && ag.Month == missingDate.Month) == 0)
                        {
                            aggregates.Add(new MonthlyAggregate { Year = missingDate.Year, Month = missingDate.Month });
                        }
                    }
                }
            }
            else
            {
                aggregates.Add(new MonthlyAggregate { Year = endOfCurrentMonth.Year, Month = endOfCurrentMonth.Month });
            }

            var overAllAggregate = new OverAllAggregate();
            
            overAllAggregate.Aggregates = aggregates;

            return Ok(overAllAggregate);
        }


        [Authorize(Roles = "consultant, client")]
        [HttpPost]
        [AuthorizeClientAPIAccess]
        [Route("api/client/{clientId}/lead/{leadId}/interaction")]
        public HttpResponseMessage Post(int clientId, int leadId, Interaction interaction)
        {
            var isAdmin = User.IsInRole("consultant") || User.IsInRole("sales");
            if (isAdmin && interaction.Type == "ReadStatus")
            {
                return new HttpResponseMessage(HttpStatusCode.Unauthorized); 
            }


            IQueryable<Contact> matchedLeads = contactRepository.Where(l => l.Id == leadId && l.ClientId == clientId);
            if (!matchedLeads.Any())
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }

            var matchedLead = matchedLeads.First();

            switch (interaction.Type)
            {
                case "ReadStatus":
                {
                    matchedLead.ReadStatus = interaction.Value;
                    break;
                }
                case "RatingScore":
                {
                    matchedLead.RatingScore = int.Parse(interaction.Value);
                    break;
                }
            }
            contactRepository.SaveChanges();

            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        public class Interaction
        {
            public string Type { get; set; }
            public string Value { get; set; }
        }

        public class OverAllAggregate
        {
            
            public List<MonthlyAggregate> Aggregates = new List<MonthlyAggregate>();

            public int Total
            {
                get { return Aggregates.Sum(a => a.ContactCount); }
            }

        }

        public class MonthlyAggregate
        {
            public int Year { get; set; }
            public int Month { get; set; }
            public int ContactCount { get; set; }
        }
        public class Scores
        {
            public int NumOfOnes { get; set; }
            public int NumOfTwos { get; set; }
            public int NumOfThrees { get; set; }
            public int NumOfFours { get; set; }
            public int NumOfFives { get; set; }
            public decimal? AverageScore
            {
                get
                {
                    var totalScoreCount = NumOfOnes + NumOfTwos + NumOfThrees + NumOfFours + NumOfFives;
                    if (totalScoreCount == 0)
                    {
                        return null;
                    }
                    else
                    {
                        return (decimal)(NumOfOnes * 1 + NumOfTwos * 2 + NumOfThrees * 3 + NumOfFours * 4 + NumOfFives * 5) / totalScoreCount;
                    }
                }
            }
        }

        public class Count
        {
            public int Phone { get; set; }
            public int Email { get; set; }
            public int Chat { get; set; }
            public int Event { get; set; }
            public int Total
            {
                get { return Chat + Phone + Email + Event; }
            }
        }

        public class MonthlyAggregateByType
        {
            
            public int Year { get; set; }
            public int Month { get; set; }
            public Count Count = new Count();
            public Scores Scores = new Scores();

        }
    }
}
