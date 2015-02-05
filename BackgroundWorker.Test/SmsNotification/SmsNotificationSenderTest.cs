using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Helpers.test;
using Inside.Time;
using InsideModel.Models;
using InsideModel.Models.TaskStorage;
using InsideModel.repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using Task.Email.NotificationEmail;
using Task.SmsNotification.SmsNotificationUserTaskCreator;
using Task.TaskCreator;
using Task = InsideModel.Models.Task;

namespace BackgroundWorker.Test.SmsNotification
{
    [TestClass]
    public class SmsNotificationSenderTest
    {
        [TestMethod]
        public void CanConstruct()
        {
            
            var contactRepository = new Mock<IRepository<Contact>>();
            var taskCreator = new Mock<ITaskManager>();
            var serverTime = new Mock<IServerTime>();
            var taskRepository = new Mock<IRepository<InsideModel.Models.Task>>();
            var taskQueueStorage = new Mock<ITaskQueueStorage>();
            var newContactSmsSender = new SmsNotificationTaskPerformer(
                contactRepository.Object,
                taskCreator.Object,
                serverTime.Object,taskRepository.Object,
                taskQueueStorage.Object);
        }

        [TestMethod]
        public void CanPerformTaskForValidStringReturnsTrueElseFalseTest()
        {
            
            var contactRepository = new Mock<IRepository<Contact>>();
            var taskCreator = new Mock<ITaskManager>();
            var serverTime = new Mock<IServerTime>();
            var taskRepository = new Mock<IRepository<InsideModel.Models.Task>>();
            var taskQueueStorage = new Mock<ITaskQueueStorage>();
            var newContactSmsSender = new SmsNotificationTaskPerformer(
                contactRepository.Object,
                taskCreator.Object,
                serverTime.Object, taskRepository.Object,
                taskQueueStorage.Object);

            var result = newContactSmsSender.CanPerformTask("CreateTaskForUsersToSendNewContactSmsNotification");
            Assert.IsTrue(result);

            var resultFalse = newContactSmsSender.CanPerformTask("This is not a Task");
            Assert.IsFalse(resultFalse);
        }

        [TestMethod]
        public void CreateUserSmsTaskForEveryUser()
        {
            var generationDate = new DateTime(2013, 1, 17);
            var contactRepository = new LocalRepository<Contact>();
           
            var taskCreator = new Mock<ITaskManager>();
            var client = ModelHelper.TestClient1AllDataNoReferences;

            var lableForClient = new Label()
            {
                Id = 1,
                Name = "Helloypaket"
            };
            var contactForUser = new Contact()
            {
                Id = 1
            };
            client.Labels = new Collection<Label>();
            
            client.Labels.Add(lableForClient);
            contactForUser.Client = client;
            var insideUser = new InsideUser
            {
                Id = "UserId1",
                ReceiveSms = true,
                Phone = "0123456"
            };
            var insideUser2 = new InsideUser
            {
                Id = "UserId2",
                ReceiveSms = true,
                Phone = "0123456"
            };
            client.InsideUserSets = new List<InsideUser>
            {
                insideUser,insideUser2
            };
            contactRepository.Add(contactForUser);

            var serverTime = new Mock<IServerTime>();
            var taskRepository = new LocalRepository<InsideModel.Models.Task>();
            var taskQueueStorage = new Mock<ITaskQueueStorage>();
            var newContactSmsSender = new SmsNotificationTaskPerformer(
                contactRepository,
                taskCreator.Object,
                serverTime.Object, taskRepository,
                taskQueueStorage.Object);

            var now = new DateTime(2013, 01, 01);
            serverTime.Setup(st => st.RequestStarted).Returns(now);
            var taskMessage = JsonConvert.SerializeObject(new SmsNotificationTaskMessage() { ContactId = contactForUser.Id });
            
            newContactSmsSender.PerformTask(new InsideModel.Models.Task() { Message = taskMessage, EarliestExecution = generationDate });
            
            taskQueueStorage.Verify(es => es.Add(It.Is<InsideModel.Models.Task>(t => t.Message == "{\"ContactId\":1,\"UserId\":\"UserId1\"}" && t.Type == "SendNewContactSmsNotificationToUser")), Times.Exactly(1));
            taskQueueStorage.Verify(es => es.Add(It.Is<InsideModel.Models.Task>(t => t.Message == "{\"ContactId\":1,\"UserId\":\"UserId2\"}" && t.Type == "SendNewContactSmsNotificationToUser")), Times.Exactly(1));
        }

