using System;

namespace InsideReporting.Helpers
{
    public static partial class DateTimeExtensions
    {
        public static string ToAnalyticsApiString(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd");
        }
    }
}