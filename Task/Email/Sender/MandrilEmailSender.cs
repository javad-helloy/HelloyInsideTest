using System;
using System.Configuration;
using System.Linq;
using Mandrill;

namespace Task.Email.Sender
{
    public class MandrilEmailSender : IEmailSender
    {
        private MandrillApi mandrillApi;

        public MandrilEmailSender()
        {
            string apiKey = ConfigurationManager.AppSettings["MandrillApiKey"];
            if (string.IsNullOrEmpty(apiKey))
            {
                throw new ConfigurationErrorsException("Found no api key for mandrill appsettings MandrillApiKey");
            }

            mandrillApi = new MandrillApi(apiKey);
        }

        public void Send(EmailMessage emailMessage)
        {

            var mandrillResult = mandrillApi.SendMessage(emailMessage);
            var emailResults =
                mandrillResult.Where(
                    mr => mr.Status == EmailResultStatus.Rejected || mr.Status == EmailResultStatus.Invalid).ToList();

            if (emailResults.Any())
            {
                throw new Exception("Mandrill Send Email Failed: " + emailResults.First().RejectReason);
            }

        }
    }
}
