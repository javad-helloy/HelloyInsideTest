using System;
using System.Globalization;

namespace Inside.ExternalData
{
    public class CallTrackingMetricsWebhookData
    {
        public int id { get; set; }
        public int account_id { get; set; }

        private string _search;
        public string search
        {
            get { return _search == "(not provided)" ? null : _search; }
            set { _search = value; }
        }
        public string referrer { get; set; }
        public string location { get; set; }
        public string source { get; set; }
        public int? duration { get; set; }
        public int? talk_time { get; set; }
        public int? ring_time { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string country { get; set; }
        public string postal_code { get; set; }
        public string called_at { get; set; }

        public DateTime? date
        {
            get
            {
                if (called_at != null)
                {
                    DateTime dateTime = DateTime.Parse(called_at, CultureInfo.InvariantCulture);
                    return dateTime;
                }
                else
                {
                    return null;
                }
            }
        }

        public string tracking_number { get; set; }
        public string business_number { get; set; }
        public string dial_status { get; set; }
        public string caller_number { get; set; }
        public string audio { get; set; }

    }
}