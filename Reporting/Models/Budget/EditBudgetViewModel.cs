namespace InsideReporting.Models.Budget
{
    public class EditBudgetViewModel
    {
        public int? id { get; set; }
        public int clientId { get; set; }
        public string month { get; set; }
        public decimal value { get; set; }
        public string comment { get; set; }
    }
}