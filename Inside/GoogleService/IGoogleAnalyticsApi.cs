using System;
using System.Collections.Generic;
using Google.Apis.Analytics.v3;
using Google.Apis.Analytics.v3.Data;

namespace Inside.GoogleService
{
    public interface IGoogleAnalyticsApi
    {
        IEnumerable<T> Get<T>(AnalyticQuery query) where T : new();
    }

    public class AnalyticQuery
    {
        public AnalyticQuery()
        {
            Dimenssions = new List<string>();
            Metrics = new List<string>();
            Filters = new List<string>();
            MaxResults = 10000;
        }
        public string TabelId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public IList<string> Dimenssions { get;  set; }
        public IList<string> Metrics { get;  set; }
        public IList<string> Filters { get;  set; }
        public string Sort { get; set; }
        public int MaxResults { get; set; }
    }
}
