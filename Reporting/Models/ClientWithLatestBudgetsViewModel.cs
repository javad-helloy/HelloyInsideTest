using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using InsideReporting.Models.Layout;

namespace InsideReporting.Models
{
    public class ClientWithLatestBudgetsViewModel 
    {
        public ClientWithLatestBudgetsViewModel(InsideModel.Models.Client client)
        {
            this.ClientId = client.Id;
            this.ClientName = client.Name;
            var now = DateTime.UtcNow.Date;
            var currentMonthDate = new DateTime(now.Year, now.Month, 1);
            var nextMonthDate = currentMonthDate.AddMonths(1).Date;
            var twoMonthsLaterDate = currentMonthDate.AddMonths(2).Date;
            if (client.Consultant != null)
            {
                this.Consultant = client.Consultant.Name;    
            }

            if (client.AccountManager != null)
            {
                this.AccountManager = client.AccountManager.Name;
            }
            CurrentMothsBudget = client.Budgets.SingleOrDefault(b => b.Month >= currentMonthDate && b.Month < nextMonthDate);
            
            NextMothsBudget = client.Budgets.SingleOrDefault(b => b.Month >= nextMonthDate && b.Month < twoMonthsLaterDate);
           
            this.IsActive = client.IsActive;
            this.HasUnHandledBudgets = client.Budgets.Any(b => b.IsHandled == false);
            this.HasCampaignPlan = client.Labels.Any(l => l.Name == "Kampanjplaneras");
        }

        public int ClientId { get; set; }
        public String ClientName { get; set; }
        public String Consultant { get; set; }
        public String AccountManager { get; set; }
        public InsideModel.Models.Budget CurrentMothsBudget { get; set; }
        public InsideModel.Models.Budget NextMothsBudget { get; set; }
        public bool IsActive { get; set; }
        public bool HasUnHandledBudgets { get; set; }
        public bool HasCampaignPlan { get; set; }
        
    }

    public class ClientsWithBudgetsViewModel : LoggedInViewModel
    {
        public ClientsWithBudgetsViewModel(IList<string> roleList)
        {
            this.Roles = roleList;
            AddMenu();
            Clients = new List<ClientWithLatestBudgetsViewModel>();
            var now = DateTime.UtcNow.Date;
            var currentMonthDate = new DateTime(now.Year, now.Month, 1);
            var nextMonthDate = currentMonthDate.AddMonths(1).Date;

            var cultureInfo = new CultureInfo("sv-SE", false);
            NameOfCurrentMonth = currentMonthDate.ToString("MMM", cultureInfo);
            NameOfnextMonth = nextMonthDate.ToString("MMM", cultureInfo);
        }

        public bool HasUnhandledBudgets { get; set; }
        public string Error { get; set; }
        public string NameOfCurrentMonth { get; set; }
        public string NameOfnextMonth { get; set; }
        public IList<ClientWithLatestBudgetsViewModel> Clients { get; set; }
    }
}