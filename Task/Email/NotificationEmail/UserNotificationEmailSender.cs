using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using InsideModel.Models;
using Mandrill;
using Newtonsoft.Json;
using Nustache.Core;
using Task.Email.Sender;
using Task.TaskPerformer.Performer;

namespace Task.Email.NotificationEmail
{
    public interface IUserNotificationEmailSender : ITaskPerformer{}

    public class UserNotificationEmailSender : IUserNotificationEmailSender
    {
        private readonly IEmailSender emailSender;
        private readonly INotificationEmailDefenitionBuilder notificationEmailDefinitionBuilder;
        
        public UserNotificationEmailSender(
            IEmailSender emailSender,
            INotificationEmailDefenitionBuilder notificationEmailDefinitionBuilder)
        {
            this.emailSender = emailSender;
            this.notificationEmailDefinitionBuilder = notificationEmailDefinitionBuilder;
            
        }

        public bool CanPerformTask(string taskType)
        {
            return taskType == TaskType.SendNewContactEmailNotificationToUser;
        }

        public void PerformTask(InsideModel.Models.Task taskMessage)
        {
            var sendEmailToUserTask = JsonConvert.DeserializeObject<UserSpecificNotificationTaskMessage>(taskMessage.Message);

            var defintion = notificationEmailDefinitionBuilder.GetDefinition(sendEmailToUserTask.ContactId, sendEmailToUserTask.UserId);
            if (defintion != null)
            {
                SendEmail(defintion);
            }
        }

        private void SendEmail(NotificationEmailDefintion defintion)
        {


            var emailMessage = new Mandrill.EmailMessage();
            emailMessage.from_email = defintion.ConsultantEmail;
            emailMessage.from_name = defintion.ConsultantName;
            var newNotificationHtml = "";
            var consultantHtml = "";
            try
            {
                var consultantStream =
                    Assembly.GetExecutingAssembly()
                        .GetManifestResourceStream("Task.Email.Template.ConsultantRow.html");
                consultantHtml = new StreamReader(consultantStream).ReadToEnd();
                if (defintion.Type == "Phone")
                {
                    var newContactStream =
                        Assembly.GetExecutingAssembly()
                            .GetManifestResourceStream("Task.Email.Template.NewPhoneCallNotificationTemplate.html");
                    newNotificationHtml = new StreamReader(newContactStream).ReadToEnd();
                }
                else if (defintion.Type == "Email")
                {
                    var newContactStream =
                        Assembly.GetExecutingAssembly()
                            .GetManifestResourceStream("Task.Email.Template.NewEmailNotificationTemplate.html");
                    newNotificationHtml = new StreamReader(newContactStream).ReadToEnd();
                }
                else if (defintion.Type == "Chat")
                {
                    var newContactStream =
                            Assembly.GetExecutingAssembly()
                                .GetManifestResourceStream("Task.Email.Template.NewChatNotificationTemplate.html");
                    newNotificationHtml = new StreamReader(newContactStream).ReadToEnd();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(
                    "Error when accessing embeded resources, NewContactEmail.html and ContactRow.html and ConsultantRow.html, Error Message: " +
                    e);
                throw;
            }


            List<EmailAddress> emailAddresses = new List<EmailAddress>();
            EmailAddress emailAddress = new EmailAddress();
            emailAddress.email = defintion.UserEmail;
            emailAddress.name = defintion.UserName;
            emailAddresses.Add(emailAddress);
            emailMessage.to = emailAddresses;

            emailMessage.subject = "Ny Kontakt";

            var cosnultantRow = Render.StringToString(consultantHtml, defintion);
            var htmlMessage = Render.StringToString(newNotificationHtml, defintion);

            htmlMessage = htmlMessage.Replace("[[ConsultantRow]]", cosnultantRow);

            emailMessage.html = htmlMessage;

            emailSender.Send(emailMessage);

        }
    }
}
