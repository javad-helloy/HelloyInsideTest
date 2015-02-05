using System;
using InsideModel.Models;

namespace Inside.ExternalData
{
    public interface IExternalDataProvider
    {
        string GetPhoneData(int callTrackingAccountId, DateTime startDate, DateTime endDate);
        string GetPhoneData(string pageUrl);
        Contact MapPhoneDataToContact(CallTrackingMetricsWebhookData callTrackingMetricsData);
        void MapPhoneDataToContact(CallTrackingMetricsWebhookData callTrackingMetricsData, Contact contact);
    }
}
