using System;
using System.Globalization;

namespace Inside.Time
{
    public class ServerTime : IServerTime
    {
        private DateTime started;
        private TimeZoneInfo standardReportTimeZone;
        private TimeZoneInfo serverTimeZone;

        public ServerTime()
        {
            started = DateTime.Now;
            
            standardReportTimeZone = TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard Time");
            serverTimeZone = TimeZoneInfo.FindSystemTimeZoneById("UTC");
            var current = TimeZone.CurrentTimeZone;
            
            
        }

        public DateTime RequestStarted
        {
            get { return started; }
        }

        public DateTime Now
        {
            get { return DateTime.Now; }
        }

        public DateTime ParseToServerTimeZoneFromStandardUser(string dateTime, string format)
        {
            CultureInfo specificCulture = CultureInfo.CreateSpecificCulture("en-US");
            
            var date = DateTime.ParseExact(dateTime, format, specificCulture);

            
            var utcdate = TimeZoneInfo.ConvertTime(date, standardReportTimeZone, serverTimeZone);

            return utcdate;
        }

        public DateTime ConvertUserStandardTimeToServerTime(DateTime dateTime)
        {
            return TimeZoneInfo.ConvertTime(dateTime, standardReportTimeZone, serverTimeZone);
        }

        public DateTime ConvertServerTimeToStandardUserTime(DateTime dateTime)
        {
            return TimeZoneInfo.ConvertTime(dateTime, serverTimeZone, standardReportTimeZone);
        }
    }
}