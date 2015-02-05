using System;
using System.Globalization;

namespace Inside.Analytics
{
    public static class AnalyticsApiServiceHelper
    {
        public static string DateTimeToAnalyticsApiDateString(DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd");
        }

        public static DateTime AnalyticsApiDateStringToDateTime(string analyticsApiDateString)
        {
            analyticsApiDateString = analyticsApiDateString.Replace("-", "");
            return DateTime.ParseExact(analyticsApiDateString, "yyyyMMdd", CultureInfo.InvariantCulture);
        }
    }
}
