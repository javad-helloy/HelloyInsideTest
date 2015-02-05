using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Helpers.test;
using InsideModel.Models;
using InsideModel.repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using Task.Email.NotificationEmail;
using Task.Email.Sender;
using Task.TaskCreator;

namespace BackgroundWorker.Test.Email.Notification
{
    [TestClass]
    public class UserNotificationEmailSenderTest
    {
        [TestMethod]
        public void CanConstruct()
        {

            var emailSender = new Mock<IEmailSender>();
            var newContactDefinitionBuilder = new Mock<INotificationEmailDefenitionBuilder>();
            var newContactForUserEmailSender = new UserNotificationEmailSender(
                emailSender.Object,
                newContactDefinitionBuilder.Object);
        }

        [TestMethod]
        public void CanPerformTask()
        {

            var emailSender = new Mock<IEmailSender>();
            var newContactDefinitionBuilder = new Mock<INotificationEmailDefenitionBuilder>();
            var newContactForUserEmailSender = new UserNotificationEmailSender(
                emailSender.Object,
                newContactDefinitionBuilder.Object);

            var result = newContactForUserEmailSender.CanPerformTask("SendNewContactEmailNotificationToUser");
            Assert.IsTrue(result);

            var resultFalse = newContactForUserEmailSender.CanPerformTask("This is not a Task");
            Assert.IsFalse(resultFalse);
        }


        [TestMethod]
        public void CanSendEmailToUserForDefenition()
        {
            var generationDate = new DateTime(2013, 1, 17);
            var emailSender = new Mock<IEmailSender>();
            var newContactDefinitionBuilder = new Mock<INotificationEmailDefenitionBuilder>();
            var newContactForUserEmailSender = new UserNotificationEmailSender(
                emailSender.Object,
                newContactDefinitionBuilder.Object);

            var contactDefenition = new NotificationEmailDefintion
            {
                ConsultantEmail = "from@email.com",
                UserEmail = "first@email.com",
                InsideUrl = "First URL"
            };

            newContactDefinitionBuilder.Setup(rb => rb.GetDefinition(It.IsAny<int>(),It.IsAny<string>()))
                .Returns(contactDefenition).Verifiable();

            var newContactEmailSender = new UserNotificationEmailSender(
               emailSender.Object,
                newContactDefinitionBuilder.Object);

            var taskMessage = JsonConvert.SerializeObject(new UserSpecificNotificationTaskMessage() { ContactId = 1, UserId = "Id1"});

            newContactEmailSender.PerformTask(new InsideModel.Models.Task() { Message = taskMessage, EarliestExecution = generationDate });

            newContactDefinitionBuilder.VerifyAll();
            emailSender.Verify(es => es.Send(It.IsAny<Mandrill.EmailMessage>()), Times.Exactly(1));
        }

        [TestMethod]
        public void DontSendEmailToUserForNoDefenition()
        {
            var generationDate = new DateTime(2013, 1, 17);
            var emailSender = new Mock<IEmailSender>();
            var newContactDefinitionBuilder = new Mock<INotificationEmailDefenitionBuilder>();
            var newContactForUserEmailSender = new UserNotificationEmailSender(
                emailSender.Object,
                newContactDefinitionBuilder.Object);

            newContactDefinitionBuilder.Setup(rb => rb.GetDefinition(It.IsAny<int>(), It.IsAny<string>()))
                .Returns((NotificationEmailDefintion) null);

            var newContactEmailSender = new UserNotificationEmailSender(
               emailSender.Object,
                newContactDefinitionBuilder.Object);

            var taskMessage = JsonConvert.SerializeObject(new UserSpecificNotificationTaskMessage() { ContactId = 1, UserId = "Id1" });

            newContactEmailSender.PerformTask(new InsideModel.Models.Task() { Message = taskMessage, EarliestExecution = generationDate });

            emailSender.Verify(es => es.Send(It.IsAny<Mandrill.EmailMessage>()), Times.Never);
        }

