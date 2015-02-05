using System.Collections.Generic;
using System.Collections.ObjectModel;
using InsideReporting.Controllers;

namespace InsideReporting.Models.Budget
{
    public class ClientBudgetListViewModel : SiriusReportModel
    {
        public string Error { get; set; }
        public CreateEditClientBudgetViewModel BudgetView { get; set; }
        public IList<BudgetViewModel> Budgets { get; set; }
        public bool CanMarkBudgetAsDone { get; set; }
        
        public ClientBudgetListViewModel()
        {
            Budgets = new Collection<BudgetViewModel>();
        }
    }
}