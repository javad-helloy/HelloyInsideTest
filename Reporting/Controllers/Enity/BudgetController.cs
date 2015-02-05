using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Inside.membership;
using InsideModel.Models;
using InsideModel.repositories;
using InsideReporting.Models;
using InsideReporting.Models.Budget;
using Microsoft.AspNet.Identity;

namespace InsideReporting.Controllers.Enity
{
    [Authorize(Roles = "consultant, sales")]
    public class BudgetController : AuthenticationController
    {
        private readonly IRepository<Client> clientRepository;
        private readonly IRepository<Budget> budgetRepository;
        private IIdentityMembershipProvider userManager;

        public BudgetController(
            IRepository<Client> clientRepository,
            IRepository<Budget> budgetRepository,
            IIdentityMembershipProvider userManager) : base(userManager)
        {
            this.clientRepository = clientRepository;
            this.budgetRepository = budgetRepository;
            this.userManager = userManager;
        }

        //
        // GET: /Budget/
        public ViewResult Index( string errorMessage, bool showAll = false)
        {
            var clientsWithBudget = new ClientsWithBudgetsViewModel(userManager.GetRoles(User.Identity.GetUserId()));
            var providerUserKey = User.Identity.GetUserId();
            

            if (showAll)
            {
                foreach (var client in clientRepository.Where(c=>c.IsActive).Include(c=>c.Budgets).Include(c=>c.Labels).Include(c=>c.Consultant))
                {
                    clientsWithBudget.Clients.Add(new ClientWithLatestBudgetsViewModel(client));
                }
            }
            else
            {
                foreach (var client in clientRepository.Where(c => c.IsActive && ((c.Consultant != null && c.Consultant.Id == providerUserKey) || (c.AccountManager != null && c.AccountManager.Id == providerUserKey))).Include(c => c.Budgets).Include(c => c.Labels).Include(c => c.Consultant))
                {
                    clientsWithBudget.Clients.Add(new ClientWithLatestBudgetsViewModel(client));
                }
            }

            clientsWithBudget.Clients = clientsWithBudget.Clients.OrderByDescending(c => c.HasUnHandledBudgets).ThenBy(c=>c.ClientName).ToList();
            clientsWithBudget.HasUnhandledBudgets = clientsWithBudget.Clients.Any(c => c.HasUnHandledBudgets);
            clientsWithBudget.Error = errorMessage;
            return View(clientsWithBudget);
        }

        public ActionResult ListAll(string errorMessage, bool showAll = false)
        {
            var model = new BudgetListViewModel(userManager.GetRoles(User.Identity.GetUserId()));
            IEnumerable<Budget> budgets = null;

            if (showAll)
            {
                model.Error = errorMessage;
                budgets = budgetRepository.Where(b=>b.Client.IsActive).OrderBy(b => b.IsHandled).ThenBy(b => b.Month).ToList();
            }
            else
            {
                var currentUserId = User.Identity.GetUserId();
                model.Error = errorMessage;
                budgets = budgetRepository.Where(b => b.Client.IsActive && (b.Client.AccountManagerId == currentUserId || b.Client.ConsultantId == currentUserId)).OrderBy(b => b.IsHandled).ThenBy(b => b.Month).ToList();
            }

            foreach (var budget in budgets)
            {
                var budgetViewModel = new BudgetViewModel(budget);
                model.Budgets.Add(budgetViewModel);
            }

            if (User.IsInRole("consultant"))
            {
                model.CanMarkBudgetAsDone = true;
            }
            else
            {
                model.CanMarkBudgetAsDone = false;
            }

            return View(model);
        }

        public ActionResult List(int clientId, string errorMessage, bool showAll = false)
        {
            var viewModel = new ClientBudgetListViewModel();
            if (User.IsInRole("consultant"))
            {
                viewModel.CanMarkBudgetAsDone = true;
            }
            else
            {
                viewModel.CanMarkBudgetAsDone = false;
            }

            IEnumerable<Budget> budgets = null;
            var client = clientRepository.Where(c => c.Id == clientId).First();

            viewModel.HasAdminMenu = User.IsInRole("consultant") || User.IsInRole("sales");
            viewModel.HasWebTab = client.Labels.Any(l => l.Name == "Webbflik");
            viewModel.ClientId = clientId;
            viewModel.ClientName = client.Name;

            viewModel.Error = errorMessage;
            viewModel.ClientId = (int)clientId;
            viewModel.ClientName = client.Name;
            viewModel.BudgetView = new CreateEditClientBudgetViewModel();
            viewModel.BudgetView.ClientId = (int)clientId;
            viewModel.BudgetView.ClientName = client.Name;
            budgets = client.Budgets.OrderBy(b => b.IsHandled).ThenBy(b => b.Month).ToList();

            foreach (var budget in budgets)
            {
                var budgetViewModel = new BudgetViewModel(budget);
                viewModel.Budgets.Add(budgetViewModel);
            }

            
            return View(viewModel);  
        }

        [HttpPost]
        public ActionResult Create(EditBudgetViewModel newBudget)
        {
            var budget = new Budget();
            budget.ClientId = newBudget.clientId;
            budget.Comment = newBudget.comment;
            budget.Value = newBudget.value;
            var year = int.Parse(newBudget.month.Substring(0, 4));
            var month = int.Parse(newBudget.month.Substring(5, 2));

            budget.Month = new DateTime(year, month,1);

            if (budgetRepository.Where(b => b.ClientId == budget.ClientId && b.Month == budget.Month).Any())
            {
                return RedirectToAction("List",new {clientId = newBudget.clientId, errorMessage = "Finns redan en budget för vald månad."});
            }

            budgetRepository.Add(budget);
            budgetRepository.SaveChanges();

            return RedirectToAction("List",new {clientId = newBudget.clientId});
        }

        [HttpPost]
        public ActionResult Handle(int budgetId)
        {

            var budget = budgetRepository.Where(b => b.Id == budgetId).Single();

            budget.IsHandled = true;
            budgetRepository.SaveChanges();

            return Content("Ok", "application/json");
        }

        public ActionResult Delete(int budgetId)
        {
            var budget = budgetRepository.Where(b => b.Id == budgetId).First();
            var clientId = budget.ClientId;
            budgetRepository.Delete(budget);
            budgetRepository.SaveChanges();

            return RedirectToAction("List", new { clientId = clientId});
        }

        public ActionResult Edit(int id)
        {
            var viewModel = new CreateEditClientBudgetViewModel(userManager.GetRoles(User.Identity.GetUserId()));
            var budget = budgetRepository.Where(c => c.Id == id).First();
            viewModel.ClientName = budget.Client.Name;
            viewModel.Month = budget.Month.ToString("yyyy-MM");
            viewModel.Value = Math.Round(budget.Value,2);
            viewModel.BudgetId = id;
            viewModel.ClientId = budget.ClientId;
            viewModel.Comment = budget.Comment;
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Edit(EditBudgetViewModel editBudget)
        {
            var budget = budgetRepository.Where(c => c.Id == editBudget.id).First();
            budget.Comment = editBudget.comment;
            budget.Value = editBudget.value;
            budget.IsHandled = false;
            budgetRepository.SaveChanges();

            return RedirectToAction("List", new { clientId = budget.ClientId });
        }
    }
}