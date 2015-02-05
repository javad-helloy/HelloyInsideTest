using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using Google.GData.Analytics;
using Google.GData.Client;

namespace Analytics
{
    public class AnalyticsService : IAnalyticsService{

        private const String CLIENT_USERNAME = "helloy.analytics.test@gmail.com";
        private const String CLIENT_PASS = "greenkey";

        public IList<AdPerformance>  GetCampaignPerformance(DataQuery partialQuery)
        {
            Google.GData.Analytics.AnalyticsService asv = new Google.GData.Analytics.AnalyticsService("gaExportAPI_acctSample_v2.0");
            asv.setUserCredentials(CLIENT_USERNAME, CLIENT_PASS);
            
            AddApiKeyToQuery(partialQuery);
            var dataFeed = asv.Query(partialQuery);

            var resultList = new List<AdPerformance>();

            foreach (DataEntry entry in dataFeed.Entries)
            {
                Dimension campaignDimension = entry.Dimensions[0];

                string adName = "Adwords - " + campaignDimension.Value;

                //"ga:adCost,ga:CPC,ga:adClicks"

                Metric costMetric = entry.Metrics[0];
                Metric cpcMetric = entry.Metrics[1];
                Metric clickMetric = entry.Metrics[2];
                
                double cost = Convert.ToDouble(costMetric.FloatValue);
                double click = Convert.ToDouble(clickMetric.FloatValue);
                double cpc = Convert.ToDouble(cpcMetric.FloatValue);

                resultList.Add(new AdPerformance(adName, (decimal)cost, (int)click,(decimal)cpc));
            }

            return resultList;
        }

        public IList<KeyValuePair<String, Double>>  GetKeyValueData(DataQuery partialQuery)
        {
            if (partialQuery.Dimensions == "ga:month,ga:year")
            {
                Func<string, string> monthTranslator = monthNumber =>
                {
                    int monthNum = Convert.ToInt32(monthNumber);
                    var date = new DateTime(1, monthNum, 1);

                    CultureInfo culture = new CultureInfo("sv-SE");

                    return Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(date.ToString("MMM", culture));
                };

                return GetData(partialQuery, monthTranslator);
            }
            else if (partialQuery.Dimensions == "ga:isMobile")
            {
                Func<string, string> mobilTranslator = isMobile =>
                {
                    if (isMobile.ToLower().Contains("y"))
                    {
                        return "Mobil";
                    }
                    else
                    {
                        return "Webb";
                    }
                    
                };

                return GetData(partialQuery, mobilTranslator);
            }
            else if (partialQuery.Dimensions == "ga:date")
            {
                Func<string, string> dateTranslator = date =>
                {
                    return date;
                };

                return GetData(partialQuery, dateTranslator);

            }
            else if (partialQuery.Dimensions == "ga:day,ga:month")
            {
                Func<string, string> monthTranslator = day =>
                                                           {
                                                               return day;
                                                           };

                return GetData(partialQuery, monthTranslator);
            }
            else if (partialQuery.Dimensions == "ga:source,ga:medium")
            {
                Func<string, string, string> sourceMediumTranslator = (source, medium) =>
                                                                         {
                                                                             if (source == "google" && medium == "cpc")
                                                                             {
                                                                                 return "Google Adwords";
                                                                             }
                                                                             else if (source == "google" && medium == "organic")
                                                                             {
                                                                                 return "Google Organic";
                                                                             }
                                                                             else if (source == "(direct)")
                                                                             {
                                                                                 return "Direkt";
                                                                             }
                                                                             else if (source.Count() > 18)
                                                                             {
                                                                                 return System.Threading.Thread.CurrentThread.
                                                                                     CurrentCulture.TextInfo.ToTitleCase
                                                                                     (source.Substring(0, 15)) + "...";
                                                                             }
                                                                             else
                                                                             {
                                                                                 return System.Threading.Thread.CurrentThread.
                                                                                     CurrentCulture.TextInfo.ToTitleCase
                                                                                     (source);
                                                                             }

                                                                         };

                return GetData2Dim(partialQuery, sourceMediumTranslator);
            }
            else if (partialQuery.Dimensions == "ga:eventAction")
            {
                Func<string, string> t = s => s;
                return GetData(partialQuery, t);
            }
            else
            {
                throw new NotImplementedException();
            }

        }

        private static IList<KeyValuePair<string, double>> GetData(DataQuery partialQuery, Func<string, string> translator)
        {
            Google.GData.Analytics.AnalyticsService asv =
                new Google.GData.Analytics.AnalyticsService("gaExportAPI_acctSample_v2.0");
            asv.setUserCredentials(CLIENT_USERNAME, CLIENT_PASS);

            AddApiKeyToQuery(partialQuery);
            var dataFeed = asv.Query(partialQuery);

            var resultList = new List<KeyValuePair<String, Double>>();
            foreach (DataEntry entry in dataFeed.Entries)
            {
                Dimension dim = entry.Dimensions[0];
                string month = translator(dim.Value);

                Metric met = entry.Metrics[0];
                int numVisitors = Convert.ToInt32(met.Value);

                resultList.Add(new KeyValuePair<String, Double>(month, numVisitors));
            }

            return resultList;
        }

        private static IList<KeyValuePair<string, double>> GetData2Dim(DataQuery partialQuery, Func<string, string, string> translator)
        {
            Google.GData.Analytics.AnalyticsService asv =
                new Google.GData.Analytics.AnalyticsService("gaExportAPI_acctSample_v2.0");
            asv.setUserCredentials(CLIENT_USERNAME, CLIENT_PASS);

            AddApiKeyToQuery(partialQuery);
            var dataFeed = asv.Query(partialQuery);

            var resultList = new List<KeyValuePair<String, Double>>();
            foreach (DataEntry entry in dataFeed.Entries)
            {
                Dimension dim1 = entry.Dimensions[0];
                Dimension dim2 = entry.Dimensions[1];
                string value = translator(dim1.Value, dim2.Value);

                Metric met = entry.Metrics[0];
                int numVisitors = Convert.ToInt32(met.Value);

                resultList.Add(new KeyValuePair<String, Double>(value, numVisitors));
            }

            return resultList;
        }

        private static void AddApiKeyToQuery(DataQuery query)
        {
            string extraParameter = HttpUtility.UrlEncode("key=AIzaSyBvvg8gtqr25l3AeM7xQqGnKAzoWhJGPP0");
            query.ExtraParameters = extraParameter;
        }
    }


    public class AdPerformance
    {
        public string Name { get; set; }
        public decimal Cost { get; set; }
        public int Clicks { get; set; }
        public decimal CPC { get; set; }

        public AdPerformance(string name, decimal cost, int clicks, decimal cpc)
        {
            Name = name;
            Cost = cost;
            Clicks = clicks;
            CPC = cpc;
        }
    }

    public class VisitorData
    {
        public VisitorData(string month, int numVisitors)
        {
            this.Month = month;
            this.NumVisitors = numVisitors;
        }

        public string Month { get; set; }
        public int NumVisitors { get; set; }
    }
}
