using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Http;
using Inside.membership;
using InsideModel.Models;
using InsideModel.repositories;
using InsideReporting.Helpers;

namespace InsideReporting.Controllers.Api
{
    public class BudgetController : ApiAuthenticationController
    {
        private IRepository<Budget> budgetRepository;


        public BudgetController(IRepository<Budget> budgetRepository, IIdentityMembershipProvider userManager)
            : base(userManager)
        {
            this.budgetRepository = budgetRepository;
            
        }
        [AuthorizeClientAPIAccess]
        public IHttpActionResult GetBudget(int id)
        {
            var budget = budgetRepository.Where(l => l.Id == id).SingleOrDefault();
            if (budget == null)
            {
                return NotFound();
            }

            return Ok(budget);
        }

        
        [System.Web.Http.Route("api/client/{clientId}/budget/list")]
        [AuthorizeClientAPIAccess]
        public IHttpActionResult GetBudgets(int clientId, DateTime startDate, DateTime endDate)
        {
            
            if (startDate > endDate)
            {
                return BadRequest("Expected valid start and enddate. End date must be after start date");
            }

            var cutofDateStartDate = startDate.Date.AddDays(1 - startDate.Date.Day); 
            var cutofDateEndDate = endDate.Date.AddDays(1);
            var budgets =
                budgetRepository.Where(
                    b => b.ClientId == clientId && b.Month >= cutofDateStartDate && b.Month < cutofDateEndDate)
                    .AsNoTracking();

            return Ok(budgets);

        }
    }
}