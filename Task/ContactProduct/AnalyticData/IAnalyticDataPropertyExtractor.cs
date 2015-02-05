using System;

namespace Task.ContactProduct.AnalyticData
{
    public interface IAnalyticDataPropertyExtractor
    {
        void UpdateContact(int clientId, DateTime fromDate, DateTime toDate);
    }
}
