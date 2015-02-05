using System;
using System.Collections.Generic;
using System.Web;
using Inside.membership;
using InsideModel.Models;
using InsideModel.repositories;
using Microsoft.AspNet.Identity;

namespace InsideReporting.Service.DemoWrapper
{
    public class AnonymisedSerpRepository : SerpRepository
    {
        private IIdentityMembershipProvider userManager;
        public AnonymisedSerpRepository(IIdentityMembershipProvider userManager)
        {
            this.userManager = userManager;
        }

        public override IList<SerpRanking> GetRankings(string domain, DateTime endDate)
        {
            var isAuthenticated = HttpContext.Current.User.Identity.IsAuthenticated;
            var isBasicAuthenticationForTaskCreating = HttpContext.Current.User.Identity.AuthenticationType == "BasicAuthenticationForTaskCreating";
            if (isAuthenticated && !isBasicAuthenticationForTaskCreating && userManager.IsInRole(HttpContext.Current.User.Identity.GetUserId(), "demo"))
            {
                var rankings = base.GetRankings(domain, endDate);

                var counter = 1;

                foreach (var serpRanking in rankings)
                {
                    serpRanking.Keyword = "Sökord " + counter;
                    serpRanking.RankingUrl = "Url " + counter++;
                }
                return rankings;
            }
            else
            {
                return base.GetRankings(domain, endDate);
            }

        }

        public override IList<SerpRankingWithHistory> GetRankingsWithHistory(string domain, DateTime startDate, DateTime endDate)
        {
            var isAuthenticated = HttpContext.Current.User.Identity.IsAuthenticated;
            var isBasicAuthenticationForTaskCreating = HttpContext.Current.User.Identity.AuthenticationType == "BasicAuthenticationForTaskCreating";
            if (isAuthenticated && !isBasicAuthenticationForTaskCreating && userManager.IsInRole(HttpContext.Current.User.Identity.GetUserId(), "demo"))
            {
                var rankings = base.GetRankingsWithHistory(domain, startDate, endDate);

                var counter = 1;

                foreach (var serpRanking in rankings)
                {
                    serpRanking.Keyword = "Sökord " + counter;
                    serpRanking.RankingUrl = "Url " + counter++;
                }

                return rankings;
            }
            else
            {
                return base.GetRankingsWithHistory(domain, startDate, endDate);
            }
        }
    }
}