using System;
using System.Collections.Generic;
using System.Linq;
using Google;
using Google.Apis.Analytics.v3;
using Google.Apis.Analytics.v3.Data;
using Google.Apis.Requests;
using Inside.Analytics;
using AnalyticsService = Google.Apis.Analytics.v3.AnalyticsService;

namespace Inside.GoogleService
{
    public class GoogleAnalyticsApi : AnalyticsService, IGoogleAnalyticsApi
    {
        
        private readonly IGoogleAuthentication googleAuthentication;
        
        public GoogleAnalyticsApi(IGoogleAuthentication googleAuthentication):base()
        {
            this.googleAuthentication = googleAuthentication;
        }

        public IEnumerable<T> Get<T>(AnalyticQuery query) where T : new()
        {
            var gaData = this.Data.Ga.Get(query.TabelId, query.StartDate.ToString("yyyy-MM-dd"),
                                          query.EndDate.ToString("yyyy-MM-dd"), string.Join(",", query.Metrics));

            if (query.Dimenssions.Any())
            {
                gaData.Dimensions = string.Join(",", query.Dimenssions);    
            }

            if (query.Filters.Any())
            {
                gaData.Filters = string.Join(";", query.Filters);
            }
            
            gaData.MaxResults = query.MaxResults;
            gaData.Sort = query.Sort;
            gaData.QuotaUser = "InsideAnalyticUser";
            return MapRows<T>(gaData);
        }

        private GaData QueryAnalytics(DataResource.GaResource.GetRequest request)
        {
            
            GaData response = null;
            var authToken = googleAuthentication.GetAccessToken();
            
            request.OauthToken = authToken.AccessToken;
            response = request.Execute();
            
            return response;
        }

        private IEnumerable<T> MapRows<T>(DataResource.GaResource.GetRequest request) where T : new()
        {
            var columnPosityions = new Dictionary<Tuple<string, string>, int>();
            var gaData = QueryAnalytics(request);
            var headers = gaData.ColumnHeaders;

            int i = 0;
            foreach (var columnHeadersData in headers)
            {
                var headerNameTypeKey = new Tuple<string, string>(columnHeadersData.ColumnType, columnHeadersData.Name);
                columnPosityions[headerNameTypeKey] = i;
                i++;
            }
            if (gaData.Rows != null && gaData.Rows.Any())
            {
                return gaData.Rows.Select(row => MapRow<T>(row, columnPosityions)).ToList();
            }

            return Enumerable.Empty<T>();
        }

        private T MapRow<T>(IList<string> row, IDictionary<Tuple<string, string>, int> columnHeadersData) where T : new()
        {
            T mappedRow = new T();

            foreach (var property in typeof(T).GetProperties())
            {
                foreach (AnalyticsMappingAttribute mappingAttribute in property.GetCustomAttributes(typeof(AnalyticsMappingAttribute), true))
                {
                    string dataSource = "";

                    var name = mappingAttribute.Name;
                    var source = mappingAttribute.Source;
                    if (columnHeadersData.Keys.Any(k => k.Item1 == "DIMENSION" && k.Item2 == name) && source == AnalyticsDataSource.Dimension)
                    {
                        var columnIndex =
                            columnHeadersData.Single(h => h.Key.Item1 == "DIMENSION" && h.Key.Item2 == name).Value;

                        dataSource = row[columnIndex];
                    }
                    else if (columnHeadersData.Keys.Any(k => k.Item1 == "METRIC" && k.Item2 == name) && source == AnalyticsDataSource.Metric)
                    {
                        var columnIndex =
                            columnHeadersData.Single(h => h.Key.Item1 == "METRIC" && h.Key.Item2 == name).Value;

                        dataSource = row[columnIndex];
                    }
                    if (property.PropertyType == typeof (int?))
                        {
                            int? value;
                            if (dataSource == "")
                            {
                                value = null;
                            }
                            else
                            {
                                value = int.Parse(dataSource);    
                            }
                            property.SetValue(mappedRow, value);
                        }
                        else if (property.PropertyType == typeof(int))
                        {
                            int value = dataSource == "" ? 0 : int.Parse(dataSource);
                            property.SetValue(mappedRow, value);
                        }
                        else if (property.PropertyType == typeof(double?))
                        {
                            double? value;
                            if (dataSource == "")
                            {
                                value = null;
                            }
                            else
                            {
                                value = double.Parse(dataSource);
                            }
                            property.SetValue(mappedRow, value);
                        }
                        else if (property.PropertyType == typeof(double))
                        {
                            double value = double.Parse(dataSource);
                            property.SetValue(mappedRow, value);
                        }
                        else if (property.PropertyType == typeof (string))
                        {
                            string value = dataSource;
                            property.SetValue(mappedRow, value);
                        }
                    
                }
            }

            return mappedRow;
        }
    }


    public class AnalyticMapRow
    {
        public AnalyticMapRow()
        {
            AnalyticMapKeyValuePairs = new List<KeyValuePair<AnalyticsMappingAttribute, string>>();
        }
        public IList<KeyValuePair<AnalyticsMappingAttribute, string>> AnalyticMapKeyValuePairs { get; set; }
    }
}
