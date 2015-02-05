using System.Collections.Generic;
using Google.GData.Analytics;

namespace Analytics
{
    public interface IAnalyticsService
    {
        IList<AdPerformance> GetCampaignPerformance(DataQuery partialQuery);
        IList<KeyValuePair<string,double>> GetKeyValueData(DataQuery partialQuery);
    }
}
