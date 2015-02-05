using System.Collections.Generic;
using System.Collections.ObjectModel;
using InsideReporting.Models.Layout;

namespace InsideReporting.Models.Budget
{
    public class BudgetListViewModel:LoggedInViewModel
    {
        public string Error { get; set; }
        public CreateEditClientBudgetViewModel BudgetView { get; set; }
        public IList<BudgetViewModel> Budgets { get; set; }
        public bool CanMarkBudgetAsDone { get; set; }

        public BudgetListViewModel(IList<string> roleList)
        {
            this.Budgets = new Collection<BudgetViewModel>();
            Roles = roleList;
            AddMenu();
        }
    }
}