using System;
using System.Linq;
using Helpers.test;
using Inside.Time;
using InsideModel.Models;
using InsideModel.Models.TaskStorage;
using InsideModel.repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Inside.Test
{
    [TestClass]
    public class ContactServiceTest
    {
        [TestMethod]
        public void CanCreate()
        {
            var leadRepository = new Mock<IRepository<Contact>>() ;
            var taskQueueStorage = new Mock<ITaskQueueStorage>() ;
            var taskRepository = new Mock<IRepository<Task>>();
            var serverTime = new Mock<IServerTime>() ;

            var contactService = new ContactService.ContactService(leadRepository.Object,
                taskQueueStorage.Object,
                taskRepository.Object,
                serverTime.Object);
        }

        [TestMethod]
        public void CanNotifyClientsForNewContactWithEmailWithOkData()
        {
            var leadRepository = new Mock<IRepository<Contact>>();
            var taskQueueStorage = new Mock<ITaskQueueStorage>();
            var taskRepository = new LocalRepository<Task>();
            var serverTime = new Mock<IServerTime>();

            var contactToQueueAsTask = new Contact()
            {
                Id = 1
            };

            var taskExecutionDate = new DateTime(2014, 01, 01);
            serverTime.Setup(st => st.RequestStarted).Returns(taskExecutionDate);

            var contactService = new ContactService.ContactService(leadRepository.Object,
                taskQueueStorage.Object,
                taskRepository,
                serverTime.Object);

            contactService.NotifyClientsForNewContactWithEmail(contactToQueueAsTask.Id);
            
            taskQueueStorage.Verify(ts => ts.Add(It.Is<Task>(t => t.Message == "{\"ContactId\":1}" && t.Type == "CreateTaskForUsersToSendNewContactEmailNotification")), Times.Once);
        }

        [TestMethod]
        public void DontNotifyClientsWithEmailForExistingTaskInStorage()
        {
            var leadRepository = new Mock<IRepository<Contact>>();
            var taskQueueStorage = new Mock<ITaskQueueStorage>();
            var taskRepository = new LocalRepository<Task>();
            var serverTime = new Mock<IServerTime>();
            var taskExecutionDate = new DateTime(2014, 01, 01);

            var contactToQueueAsTask = new Contact()
            {
                Id = 1
            };

            var taskInRepository = new Task()
            {
                Id = 1,
                Message = "{\"ContactId\":1}",
                EarliestExecution = taskExecutionDate,
                Type = "CreateTaskForUsersToSendNewContactEmailNotification"
            };
            taskRepository.Add(taskInRepository);

           
            serverTime.Setup(st => st.RequestStarted).Returns(taskExecutionDate);

            var contactService = new ContactService.ContactService(leadRepository.Object,
                taskQueueStorage.Object,
                taskRepository,
                serverTime.Object);

            contactService.NotifyClientsForNewContactWithEmail(contactToQueueAsTask.Id);

            Assert.AreEqual(1, taskRepository.All().Count());
            Assert.AreEqual("{\"ContactId\":1}", taskRepository.All().First().Message);
            Assert.AreEqual("2014-01-01", taskRepository.All().First().EarliestExecution.ToString("yyy-MM-dd"));
            Assert.AreEqual("CreateTaskForUsersToSendNewContactEmailNotification", taskRepository.All().First().Type);

            taskQueueStorage.Verify(ts => ts.Add(It.IsAny<Task>()), Times.Never);
        }

        [TestMethod]
        public void CanNotifyClientsForNewContactWithPhoneNotificationWithOkData()
        {
            var leadRepository = new Mock<IRepository<Contact>>();
            var taskQueueStorage = new Mock<ITaskQueueStorage>();
            var taskRepository = new LocalRepository<Task>();
            var serverTime = new Mock<IServerTime>();

            var contactToQueueAsTask = new Contact()
            {
                Id = 1
            };

            var taskExecutionDate = new DateTime(2014, 01, 01);
            serverTime.Setup(st => st.RequestStarted).Returns(taskExecutionDate);

            var contactService = new ContactService.ContactService(leadRepository.Object,
                taskQueueStorage.Object,
                taskRepository,
                serverTime.Object);

            contactService.NotifyClientsForNewContactWithPhoneNotification(contactToQueueAsTask.Id);

            taskQueueStorage.Verify(ts => ts.Add(It.Is<Task>(t => t.Message == "{\"ContactId\":1}" && t.Type == "SendNewContactPhoneNotification")), Times.Once);
        }

        [TestMethod]
        public void DontNotifyClientsWithPhoneNotificationForExistingTaskInStorage()
        {
            var leadRepository = new Mock<IRepository<Contact>>();
            var taskQueueStorage = new Mock<ITaskQueueStorage>();
            var taskRepository = new LocalRepository<Task>();
            var serverTime = new Mock<IServerTime>();
            var taskExecutionDate = new DateTime(2014, 01, 01);

            var contactToQueueAsTask = new Contact()
            {
                Id = 1
            };

            var taskInRepository = new Task()
            {
                Id = 1,
                Message = "{\"ContactId\":1}",
                EarliestExecution = taskExecutionDate,
                Type = "SendNewContactPhoneNotification"
            };
            taskRepository.Add(taskInRepository);


            serverTime.Setup(st => st.RequestStarted).Returns(taskExecutionDate);

            var contactService = new ContactService.ContactService(leadRepository.Object,
                taskQueueStorage.Object,
                taskRepository,
                serverTime.Object);

            contactService.NotifyClientsForNewContactWithPhoneNotification(contactToQueueAsTask.Id);

            Assert.AreEqual(1, taskRepository.All().Count());
            Assert.AreEqual("{\"ContactId\":1}", taskRepository.All().First().Message);
            Assert.AreEqual("2014-01-01", taskRepository.All().First().EarliestExecution.ToString("yyy-MM-dd"));
            Assert.AreEqual("SendNewContactPhoneNotification", taskRepository.All().First().Type);

            taskQueueStorage.Verify(ts => ts.Add(It.IsAny<Task>()), Times.Never);
        }

        [TestMethod]
        public void CanNotifyClientsForNewContactWithSmsNotificationWithOkData()
        {
            var leadRepository = new Mock<IRepository<Contact>>();
            var taskQueueStorage = new Mock<ITaskQueueStorage>();
            var taskRepository = new LocalRepository<Task>();
            var serverTime = new Mock<IServerTime>();

            var contactToQueueAsTask = new Contact()
            {
                Id = 1
            };

            var taskExecutionDate = new DateTime(2014, 01, 01);
            serverTime.Setup(st => st.RequestStarted).Returns(taskExecutionDate);

            var contactService = new ContactService.ContactService(leadRepository.Object,
                taskQueueStorage.Object,
                taskRepository,
                serverTime.Object);

            contactService.NotifyClientsForNewContactWithSmsNotification(contactToQueueAsTask.Id);

            taskQueueStorage.Verify(ts => ts.Add(It.Is<Task>(t => t.Message == "{\"ContactId\":1}" && t.Type == "CreateTaskForUsersToSendNewContactSmsNotification")), Times.Once);
        }

        [TestMethod]
        public void DontNotifyClientsWithSmsNotificationForExistingTaskInStorage()
        {
            var leadRepository = new Mock<IRepository<Contact>>();
            var taskQueueStorage = new Mock<ITaskQueueStorage>();
            var taskRepository = new LocalRepository<Task>();
            var serverTime = new Mock<IServerTime>();
            var taskExecutionDate = new DateTime(2014, 01, 01);

            var contactToQueueAsTask = new Contact()
            {
                Id = 1
            };

            var taskInRepository = new Task()
            {
                Id = 1,
                Message = "{\"ContactId\":1}",
                EarliestExecution = taskExecutionDate,
                Type = "CreateTaskForUsersToSendNewContactSmsNotification"
            };
            taskRepository.Add(taskInRepository);


            serverTime.Setup(st => st.RequestStarted).Returns(taskExecutionDate);

            var contactService = new ContactService.ContactService(leadRepository.Object,
                taskQueueStorage.Object,
                taskRepository,
                serverTime.Object);

            contactService.NotifyClientsForNewContactWithSmsNotification(contactToQueueAsTask.Id);

            Assert.AreEqual(1, taskRepository.All().Count());
            Assert.AreEqual("{\"ContactId\":1}", taskRepository.All().First().Message);
            Assert.AreEqual("2014-01-01", taskRepository.All().First().EarliestExecution.ToString("yyy-MM-dd"));
            Assert.AreEqual("CreateTaskForUsersToSendNewContactSmsNotification", taskRepository.All().First().Type);

            taskQueueStorage.Verify(ts => ts.Add(It.IsAny<Task>()), Times.Never);
        }
    }
}
