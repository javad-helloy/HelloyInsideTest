using System;
using System.Collections.Generic;

namespace InsideModel.Models
{
    public class Notification
    {
        public Notification()
        {
            request = new request();
        }
        public request request { get; set; }

    }

    public class request
    {
        public request ()
        {
            notifications = new List<NotificationData>();
        }
        public string application { get; set; }
        public string auth { get; set; }
        public List<NotificationData> notifications { get; set; }
    }

    public class NotificationData
    {
        public NotificationData()
        {
            data = new List<NotificationInternalData>();
        }
        public string send_date { get; set; }
        public string content { get; set; }
        public List<NotificationInternalData> data { get; set; }
        public string ios_badges { get; set; }
        public List<List<string>> conditions { get; set; }

    }

    public class NotificationInternalData
    {
        public string custom { get; set; }
    }

    public class NotificationCondition
    {
        public NotificationCondition(string tagName, string tagOperator, string tagValue)
        {
            var list = new List<string>() { tagName, tagOperator, tagValue };
            conditions = new List<List<string>> {list};
        }

        public List<List<string>> conditions { get; set; }
    }

}