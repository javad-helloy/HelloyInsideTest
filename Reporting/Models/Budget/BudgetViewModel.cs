using System;

namespace InsideReporting.Models.Budget
{
    public class BudgetViewModel
    {
        public BudgetViewModel()
        {

        }

        public BudgetViewModel(InsideModel.Models.Budget budget)
        {
            Id = budget.Id;
            ClientId = budget.ClientId;
            Month = budget.Month;
            Value = budget.Value;
            IsHandled = budget.IsHandled;
            Comment = budget.Comment;
            Client = budget.Client;

        }

        public int Id { get; set; }
        public int ClientId { get; set; }
        public DateTime Month { get; set; }
        public decimal Value { get; set; }
        public bool IsHandled { get; set; }
        public string Comment { get; set; }

        public decimal AdSpent
        {
            get { return (this.Value )* ((decimal)1.00 - this.Client.FeePercent) ; }
        }

        public InsideModel.Models.Client Client { get; set; }

    }

}