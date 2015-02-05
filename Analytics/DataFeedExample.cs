// Copyright 2010 Google Inc. All Rights Reserved.

/* Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Text;
using Google.Analytics;
using Google.GData.Analytics;
using Google.GData.Client;
using Google.GData.Extensions;


namespace Analytics
{
    public class DataFeedExample
    {
        private const String CLIENT_USERNAME = "helloy.analytics.test@gmail.com";
        private const String CLIENT_PASS = "greenkey";
        private const String TABLE_ID = "ga:46680574";

        public DataFeed feed;
        public DataFeed feed2;

        public string GetData()
        {
            StringBuilder sb = new StringBuilder();

            DataFeedExample example;

            try
            {
                example = new DataFeedExample();
            }
            catch (AuthenticationException e)
            {
                return "Authentication failed : " + e.Message;
            }
            catch (Google.GData.Client.GDataRequestException e)
            {
                return "Authentication failed : " + e.Message;
            }

            example.printFeedData(sb);
            example.printFeedDataSources(sb);
            example.printFeedAggregates(sb);
            example.printSegmentInfo(sb);
            example.printDataForOneEntry(sb);

            //Console.WriteLine(example.getEntriesAsTable());

            sb.AppendLine(example.getEntriesAsTable());
            return sb.ToString();
        }

        /**
         * Creates a new service object, attempts to authorize using the Client Login
         * authorization mechanism and requests data from the Google Analytics API.
         */
        public DataFeedExample()
        {

            // Configure GA API.
            Google.GData.Analytics.AnalyticsService asv = new Google.GData.Analytics.AnalyticsService("gaExportAPI_acctSample_v2.0");

            // Client Login Authorization.
            asv.setUserCredentials(CLIENT_USERNAME, CLIENT_PASS);

            // GA Data Feed query uri.
            String baseUrl = "https://www.google.com/analytics/feeds/data";

            DataQuery query = new DataQuery(baseUrl);
            query.Ids = TABLE_ID;
            query.Dimensions = "ga:source";
            query.Metrics = "ga:visits";
            query.Segment = "";
            query.Filters = "";
            query.Sort = "-ga:visits";
            query.NumberToRetrieve = 5;
            query.GAStartDate = "2012-01-01";
            query.GAEndDate = "2012-01-16";
            Uri url = query.Uri;
            Console.WriteLine("URL: " + url.ToString());


            // Send our request to the Analytics API and wait for the results to
            // come back.

            feed = asv.Query(query);
        }

        /**
         * Prints the important Google Analytics relates data in the Data Feed.
         */
        public void printFeedData(StringBuilder sb)
        {
            sb.AppendLine("\n-------- Important Feed Information --------");
            sb.AppendLine(
              "\nFeed Title      = " + feed.Title.Text +
              "\nFeed ID         = " + feed.Id.Uri +
              "\nTotal Results   = " + feed.TotalResults +
              "\nSart Index      = " + feed.StartIndex +
              "\nItems Per Page  = " + feed.ItemsPerPage
              );
        }

        /**
         * Prints the important information about the data sources in the feed.
         * Note: the GA Export API currently has exactly one data source.
         */
        public void printFeedDataSources(StringBuilder sb)
        {

            DataSource gaDataSource = feed.DataSource;
            sb.AppendLine("\n-------- Data Source Information --------");
            sb.AppendLine(
              "\nTable Name      = " + gaDataSource.TableName +
              "\nTable ID        = " + gaDataSource.TableId +
              "\nWeb Property Id = " + gaDataSource.WebPropertyId +
              "\nProfile Id      = " + gaDataSource.ProfileId +
              "\nAccount Name    = " + gaDataSource.AccountName);
        }

        /**
         * Prints all the metric names and values of the aggregate data. The
         * aggregate metrics represent the sum of the requested metrics across all
         * of the entries selected by the query and not just the rows returned.
         */
        public void printFeedAggregates(StringBuilder sb)
        {
            Console.WriteLine("\n-------- Aggregate Metric Values --------");
            Aggregates aggregates = feed.Aggregates;

            foreach (Metric metric in aggregates.Metrics)
            {
                sb.AppendLine(
                  "\nMetric Name  = " + metric.Name +
                  "\nMetric Value = " + metric.Value +
                  "\nMetric Type  = " + metric.Type +
                  "\nMetric CI    = " + metric.ConfidenceInterval);
            }
        }

        /**
         * Prints segment information if the query has an advanced segment defined.
         */
        public void printSegmentInfo(StringBuilder sb)
        {
            if (feed.Segments.Count > 0)
            {
                sb.AppendLine("\n-------- Advanced Segments Information --------");
                foreach (Segment segment in feed.Segments)
                {
                    sb.AppendLine(
                        "\nSegment Name       = " + segment.Name +
                        "\nSegment ID         = " + segment.Id +
                        "\nSegment Definition = " + segment.Definition.Value);
                }
            }
        }

        /**
         * Prints all the important information from the first entry in the
         * data feed.
         */
        public void printDataForOneEntry(StringBuilder sb)
        {
            Console.WriteLine("\n-------- Important Entry Information --------\n");
            if (feed.Entries.Count == 0)
            {
                Console.WriteLine("No entries found");
            }
            else
            {
                DataEntry singleEntry = feed.Entries[0] as DataEntry;

                // Properties specific to all the entries returned in the feed.
                sb.AppendLine("Entry ID    = " + singleEntry.Id.Uri);
                sb.AppendLine("Entry Title = " + singleEntry.Title.Text);

                // Iterate through all the dimensions.
                foreach (Dimension dimension in singleEntry.Dimensions)
                {
                    sb.AppendLine("Dimension Name  = " + dimension.Name);
                    sb.AppendLine("Dimension Value = " + dimension.Value);
                }

                // Iterate through all the metrics.
                foreach (Metric metric in singleEntry.Metrics)
                {
                    sb.AppendLine("Metric Name  = " + metric.Name);
                    sb.AppendLine("Metric Value = " + metric.Value);
                    sb.AppendLine("Metric Type  = " + metric.Type);
                    sb.AppendLine("Metric CI    = " + metric.ConfidenceInterval);
                }
            }
        }

        /**
         * Get the data feed values in the feed as a string.
         * @return {String} This returns the contents of the feed.
         */
        public String getEntriesAsTable()
        {
            if (feed.Entries.Count == 0)
            {
                return "No entries found";
            }
            DataEntry singleEntry = feed.Entries[0] as DataEntry;

            StringBuilder feedDataLines = new StringBuilder("\n-------- All Entries In A Table --------\n");

            // Put all the dimension and metric names into an array.
            foreach (Dimension dimension in singleEntry.Dimensions)
            {
                String[] args = { dimension.Name, dimension.Value };
                feedDataLines.AppendLine(String.Format("\n{0} \t= {1}", args));
            }
            foreach (Metric metric in singleEntry.Metrics)
            {
                String[] args = { metric.Name, metric.Value };
                feedDataLines.AppendLine(String.Format("\n{0} \t= {1}", args));
            }

            feedDataLines.Append("\n");
            return feedDataLines.ToString();
        }
    }
}