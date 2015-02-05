using System;

namespace Inside.Analytics
{
    public class AnalyticsMappingAttribute : Attribute
    {
        public AnalyticsMappingAttribute() { }

        public AnalyticsMappingAttribute(AnalyticsDataSource source, string dataName)
        {
            Source = source;
            Name = dataName;
        }

        public AnalyticsDataSource Source { get; set; }
        public string Name { get; set; }
    }

    public enum AnalyticsDataSource
    {
        Metric,
        Dimension
    }
}
