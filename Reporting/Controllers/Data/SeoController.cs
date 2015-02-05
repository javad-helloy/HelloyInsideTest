using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Inside;
using Inside.membership;
using InsideModel.Models;
using InsideModel.repositories;
using InsideReporting.Helpers;

namespace InsideReporting.Controllers.Data
{
    public class SeoController : AuthenticationController
    {
        private IRepository<Client> clientRepository;
        private readonly ISerpRankingRepository serpRankingRepository;
        private readonly IJsonConverter jsonConverter;

        public SeoController(
            IRepository<Client> clientRepository,
            ISerpRankingRepository serpRankingRepository,
        IJsonConverter jsonConverter, IIdentityMembershipProvider userManager)
            : base(userManager)
        {
            this.serpRankingRepository = serpRankingRepository;
        
            this.clientRepository = clientRepository;
            this.jsonConverter = jsonConverter;
        }

        //
        // GET: /Seo/GetPositions
        [AuthorizeClientAccess]
        public ActionResult GetPositions(int clientId, DateTime endDate)
        {

            var domain = clientRepository.Where(c => c.Id == clientId).Select(c => c.Domain).SingleOrDefault();

            if (string.IsNullOrEmpty(domain))
            {
                Response.StatusCode = 404;
                return Content("Could not find domain data for client.", "application/json");
            }

            var rakings = serpRankingRepository.GetRankings(domain, endDate);

            return Content(jsonConverter.Serilize(rakings), "application/json");
        }

        //
        // GET: /Seo/GetPositions
        [AuthorizeClientAccess]
        public ActionResult GetPositionsWithHistory(int clientId, DateTime startDate, DateTime endDate)
        {
            var domain = clientRepository.Where(c => c.Id == clientId).Select(c => c.Domain).SingleOrDefault();

            if (string.IsNullOrEmpty(domain))
            {
                Response.StatusCode = 404;
                return Content("Could not find domain data for client.", "application/json");
            }


            var raknings = serpRankingRepository.GetRankingsWithHistory(domain, startDate, endDate);

            return Content(jsonConverter.Serilize(raknings), "application/json");

        }
    }
}
