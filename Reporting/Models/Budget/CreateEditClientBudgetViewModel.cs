using System.Collections.Generic;
using InsideReporting.Models.Layout;

namespace InsideReporting.Models.Budget
{
    public class CreateEditClientBudgetViewModel : LoggedInViewModel
    {
        public string Error { get; set; }
        public int ClientId { get; set; }
        public string ClientName { get; set; }
        public string Comment { get; set; }
        public string Month { get; set; }
        public decimal Value { get; set; }
        public int BudgetId { get; set; }
        public CreateEditClientBudgetViewModel(IList<string> roleList)
        {
            Roles = roleList;
            AddMenu();
        }

        public CreateEditClientBudgetViewModel()
        {
           
        }
    }
}