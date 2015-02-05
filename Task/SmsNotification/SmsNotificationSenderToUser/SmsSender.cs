using System;
using System.Configuration;
using Twilio;

namespace Task.SmsNotification.SmsNotificationSenderToUser
{
    public class SmsSender : ISmsSender
    {
        private readonly string accountSid;
        private readonly string authToken;
        public SmsSender()
        {
            
            accountSid = ConfigurationManager.AppSettings["TwilioAccountSid"];
            authToken = ConfigurationManager.AppSettings["TwilioAuthenticationToken"];
        }

        public string SendSms(string phoneNumber, string textMessage)
        {
            var twilio = new TwilioRestClient(accountSid, authToken);
            
            var message = twilio.SendMessage("+46769438884", phoneNumber, textMessage);
            if (message.ErrorCode != null)
            {
                throw new Exception("Sending Sms failed for phone number: " + phoneNumber + " Error Code: " + message.ErrorCode);
            }

            return message.Status;
        }

    }

    
}