        [TestMethod]
        public void CanGetHtmlForOneEmailWithData()
        {
            var generationDate = new DateTime(2013, 1, 17);

            var emailSender = new Mock<IEmailSender>();
            var contactRepository = new LocalRepository<Contact>();
            var newContactDefinitionBuilder = new Mock<INotificationEmailDefenitionBuilder>();
            var taskCreator = new Mock<ITaskManager>();
            var client = ModelHelper.TestClient1AllDataNoReferences;

            var user1 = ModelHelper.TestUser1AllDataNoReferences;
            client.InsideUserSets = new List<InsideUser> {user1};

            var contactProperty = new ContactProperty()
            {
                ContactId = 1,
                Type = "Duration",
                Value = "90"
            };

            var contactForUser = new Contact()
            {
                Id = 1,
                ClientId = client.Id,
                LeadType = "Phone"
            };
            contactForUser.Client = client;
            contactForUser.Property.Add(contactProperty);
            contactRepository.Add(contactForUser);

            var newContactEmailDefintion1 = new NotificationEmailDefintion();
            newContactEmailDefintion1.ConsultantEmail = "consultant@helloy.se";
            newContactEmailDefintion1.ConsultantImage = "http:path.to/image.jpg";
            newContactEmailDefintion1.ConsultantName = "Consultant Name";
            newContactEmailDefintion1.ConsultantPhone = "08654321";

            newContactEmailDefintion1.UserName = "The Company AB";
            newContactEmailDefintion1.Type = contactForUser.LeadType;
            newContactEmailDefintion1.UserEmail = "client@company.se";
            newContactEmailDefintion1.Date = "2014-01-01";
            newContactEmailDefintion1.From = "08123456";
            newContactEmailDefintion1.InsideUrl = "http://inside.helloy.se/Account/AuthenticateToken?token=Aa1Bb2Cc3Dd4Ee5%26returnUrl=%2freport%2fphone%3fclientid%3d1";
            newContactEmailDefintion1.SubjectOrDuration = "1:30";

            

            newContactDefinitionBuilder.Setup(rb => rb.GetDefinition(contactForUser.Id, user1.Id))
                .Returns(newContactEmailDefintion1);



            var newContactEmailSender = new UserNotificationEmailSender(emailSender.Object,
                newContactDefinitionBuilder.Object);

            var taskMessage = JsonConvert.SerializeObject(new UserSpecificNotificationTaskMessage() { ContactId = contactForUser.Id , UserId = user1.Id});

            emailSender.Setup(es => es.Send(It.IsAny<Mandrill.EmailMessage>())).Callback<Mandrill.EmailMessage>(em =>
            {
                var html = em.html;

                Assert.AreEqual(1, Regex.Matches(html, "nytt telefonsamtal").Count);

                Assert.AreEqual(1, Regex.Matches(html, "08123456").Count);

                Assert.AreEqual(1, em.to.Count());
                Assert.AreEqual("client@company.se", em.to.ElementAt(0).email);
                Assert.AreEqual("The Company AB", em.to.ElementAt(0).name);
                Assert.AreEqual("consultant@helloy.se", em.from_email);
                Assert.AreEqual("Consultant Name", em.from_name);

                string actualHtml = Regex.Replace(html, @"^\s+", string.Empty, RegexOptions.Multiline).TrimEnd();
                var expectedHtml = Regex.Replace(@"<html>
<meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8"">
<meta property=""og:title"" content=""*|MC:SUBJECT|*"">

<body style=""-webkit-text-size-adjust: none;margin: 0;padding: 3%;font-family: Helvetica,Arial,sans-serif;line-height: 150%;color: #444;width: 94%;"">
    <p style=""font-family: Helvetica,Arial,sans-serif;line-height: 150%;color: #444;font-size: 15px;margin: 0 0 10px;"">Helloy The Company AB!</p>
    <p class=""lighter-text"" style=""font-family: Helvetica,Arial,sans-serif;line-height: 150%;color: #444;font-size: 15px;margin: 0 0 10px;"">Du har fått ett nytt telefonsamtal från 08123456. <a href=""http://inside.helloy.se/Account/AuthenticateToken?token=Aa1Bb2Cc3Dd4Ee5%26returnUrl=%2freport%2fphone%3fclientid%3d1"" style=""color: #009dd2;"">Lyssna</a> eller 
                    <a href=""http://inside.helloy.se/Account/AuthenticateToken?token=Aa1Bb2Cc3Dd4Ee5%26returnUrl=%2freport%2fphone%3fclientid%3d1"" style=""color: #009dd2;"">Betygsätt</a> direkt!</p>
    <p class=""lighter-text"" style=""font-family: Helvetica,Arial,sans-serif;line-height: 150%;color: #444;font-size: 15px;margin: 0 0 10px;"">Datum: 2014-01-01 </p>
    <p class=""lighter-text"" style=""font-family: Helvetica,Arial,sans-serif;line-height: 150%;color: #444;font-size: 15px;margin: 0 0 10px;"">Samtalslängd: 1:30</p>
    <table style=""margin: 20px 0;"">
        <p style=""font-family: Helvetica,Arial,sans-serif;line-height: 150%;color: #444;font-size: 15px;margin: 0 0 10px;"">Med vänliga hälsningar</p>
<tr>
    <td>
        <p style=""font-family: Helvetica,Arial,sans-serif;line-height: 150%;color: #444;font-size: 15px;margin: 0 0 10px;"">Consultant Name</p>
        <p style=""font-family: Helvetica,Arial,sans-serif;line-height: 150%;color: #444;font-size: 15px;margin: 0 0 10px;""><a href=""tel:08654321"" style=""color: #009dd2;"">08654321</a></p>
        <p style=""font-family: Helvetica,Arial,sans-serif;line-height: 150%;color: #444;font-size: 15px;margin: 0 0 10px;""><a href=""mailto:consultant@helloy.se"" style=""color: #009dd2;"">consultant@helloy.se</a></p>
    </td>
    <td>
        <img class=""greyscale"" style=""width: 115px;margin-left: 50px;border: none;font-size: 14px;font-weight: bold;height: auto;line-height: 100%;outline: none;text-decoration: none;text-transform: capitalize;-webkit-filter: grayscale(1);-moz-filter: grayscale(100%);filter: gray;"" src=""http:path.to/image.jpg"">
    </td>
</tr>
    </table>
</body>
</html>", @"^\s+", string.Empty, RegexOptions.Multiline).TrimEnd();
                actualHtml = Regex.Replace(actualHtml, @"\\r\\n", "");
                expectedHtml = Regex.Replace(actualHtml, @"\\r\\n", "");
                Assert.AreEqual(expectedHtml, actualHtml);

            });

            newContactEmailSender.PerformTask(new InsideModel.Models.Task() { Message = taskMessage, EarliestExecution = generationDate });
        }

    }
}
