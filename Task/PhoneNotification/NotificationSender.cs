using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using InsideModel.Models;
using Newtonsoft.Json;
using RestSharp;

namespace Task.PhoneNotification
{
    public class NotificationSender : INotificationSender
    {
        private readonly IRestClient restClient;
        private readonly string pwAuthToken;
        private readonly string pwApplicationId;

        public NotificationSender()
        {
            restClient = new RestClient("https://cp.pushwoosh.com");
            pwAuthToken =  ConfigurationManager.AppSettings["PwApiKey"];
            pwApplicationId = ConfigurationManager.AppSettings["PwApplicationId"];
        }


        private string Push(string action, Notification data)
        {
            var request = new RestRequest("json/1.3/"+action, Method.POST);
            data.request.application = pwApplicationId;
            data.request.auth = pwAuthToken;

           // var json = new JObject(new JProperty("request", request.JsonSerializer.Serialize(data)));
            request.RequestFormat = DataFormat.Json;
            request.AddBody(data);
            var result = restClient.Execute(request);

            if (result.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception("Sending Phone Notification Failed("+result.StatusCode+"): "+result.StatusDescription);
            }
            var pushWooshResult = JsonConvert.DeserializeObject<PushWooshResult>(result.Content);

            if (pushWooshResult.status_code != 200)
            {
                throw new Exception("Sending Phone Notification Failed: Status Code =" + pushWooshResult.status_code + ", Description: " + pushWooshResult.status_message);
            }

            return result.Content;
        }

        public string SendNotification(int clientId, string content)
        {
            var data = new Notification();

            var clientCondition = new List<string>(){"ClientId","EQ",clientId.ToString()};
            var conditions = new List<List<string>>();
            conditions.Add(clientCondition);
            data.request.notifications.Add(new NotificationData()
            {
                content = content,
                send_date = "now",
                ios_badges = "+1",
                conditions = conditions
            });

            var result = Push("createMessage", data);
            return result;

        }

    }

    public class PushWooshResult
    {
        public int status_code { get; set; }
        public string status_message { get; set; }
        public PushWooshReposnseMessage response { get; set; }

    }

    public class PushWooshReposnseMessage
    {
        public List<string> Messages { get; set; }
    }
}