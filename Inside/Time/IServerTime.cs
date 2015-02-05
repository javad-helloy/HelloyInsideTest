using System;

namespace Inside.Time
{
    public interface IServerTime
    {
        DateTime RequestStarted { get; }
        DateTime Now { get; }
        DateTime ParseToServerTimeZoneFromStandardUser(string dateTime, string format);
        
        DateTime ConvertUserStandardTimeToServerTime(DateTime dateTime);
        DateTime ConvertServerTimeToStandardUserTime(DateTime dateTime);
    }
}
