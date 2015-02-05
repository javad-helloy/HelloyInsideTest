using System;
using System.Text;
using System.Collections.Generic;
using Helpers.test;
using InsideModel.Models;
using InsideModel.repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using Task.Email.NotificationEmail;
using Task.PhoneNotification;

namespace BackgroundWorker.Test.PhoneNotification
{

    [TestClass]
    public class PhoneNotificationTaskPerformerTest
    {
        [TestMethod]
        public void CanCreate()
        {
            var phoneNotificationSender = new Mock<INotificationSender>();
            var notificationPhoneTextBuilder = new Mock<IPhoneNotificationTextBuilder>();
            var contactRepository = new Mock<IRepository<Contact>>();

            var notificationTaskPerformer = new PhoneNotificationTaskPerformer(phoneNotificationSender.Object,
                notificationPhoneTextBuilder.Object,
                contactRepository.Object);
        }

        [TestMethod]
        public void CanPerformTask()
        {
            var phoneNotificationSender = new Mock<INotificationSender>();
            var notificationPhoneTextBuilder = new Mock<IPhoneNotificationTextBuilder>();
            var contactRepository = new Mock<IRepository<Contact>>();

            var notificationTaskPerformer = new PhoneNotificationTaskPerformer(phoneNotificationSender.Object,
                notificationPhoneTextBuilder.Object,
                contactRepository.Object);

            var result = notificationTaskPerformer.CanPerformTask("SendNewContactPhoneNotification");

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void SendPhoneNotificationForOkData()
        {
            var phoneNotificationSender = new Mock<INotificationSender>();
            var notificationPhoneTextBuilder = new Mock<IPhoneNotificationTextBuilder>();
            var contactRepository = new LocalRepository<Contact>();

            var client = ModelHelper.TestClient1AllDataNoReferences;
            var generationDate = new DateTime(2013, 1, 17);
            var lableForClient = new Label()
            {
                Id = 1,
                Name = "Helloypaket"
            };
            var contactForUser = new Contact()
            {
                Id = 1
            };
            client.Labels.Add(lableForClient);
            contactForUser.Client = client;
            contactRepository.Add(contactForUser);

            var notificationText = "Du har fått ett nytt kontakt";

            notificationPhoneTextBuilder.Setup(rb => rb.GetDefinition(contactForUser.Id))
                .Returns(notificationText);

            var notificationTaskPerformer = new PhoneNotificationTaskPerformer(phoneNotificationSender.Object,
                notificationPhoneTextBuilder.Object,
                contactRepository);

            var taskMessage = "{\"ContactId\":1}";
            

            notificationTaskPerformer.PerformTask(new InsideModel.Models.Task() { Message = taskMessage, EarliestExecution = generationDate });

            notificationPhoneTextBuilder.VerifyAll();
            phoneNotificationSender.Verify(es => es.SendNotification(client.Id,notificationText), Times.Exactly(1));
        }

        [TestMethod]
        public void DontSendPhoneNotificationForNotHelloyPackages()
        {
            var phoneNotificationSender = new Mock<INotificationSender>();
            var notificationPhoneTextBuilder = new Mock<IPhoneNotificationTextBuilder>();
            var contactRepository = new LocalRepository<Contact>();

            var client = ModelHelper.TestClient1AllDataNoReferences;
            var generationDate = new DateTime(2013, 1, 17);
            var lableForClient = new Label()
            {
                Id = 1,
                Name = "NotHelloypaket"
            };

            var contactForUser = new Contact()
            {
                Id = 1,
                Client = client
            };

            client.Labels.Add(lableForClient);
            contactRepository.Add(contactForUser);

            var notificationTaskPerformer = new PhoneNotificationTaskPerformer(phoneNotificationSender.Object,
                notificationPhoneTextBuilder.Object,
                contactRepository);

            var taskMessage = "{\"ContactId\":1}";

            notificationTaskPerformer.PerformTask(new InsideModel.Models.Task() { Message = taskMessage, EarliestExecution = generationDate });

            phoneNotificationSender.Verify(es => es.SendNotification(client.Id, It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void ThrowExceptionWithNoContact()
        {
            var phoneNotificationSender = new Mock<INotificationSender>();
            var notificationPhoneTextBuilder = new Mock<IPhoneNotificationTextBuilder>();
            var contactRepository = new LocalRepository<Contact>();

            var client = ModelHelper.TestClient1AllDataNoReferences;
            var generationDate = new DateTime(2013, 1, 17);
            var lableForClient = new Label()
            {
                Id = 1,
                Name = "NotHelloypaket"
            };

            client.Labels.Add(lableForClient);

            var notificationTaskPerformer = new PhoneNotificationTaskPerformer(phoneNotificationSender.Object,
               notificationPhoneTextBuilder.Object,
               contactRepository);

            var taskMessage = "{\"ContactId\":1}";
            notificationTaskPerformer.PerformTask(new InsideModel.Models.Task() { Message = taskMessage, EarliestExecution = generationDate });
        }

        [TestMethod]
        public void ThrowExceptionWithInActiveClient()
        {
            var phoneNotificationSender = new Mock<INotificationSender>();
            var notificationPhoneTextBuilder = new Mock<IPhoneNotificationTextBuilder>();
            var contactRepository = new LocalRepository<Contact>();

            var client = ModelHelper.TestClient1AllDataNoReferences;
            client.IsActive = false;
            var generationDate = new DateTime(2013, 1, 17);

            var contactForUser = new Contact()
            {
                Id = 1,
                Client = client
            };

            contactRepository.Add(contactForUser);
            var notificationTaskPerformer = new PhoneNotificationTaskPerformer(phoneNotificationSender.Object,
               notificationPhoneTextBuilder.Object,
               contactRepository);

            var taskMessage = "{\"ContactId\":1}";
            notificationTaskPerformer.PerformTask(new InsideModel.Models.Task() { Message = taskMessage, EarliestExecution = generationDate });
        }
    }
}
