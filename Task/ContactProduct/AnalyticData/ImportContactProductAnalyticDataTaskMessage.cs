using System;

namespace Task.ContactProduct.AnalyticData
{
    public class ImportContactProductAnalyticDataTaskMessage
    {
        public int ClientId { get; set; }
        public DateTime StartDate{ get; set; }
        public DateTime EndDate { get; set; }
    }
}