        [TestMethod]
        public void DontCreateUserSmsTaskForUserWithNoPhoneOrSmsRecieve()
        {
            var generationDate = new DateTime(2013, 1, 17);
            var contactRepository = new LocalRepository<Contact>();

            var taskCreator = new Mock<ITaskManager>();
            var client = ModelHelper.TestClient1AllDataNoReferences;

            var lableForClient = new Label()
            {
                Id = 1,
                Name = "Helloypaket"
            };
            var contactForUser = new Contact()
            {
                Id = 1
            };
            client.Labels = new Collection<Label>();

            client.Labels.Add(lableForClient);
            contactForUser.Client = client;
            var insideUserNoPhoneNumber = new InsideUser
            {
                Id = "UserId1",
                ReceiveSms = true,
            };
            var insideUserNoRecieveSms = new InsideUser
            {
                Id = "UserId2",
                ReceiveSms = false,
                Phone = "0123456"
            };
            client.InsideUserSets = new List<InsideUser>
            {
                insideUserNoPhoneNumber,insideUserNoRecieveSms
            };
            contactRepository.Add(contactForUser);

            var serverTime = new Mock<IServerTime>();
            var taskRepository = new Mock<IRepository<InsideModel.Models.Task>>();
            var taskQueueStorage = new Mock<ITaskQueueStorage>();
            var newContactSmsSender = new SmsNotificationTaskPerformer(
                contactRepository,
                taskCreator.Object,
                serverTime.Object, taskRepository.Object,
                taskQueueStorage.Object);

            var now = new DateTime(2013, 01, 01);
            serverTime.Setup(st => st.RequestStarted).Returns(now);
            var taskMessage = JsonConvert.SerializeObject(new SmsNotificationTaskMessage() { ContactId = contactForUser.Id });

            newContactSmsSender.PerformTask(new InsideModel.Models.Task() { Message = taskMessage, EarliestExecution = generationDate });

            taskRepository.Verify(es => es.Add(It.IsAny<InsideModel.Models.Task>()), Times.Never);

            taskQueueStorage.Verify(es => es.Add(It.IsAny<InsideModel.Models.Task>()), Times.Never);
        }

        [TestMethod]
        
        public void DontCreateUserEmailTaskForNoUsers()
        {
            var generationDate = new DateTime(2013, 1, 17);
            var contactRepository = new LocalRepository<Contact>();

            var taskCreator = new Mock<ITaskManager>();
            var client = ModelHelper.TestClient1AllDataNoReferences;

            client.Labels = new Collection<Label>();
            client.InsideUserSets = new List<InsideUser>();
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

            var serverTime = new Mock<IServerTime>();
            var taskRepository = new LocalRepository<InsideModel.Models.Task>();
            var taskQueueStorage = new Mock<ITaskQueueStorage>();
            var newContactSmsSender = new SmsNotificationTaskPerformer(
                contactRepository,
                taskCreator.Object,
                serverTime.Object, taskRepository,
                taskQueueStorage.Object);

            var taskMessage = JsonConvert.SerializeObject(new SmsNotificationTaskMessage() { ContactId = contactForUser.Id });

            newContactSmsSender.PerformTask(new InsideModel.Models.Task() { Message = taskMessage, EarliestExecution = generationDate });

            Assert.AreEqual(0, taskRepository.All().Count());

            taskQueueStorage.Verify(es => es.Add(It.IsAny<InsideModel.Models.Task>()), Times.Never);

        }

        [TestMethod]
        
        public void DontCreateUserEmailTaskForInActiveClient()
        {
            var generationDate = new DateTime(2013, 1, 17);
            var contactRepository = new LocalRepository<Contact>();
            var taskCreator = new Mock<ITaskManager>();
            var client = ModelHelper.TestClient1AllDataNoReferences;
            client.IsActive = false;
            client.Labels = new Collection<Label>();
            client.InsideUserSets = new List<InsideUser>();
            var lableForClient = new Label()
            {
                Id = 1,
                Name = "Helloypaket"
            };
            var contactForUser = new Contact()
            {
                Id = 1
            };

            client.InsideUserSets.Add(new InsideUser
            {
                Id = "UserId1"
            });
            

            client.Labels.Add(lableForClient);
            contactForUser.Client = client;
            contactRepository.Add(contactForUser);

            var serverTime = new Mock<IServerTime>();
            var taskRepository = new Mock<IRepository<InsideModel.Models.Task>>();
            var taskQueueStorage = new Mock<ITaskQueueStorage>();
            var newContactSmsSender = new SmsNotificationTaskPerformer(
                contactRepository,
                taskCreator.Object,
                serverTime.Object, taskRepository.Object,
                taskQueueStorage.Object);

            var taskMessage = JsonConvert.SerializeObject(new SmsNotificationTaskMessage() { ContactId = contactForUser.Id });

            newContactSmsSender.PerformTask(new InsideModel.Models.Task() { Message = taskMessage, EarliestExecution = generationDate });

            taskRepository.Verify(es => es.Add(It.IsAny<InsideModel.Models.Task>()), Times.Never);

            taskQueueStorage.Verify(es => es.Add(It.IsAny<InsideModel.Models.Task>()), Times.Never);
        }

