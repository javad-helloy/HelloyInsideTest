using System;
using System.Linq;
using System.Net;
using Inside.Time;
using InsideModel.Models;
using InsideModel.repositories;
using Microsoft.Ajax.Utilities;
using RestSharp;

namespace Inside.ExternalData
{
    public class ExternalDataProvider : IExternalDataProvider
    {
        private readonly IRestClient restClient;
        private readonly IRepository<Client> clientRepository;
        private readonly IServerTime serverTime;

        public ExternalDataProvider(IRestClient restClient,
                                    IRepository<Client> clientRepository,
                                    IServerTime serverTime)
        {
            this.restClient = restClient;
            this.restClient.BaseUrl = new Uri("https://api.calltrackingmetrics.com");
            this.clientRepository = clientRepository;
            this.serverTime = serverTime;
        }

        public string GetPhoneData(int callTrackingAccountId, DateTime startDate, DateTime endDate)
        {

            var url = "https://api.calltrackingmetrics.com/api/v1/accounts/" + callTrackingAccountId +
                      "/calls.json?start_date=" + startDate.ToString("yyyy-MM-dd") + "&end_date=" + endDate.ToString("yyyy-MM-dd");
            
            return GetPhoneData(url);
        }

        public string GetPhoneData(string url)
        {
            if (url.IsNullOrWhiteSpace()) return null;
            var urlWithNoBaseValue = url.Replace("https://api.calltrackingmetrics.com/", "");
            var request =
                    new RestRequest(urlWithNoBaseValue, Method.GET);
            var username = "dev@helloy.se";
            var password = "u=-Y5+rVrU5.$RQ";
            
            restClient.Authenticator = new HttpBasicAuthenticator(username, password);
            var result = restClient.Execute(request);

            if (result.StatusCode != HttpStatusCode.OK)
            {
                 throw new Exception(result.ErrorMessage);
            }

            return result.Content;
        }

        public Contact MapPhoneDataToContact(CallTrackingMetricsWebhookData callTrackingMetricsData)
        {
            var phoneCall = new Contact();
            MapPhoneDataToContact(callTrackingMetricsData, phoneCall);
            return phoneCall;
        }

        public void MapPhoneDataToContact(CallTrackingMetricsWebhookData data, Contact phoneCall)
        {
            phoneCall.LeadType = "Phone";

            phoneCall.Date = serverTime.ParseToServerTimeZoneFromStandardUser(data.called_at.Substring(0, 19), "yyyy-MM-dd hh:mm tt");

            phoneCall.ClientId = clientRepository.Where(c => c.CallTrackingMetricId == data.account_id).First().Id;

            if (!data.id.ToString().IsNullOrWhiteSpace())
            {
                phoneCall.SetPropertyValue("CTMId", data.id.ToString());
            }
            if (!data.audio.IsNullOrWhiteSpace())
            {
                phoneCall.SetPropertyValue("Audio", data.audio);
            }
            if (!data.business_number.IsNullOrWhiteSpace())
            {
                phoneCall.SetPropertyValue("BusinessNumber", data.business_number);
            }
            if (!data.caller_number.IsNullOrWhiteSpace())
            {
                phoneCall.SetPropertyValue("CallerNumber", data.caller_number);
            }
            if (!data.city.IsNullOrWhiteSpace())
            {
                phoneCall.SetPropertyValue("City", data.city);
            }
            if (!data.country.IsNullOrWhiteSpace())
            {
                phoneCall.SetPropertyValue("Country", data.country);
            }
            if (!data.location.IsNullOrWhiteSpace())
            {
                phoneCall.SetPropertyValue("LocationUrl", data.location);
            }
            if (!data.postal_code.IsNullOrWhiteSpace())
            {
                phoneCall.SetPropertyValue("PostalCode", data.postal_code);
            }
            if (!data.referrer.IsNullOrWhiteSpace())
            {
                phoneCall.SetPropertyValue("ReferalUrl", data.referrer);
            }
            if (!data.search.IsNullOrWhiteSpace())
            {
                phoneCall.SetPropertyValue("SearchPhrase", data.search);
            }
            if (!data.source.IsNullOrWhiteSpace())
            {
                phoneCall.SetPropertyValue("TrackingNumberName", data.source);
            }
            if (!data.state.IsNullOrWhiteSpace())
            {
                phoneCall.SetPropertyValue("State", data.state);
            }
            if (!data.dial_status.IsNullOrWhiteSpace())
            {
                phoneCall.SetPropertyValue("Status", data.dial_status);
            }
            if (!data.tracking_number.IsNullOrWhiteSpace())
            {
                phoneCall.SetPropertyValue("TrackingNumber", data.tracking_number);
            }
            if (!data.account_id.ToString().IsNullOrWhiteSpace())
            {
                phoneCall.SetPropertyValue("CTMAccoutId", data.account_id.ToString());
            }
            if (!data.duration.ToString().IsNullOrWhiteSpace())
            {
                phoneCall.SetPropertyValue("Duration", data.duration.ToString());
            }
            
        }
    }
}
