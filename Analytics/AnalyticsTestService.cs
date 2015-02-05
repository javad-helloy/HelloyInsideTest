using System.Collections.Generic;
using System.Linq;
using System.Text;
using Google.GData.Analytics;

namespace Analytics
{
    public class AnalyticsTestService : IAnalyticsService
    {
        public IList<AdPerformance> GetCampaignPerformance(DataQuery partialQuery)
        {
            var data = new List<AdPerformance>();

            data.Add(new AdPerformance("Ad 1", 151512.2m,2,3.4343434m));
            data.Add(new AdPerformance("Ad 2", 5.4324m,4,2.4343344m));

            return data;
        }

        public IList<KeyValuePair<string, double>> GetKeyValueData(DataQuery partialQuery)
        {
            var data = new List<KeyValuePair<string, double>>();

            data.Add(new KeyValuePair<string, double>("jan", 1));
            data.Add(new KeyValuePair<string, double>("feb", 4));
            data.Add(new KeyValuePair<string, double>("mars", 3));

            return data;
        }
    }
}