        [TestMethod]
        public void DontCreateUserSmsTaskForClientsWithNoHelloyPaketLabel()
        {
            var generationDate = new DateTime(2013, 1, 17);

            
            var contactRepository = new LocalRepository<Contact>();
            var taskCreator = new Mock<ITaskManager>();
            var client = ModelHelper.TestClient1AllDataNoReferences;
            var lableForClient = new Label()
            {
                Id = 1,
                Name = "NotHelloypaket"
            };
            var contactForUser = new Contact()
            {
                Id = 1
            };
             client.Labels = new Collection<Label>();
            client.InsideUserSets = new List<InsideUser>();

            client.Labels.Add(lableForClient);
            contactForUser.Client = client;
            client.InsideUserSets.Add(new InsideUser
            {
                Id = "UserId1"
            });
            contactRepository.Add(contactForUser);

            var serverTime = new Mock<IServerTime>();
            var taskRepository = new Mock<IRepository<InsideModel.Models.Task>>();
            var taskQueueStorage = new Mock<ITaskQueueStorage>();
            var newContactSmsSender = new SmsNotificationTaskPerformer(
                contactRepository,
                taskCreator.Object,
                serverTime.Object, taskRepository.Object,
                taskQueueStorage.Object);

            var taskMessage = JsonConvert.SerializeObject(new SmsNotificationTaskMessage() { ContactId = contactForUser.Id });

            newContactSmsSender.PerformTask(new InsideModel.Models.Task() { Message = taskMessage, EarliestExecution = generationDate });

            taskRepository.Verify(es => es.Add(It.IsAny<InsideModel.Models.Task>()), Times.Never);

            taskQueueStorage.Verify(es => es.Add(It.IsAny<InsideModel.Models.Task>()), Times.Never);
        }

        [TestMethod]
        public void DontCreateUserSmsTaskForExistingTaskInDatabase()
        {
            var generationDate = new DateTime(2013, 1, 17);
            var contactRepository = new LocalRepository<Contact>();
           
            var taskCreator = new Mock<ITaskManager>();
            var client = ModelHelper.TestClient1AllDataNoReferences;

            var lableForClient = new Label()
            {
                Id = 1,
                Name = "Helloypaket"
            };
            var contactForUser = new Contact()
            {
                Id = 1
            };
            client.Labels = new Collection<Label>();
            
            client.Labels.Add(lableForClient);
            contactForUser.Client = client;
            var insideUser = new InsideUser
            {
                Id = "UserId1",
                ReceiveSms = true,
                Phone = "0123456"
            };
           
            client.InsideUserSets = new List<InsideUser>
            {
                insideUser
            };
            contactRepository.Add(contactForUser);

            var serverTime = new Mock<IServerTime>();
            var taskRepository = new Mock<IRepository<InsideModel.Models.Task>>();
            var taskQueueStorage = new Mock<ITaskQueueStorage>();
            var newContactEmailSender = new SmsNotificationTaskPerformer(
                contactRepository,
                taskCreator.Object,
                serverTime.Object, taskRepository.Object,
                taskQueueStorage.Object);

            var now = new DateTime(2013, 01, 01);
            serverTime.Setup(st => st.RequestStarted).Returns(now);

            var taskMessage = JsonConvert.SerializeObject(new SmsNotificationTaskMessage() { ContactId = contactForUser.Id });
            taskCreator.Setup(
                tc =>
                    tc.HasTaskInRepository("{\"ContactId\":1,\"UserId\":\"UserId1\"}",
                        TaskType.SendNewContactSmsNotificationToUser, now, now.AddDays(1))).Returns(true);

            newContactEmailSender.PerformTask(new InsideModel.Models.Task() { Message = taskMessage, EarliestExecution = generationDate });

            taskRepository.Verify(es => es.Add(It.IsAny<InsideModel.Models.Task>()), Times.Never);

            taskQueueStorage.Verify(es => es.Add(It.IsAny<InsideModel.Models.Task>()), Times.Never);
           
        }
    }
}
