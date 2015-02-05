using System;
using Inside.Time;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Inside.Test.Time
{
    [TestClass]
    public class ServerTimeTest
    {
        [TestMethod]
        public void CanCreate()
        {
            var service = new ServerTime();
        }

        [TestMethod]
        public void CanParseToServerTimeZoneFromStandardUser()
        {
            var service = new ServerTime();

            var parsedTimeBeforeSaving = service.ParseToServerTimeZoneFromStandardUser("2014-02-22 01:24 PM", "yyyy-MM-dd hh:mm tt");

            Assert.AreEqual(12, parsedTimeBeforeSaving.Hour);
            Assert.AreEqual(24, parsedTimeBeforeSaving.Minute);

            var parsedTimeInSaving = service.ParseToServerTimeZoneFromStandardUser("2014-09-22 01:24 PM", "yyyy-MM-dd hh:mm tt");

            Assert.AreEqual(11, parsedTimeInSaving.Hour);
            Assert.AreEqual(24, parsedTimeInSaving.Minute);

            var parsedTimeAfterSaving = service.ParseToServerTimeZoneFromStandardUser("2014-12-22 01:24 PM", "yyyy-MM-dd hh:mm tt");

            Assert.AreEqual(12, parsedTimeAfterSaving.Hour);
            Assert.AreEqual(24, parsedTimeAfterSaving.Minute);

            var parsedTimeBreakingPOint = service.ParseToServerTimeZoneFromStandardUser("2013-10-01 10:27 AM", "yyyy-MM-dd hh:mm tt");

            Assert.AreEqual(8, parsedTimeBreakingPOint.Hour);
            Assert.AreEqual(27, parsedTimeBreakingPOint.Minute);
            
        }

        [TestMethod]
        public void CanConvertToServerTimeZoneFromStandardUser()
        {
            var service = new ServerTime();

            var dateToConvertBeforeSaving = new DateTime(2014, 02, 22, 15, 24, 00);
            var parsedTimeBeforeSaving = service.ConvertUserStandardTimeToServerTime(dateToConvertBeforeSaving);

            Assert.AreEqual(14, parsedTimeBeforeSaving.Hour);
            Assert.AreEqual(24, parsedTimeBeforeSaving.Minute);

            var dateToConvertInSaving = new DateTime(2014, 09, 22, 15, 24, 00);
            var parsedTimeInSaving = service.ConvertUserStandardTimeToServerTime(dateToConvertInSaving);
            
            Assert.AreEqual(13, parsedTimeInSaving.Hour);
            Assert.AreEqual(24, parsedTimeInSaving.Minute);

            var dateToConvertAfterSaving = new DateTime(2014, 12, 22, 15, 24, 00);
            var parsedTimeAfterSaving = service.ConvertUserStandardTimeToServerTime(dateToConvertAfterSaving);

            Assert.AreEqual(14, parsedTimeAfterSaving.Hour);
            Assert.AreEqual(24, parsedTimeAfterSaving.Minute);

            var dateToConvertBreakingPOint = new DateTime(2013, 10, 01, 10, 24, 00);
            var parsedTimeBreakingPOint = service.ConvertUserStandardTimeToServerTime(dateToConvertBreakingPOint);

            Assert.AreEqual(8, parsedTimeBreakingPOint.Hour);
            Assert.AreEqual(24, parsedTimeBreakingPOint.Minute);
            
        }

        [TestMethod]
        public void CanConvertDateTimeFromServerTimeZoneToStandardUserTimeZone()
        {
            var service = new ServerTime();

            var serverDateTimeBeforeSaving = new DateTime(2014, 02, 22, 13, 24, 00);
            var parsedTimeBeforeSaving = service.ConvertServerTimeToStandardUserTime(serverDateTimeBeforeSaving);

            Assert.AreEqual(14, parsedTimeBeforeSaving.Hour);
            Assert.AreEqual(24, parsedTimeBeforeSaving.Minute);

            var serverDateTimeInSaving = new DateTime(2014, 09, 22, 13, 24, 00);
            var parsedTimeInSaving = service.ConvertServerTimeToStandardUserTime(serverDateTimeInSaving);

            Assert.AreEqual(15, parsedTimeInSaving.Hour);
            Assert.AreEqual(24, parsedTimeInSaving.Minute);

            var serverDateTimeAfterSaving = new DateTime(2014, 12, 22, 13, 24, 00);
            var parsedTimeAfterSaving = service.ConvertServerTimeToStandardUserTime(serverDateTimeAfterSaving);

            Assert.AreEqual(14, parsedTimeAfterSaving.Hour);
            Assert.AreEqual(24, parsedTimeAfterSaving.Minute);

            var serverDateTimeBreakingPOint = new DateTime(2013, 10, 01, 10, 24, 00);
            var parsedTimeBreakingPOint = service.ConvertServerTimeToStandardUserTime(serverDateTimeBreakingPOint);

            Assert.AreEqual(12, parsedTimeBreakingPOint.Hour);
            Assert.AreEqual(24, parsedTimeBreakingPOint.Minute);

        }


    }
}
