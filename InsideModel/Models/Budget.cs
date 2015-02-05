using System;

namespace InsideModel.Models
{
    public partial class Budget
    {
        public Budget()
        {
            
        }

        public Budget(Budget budget)
        {
            Id = budget.Id;
            ClientId = budget.ClientId;
            Month = budget.Month;
            Value = budget.Value;
            IsHandled = budget.IsHandled;
            Comment = budget.Comment;

        }

        public int Id { get; set; }
        public int ClientId { get; set; }
        public DateTime Month { get; set; }
        public decimal Value { get; set; }
        public bool IsHandled { get; set; }
        public string Comment { get; set; }	
        public virtual Client Client { get; set; }
    }
}
