using System;
using System.Linq;
using System.Net;
using Google;
using Helpers.test;
using Inside.Time;
using InsideModel.Models;
using InsideModel.Models.TaskStorage;
using InsideModel.repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Task.ContactProduct.AnalyticData;
using Task.ContactProduct.AnalyticDataTaskCreator;
using Task.Email.NotificationEmail;
using Task.ImportCustomEvents;
using Task.ImportSeoData;
using Task.PhoneNotification;
using Task.RemoveExpiredTokens;
using Task.SmsNotification.SmsNotificationSenderToUser;
using Task.SmsNotification.SmsNotificationUserTaskCreator;
using Task.TaskPerformer.Delegate;
using Task.UpdatePhonecalls.UpdatePhoneCalls;
using Task.UpdatePhonecalls.UpdatePhonecallsTaskCreator;

namespace BackgroundWorker.Test.TaskPerformer.Delegate
{
    [TestClass]
    public class TaskDelegateTest
    {
        [TestMethod]
        public void CanConstuct()
        {
            var taskQueueStorage = new Mock<ITaskQueueStorage>();
            var taskRepository = new Mock<IRepository<InsideModel.Models.Task>>();
            var serverTime = new Mock<IServerTime>();
            var newContactEmailSender = new Mock<INotificationEmailSender>();
            var phoneNotificationSender = new Mock<IPhoneNotificationTaskPerformer>();
            var contactProductAnlyticDataImporter = new Mock<IAddAnalyticProductDataForClient>();
            var createAnalyticDataTasksForClients = new Mock<ICreateAnalyticDataTasksForClients>();
            var customEventsImporter = new Mock<ICustomEventsImporter>();
            var seoDataImporter = new Mock<ISeoDataImporter>();
            var userEmailSender = new Mock<IUserNotificationEmailSender>();
            var importCustomEventsTaskCreator = new Mock<IImportCustomEventsTaskCreator>();
            var removeExpiredTokens = new Mock<IRemoveExpiredTokens>();
            var createUpdatePhoneCallsForClients = new Mock<ICreateUpdatePhonecallsTasksForClients>();
            var updatePhoneCalls = new Mock<IUpdatePhonecalls>();
            var smsNotificationTaskCreator = new Mock<ISmsNotificationTaskPerformer>();
            var userSmsNotificationTaskPerformer = new Mock<IUserSmsNotificationTaskPerformer>();
            var taskDelegate = new TaskDelegate(
                taskQueueStorage.Object,
                taskRepository.Object,
                serverTime.Object,
                newContactEmailSender.Object,
                phoneNotificationSender.Object,
                contactProductAnlyticDataImporter.Object,
                createAnalyticDataTasksForClients.Object,
                customEventsImporter.Object,
                seoDataImporter.Object,
                userEmailSender.Object,
                importCustomEventsTaskCreator.Object,
                removeExpiredTokens.Object,
                createUpdatePhoneCallsForClients.Object,
                updatePhoneCalls.Object,
                smsNotificationTaskCreator.Object,
                userSmsNotificationTaskPerformer.Object);
        }


        [TestMethod]
        public void PerfromNextTaskReturnsIfNoTasksAreInQueue()
        {
            var taskQueueStorage = new Mock<ITaskQueueStorage>();
            var taskRepository = new Mock<IRepository<InsideModel.Models.Task>>();
            var serverTime = new Mock<IServerTime>();
            var newContactEmailSender = new Mock<INotificationEmailSender>();
            var phoneNotificationSender = new Mock<IPhoneNotificationTaskPerformer>();
            var contactProductAnlyticDataImporter = new Mock<IAddAnalyticProductDataForClient>();
            var createAnalyticDataTasksForClients = new Mock<ICreateAnalyticDataTasksForClients>();
            var customEventsImporter = new Mock<ICustomEventsImporter>();
            var seoDataImporter = new Mock<ISeoDataImporter>();
            var userEmailSender = new Mock<IUserNotificationEmailSender>();
            var importCustomEventsTaskCreator = new Mock<IImportCustomEventsTaskCreator>();
            var removeExpiredTokens = new Mock<IRemoveExpiredTokens>();
            var createUpdatePhoneCallsForClients = new Mock<ICreateUpdatePhonecallsTasksForClients>();
            var updatePhoneCalls = new Mock<IUpdatePhonecalls>();
            var smsNotificationTaskCreator = new Mock<ISmsNotificationTaskPerformer>();
            var userSmsNotificationTaskPerformer = new Mock<IUserSmsNotificationTaskPerformer>();
            
            var taskDelegate = new TaskDelegate(
                taskQueueStorage.Object,
                taskRepository.Object,
                serverTime.Object,
                newContactEmailSender.Object,
                phoneNotificationSender.Object,
                contactProductAnlyticDataImporter.Object,
                createAnalyticDataTasksForClients.Object,
                customEventsImporter.Object,
                seoDataImporter.Object,
                userEmailSender.Object,
                importCustomEventsTaskCreator.Object,
                removeExpiredTokens.Object,
                createUpdatePhoneCallsForClients.Object,
                updatePhoneCalls.Object,
                smsNotificationTaskCreator.Object,
                userSmsNotificationTaskPerformer.Object);

            taskDelegate.PerformNextTask("1");
        }


        [TestMethod]
        public void TaskWithUnexpectedTypeReQueueQueueMessage()
        {
            DateTime earliestExecution = new DateTime(2013, 1, 15);
            DateTime now = new DateTime(2013, 1, 17);

            var taskQueueStorage = new Mock<ITaskQueueStorage>();
            var taskRepository = new LocalRepository<InsideModel.Models.Task>();
            var serverTime = new Mock<IServerTime>();
            var newContactEmailSender = new Mock<INotificationEmailSender>();
            var phoneNotificationSender = new Mock<IPhoneNotificationTaskPerformer>();
            var contactProductAnlyticDataImporter = new Mock<IAddAnalyticProductDataForClient>();
            var createAnalyticDataTasksForClients = new Mock<ICreateAnalyticDataTasksForClients>();
            var customEventsImporter = new Mock<ICustomEventsImporter>();
            var seoDataImporter = new Mock<ISeoDataImporter>();
            var userEmailSender = new Mock<IUserNotificationEmailSender>();
            var importCustomEventsTaskCreator = new Mock<IImportCustomEventsTaskCreator>();
            var removeExpiredTokens = new Mock<IRemoveExpiredTokens>();
            var createUpdatePhoneCallsForClients = new Mock<ICreateUpdatePhonecallsTasksForClients>();
            var updatePhoneCalls = new Mock<IUpdatePhonecalls>();
            var smsNotificationTaskCreator = new Mock<ISmsNotificationTaskPerformer>();
            var userSmsNotificationTaskPerformer = new Mock<IUserSmsNotificationTaskPerformer>();

            serverTime.Setup(st => st.RequestStarted).Returns(now);
            var task1 = ModelHelper.Task1AllData;
            task1.Type = "bad task type";
            task1.EarliestExecution = earliestExecution;

            taskRepository.Add(task1);

            var taskDelegate = new TaskDelegate(
                taskQueueStorage.Object,
                taskRepository,
                serverTime.Object,
                newContactEmailSender.Object,
                phoneNotificationSender.Object,
                contactProductAnlyticDataImporter.Object,
                createAnalyticDataTasksForClients.Object,
                customEventsImporter.Object,
                seoDataImporter.Object,
                userEmailSender.Object,
                importCustomEventsTaskCreator.Object,
                removeExpiredTokens.Object,
                createUpdatePhoneCallsForClients.Object,
                updatePhoneCalls.Object,
                smsNotificationTaskCreator.Object,
                userSmsNotificationTaskPerformer.Object);

            taskDelegate.PerformNextTask(task1.Id.ToString());
            Assert.AreEqual(1, taskRepository.Where(t => t.Id == task1.Id).Count());
            Assert.AreEqual(1, taskRepository.Where(t => t.Id == task1.Id).First().NumTries);
        }

        
        [TestMethod]
        public void TaskFailsItupdatesTheTaskAndDoesntRequeuTaskQueueMessage()
        {
            DateTime earliestExecution = new DateTime(2013, 1, 15);
            DateTime now = new DateTime(2013, 1, 17);

            var taskQueueStorage = new Mock<ITaskQueueStorage>();
            var taskRepository = new LocalRepository<InsideModel.Models.Task>();
            var serverTime = new Mock<IServerTime>();
            var newContactEmailSender = new Mock<INotificationEmailSender>();
            var phoneNotificationSender = new Mock<IPhoneNotificationTaskPerformer>();
            var contactProductAnlyticDataImporter = new Mock<IAddAnalyticProductDataForClient>();
            var createAnalyticDataTasksForClients = new Mock<ICreateAnalyticDataTasksForClients>();
            var customEventsImporter = new Mock<ICustomEventsImporter>();
            var seoDataImporter = new Mock<ISeoDataImporter>();

            var userEmailSender = new Mock<IUserNotificationEmailSender>();
            var importCustomEventsTaskCreator = new Mock<IImportCustomEventsTaskCreator>();
            var removeExpiredTokens = new Mock<IRemoveExpiredTokens>();
            var createUpdatePhoneCallsForClients = new Mock<ICreateUpdatePhonecallsTasksForClients>();
            var updatePhoneCalls = new Mock<IUpdatePhonecalls>();
            var smsNotificationTaskCreator = new Mock<ISmsNotificationTaskPerformer>();
            var userSmsNotificationTaskPerformer = new Mock<IUserSmsNotificationTaskPerformer>();


            serverTime.Setup(st => st.RequestStarted).Returns(now);
            serverTime.Setup(st => st.Now).Returns(now);
            var task1 = ModelHelper.Task1AllData;
            var taskId = task1.Id;
            task1.EarliestExecution = earliestExecution;

            taskRepository.Add(task1);

            TaskQueueMessage taskQueueMessage = new TaskQueueMessage()
            {
                Id = "taskqueuemessageId",
                PopReceipt = "PopReceipt",
                Message = taskId.ToString()
            };

            seoDataImporter.Setup(wr => wr.PerformTask(It.IsAny<InsideModel.Models.Task>())).Throws(new Exception("task failed"));

            var taskDelegate = new TaskDelegate(
                taskQueueStorage.Object,
                taskRepository,
                serverTime.Object,
                newContactEmailSender.Object,
                phoneNotificationSender.Object,
                contactProductAnlyticDataImporter.Object,
                createAnalyticDataTasksForClients.Object,
                customEventsImporter.Object,
                seoDataImporter.Object,
                userEmailSender.Object,
                importCustomEventsTaskCreator.Object,
                removeExpiredTokens.Object,
                createUpdatePhoneCallsForClients.Object,
                updatePhoneCalls.Object,
                smsNotificationTaskCreator.Object,
                userSmsNotificationTaskPerformer.Object);

            taskDelegate.PerformNextTask(taskQueueMessage.Message);
            
            Assert.AreEqual(1, taskRepository.Where(t => t.Id == taskId).Count());
            Assert.AreEqual(1, taskRepository.Where(t => t.Id == taskId).First().NumTries);
            Assert.AreEqual("Failed", taskRepository.Where(t => t.Id == taskId).First().Status);
            Assert.IsTrue(earliestExecution <= taskRepository.Where(t => t.Id == taskId).First().EarliestExecution);
        }

        [TestMethod]
        public void OnGoogleApiExeptionAnalyticProductRetryForbidden()
        {
            DateTime earliestExecution = new DateTime(2013, 1, 15);
            DateTime now = new DateTime(2013, 1, 17);

            var taskQueueStorage = new Mock<ITaskQueueStorage>();
            var taskRepository = new LocalRepository<InsideModel.Models.Task>();
            var serverTime = new Mock<IServerTime>();
            var newContactEmailSender = new Mock<INotificationEmailSender>();
            var phoneNotificationSender = new Mock<IPhoneNotificationTaskPerformer>();
            var contactProductAnlyticDataImporter = new Mock<IAddAnalyticProductDataForClient>();
            var createAnalyticDataTasksForClients = new Mock<ICreateAnalyticDataTasksForClients>();
            var customEventsImporter = new Mock<ICustomEventsImporter>();
            var seoDataImporter = new Mock<ISeoDataImporter>();

            var userEmailSender = new Mock<IUserNotificationEmailSender>();
            var importCustomEventsTaskCreator = new Mock<IImportCustomEventsTaskCreator>();
            var removeExpiredTokens = new Mock<IRemoveExpiredTokens>();
            var createUpdatePhoneCallsForClients = new Mock<ICreateUpdatePhonecallsTasksForClients>();
            var updatePhoneCalls = new Mock<IUpdatePhonecalls>();
            var smsNotificationTaskCreator = new Mock<ISmsNotificationTaskPerformer>();
            var userSmsNotificationTaskPerformer = new Mock<IUserSmsNotificationTaskPerformer>();


            serverTime.Setup(st => st.RequestStarted).Returns(now);
            serverTime.Setup(st => st.Now).Returns(now);
            var task1 = ModelHelper.Task1AllData;
            task1.Type = TaskType.AddProductAnalyticData;
            var taskId = task1.Id;
            task1.EarliestExecution = earliestExecution;

            taskRepository.Add(task1);

            TaskQueueMessage taskQueueMessage = new TaskQueueMessage()
            {
                Id = "taskqueuemessageId",
                PopReceipt = "PopReceipt",
                Message = taskId.ToString()
            };

            contactProductAnlyticDataImporter.Setup(wr => wr.PerformTask(It.IsAny<InsideModel.Models.Task>())).Throws(new GoogleApiException("Analytics", "task failed") { HttpStatusCode = HttpStatusCode.Forbidden });

            var taskDelegate = new TaskDelegate(
                taskQueueStorage.Object,
                taskRepository,
                serverTime.Object,
                newContactEmailSender.Object,
                phoneNotificationSender.Object,
                contactProductAnlyticDataImporter.Object,
                createAnalyticDataTasksForClients.Object,
                customEventsImporter.Object,
                seoDataImporter.Object,
                userEmailSender.Object,
                importCustomEventsTaskCreator.Object,
                removeExpiredTokens.Object,
                createUpdatePhoneCallsForClients.Object,
                updatePhoneCalls.Object,
                smsNotificationTaskCreator.Object,
                userSmsNotificationTaskPerformer.Object);

            taskDelegate.PerformNextTask(taskQueueMessage.Message);
            
            Assert.AreEqual(1, taskRepository.Where(t => t.Id == taskId).Count());
            Assert.AreEqual(1, taskRepository.Where(t => t.Id == taskId).First().NumTries);
            Assert.AreEqual("Queued", taskRepository.Where(t => t.Id == taskId).First().Status);
            taskQueueStorage.Verify(tq=>tq.ReQueue(It.Is<InsideModel.Models.Task>(t=>t.NumTries==1)), Times.Once);
        }

        
        [TestMethod]
        public void TaskSuccessSetStatusCompleteAndDeleteTaskQueueMessage()
        {
            DateTime now = new DateTime(2013, 1, 17);
            var taskQueueStorage = new Mock<ITaskQueueStorage>();
            var taskRepository = new LocalRepository<InsideModel.Models.Task>();
            var serverTime = new Mock<IServerTime>();
            var newContactEmailSender = new Mock<INotificationEmailSender>();
            var phoneNotificationSender = new Mock<IPhoneNotificationTaskPerformer>();
            var contactProductAnlyticDataImporter = new Mock<IAddAnalyticProductDataForClient>();
            var createAnalyticDataTasksForClients = new Mock<ICreateAnalyticDataTasksForClients>();
            var customEventsImporter = new Mock<ICustomEventsImporter>();
            var seoDataImporter = new Mock<ISeoDataImporter>();

            var userEmailSender = new Mock<IUserNotificationEmailSender>();
            var importCustomEventsTaskCreator = new Mock<IImportCustomEventsTaskCreator>();
            var removeExpiredTokens = new Mock<IRemoveExpiredTokens>();
            var createUpdatePhoneCallsForClients = new Mock<ICreateUpdatePhonecallsTasksForClients>();
            var updatePhoneCalls = new Mock<IUpdatePhonecalls>();
            var smsNotificationTaskCreator = new Mock<ISmsNotificationTaskPerformer>();
            var userSmsNotificationTaskPerformer = new Mock<IUserSmsNotificationTaskPerformer>();

            serverTime.Setup(st => st.RequestStarted).Returns(now);
            var task1 = ModelHelper.Task1AllData;
            var taskId = task1.Id;

            taskRepository.Add(task1);

            TaskQueueMessage taskQueueMessage = new TaskQueueMessage()
            {
                Id = "taskqueuemessageId",
                PopReceipt = "PopReceipt",
                Message = taskId.ToString()
            };
            
            var taskDelegate = new TaskDelegate(
                taskQueueStorage.Object,
                taskRepository,
                serverTime.Object,
                newContactEmailSender.Object,
                phoneNotificationSender.Object,
                contactProductAnlyticDataImporter.Object,
                createAnalyticDataTasksForClients.Object,
                customEventsImporter.Object,
                seoDataImporter.Object,
                userEmailSender.Object,
                importCustomEventsTaskCreator.Object,
                removeExpiredTokens.Object,
                createUpdatePhoneCallsForClients.Object,
                updatePhoneCalls.Object,
                smsNotificationTaskCreator.Object,
                userSmsNotificationTaskPerformer.Object);

            taskDelegate.PerformNextTask(taskQueueMessage.Message);

            Assert.AreEqual(1, taskRepository.Where(t => t.Id == taskId).Count());
            Assert.AreEqual(TaskStatus.Completed, taskRepository.Where(t => t.Id == taskId).First().Status);
        }

        [TestMethod]
        public void TaskWithMoreTriesThanMaxShouldBeSetToFailedNotPerformedAndQueueMessageDeleted()
        {
            var taskQueueStorage = new Mock<ITaskQueueStorage>();
            var taskRepository = new LocalRepository<InsideModel.Models.Task>();
            var serverTime = new Mock<IServerTime>();
            var newContactEmailSender = new Mock<INotificationEmailSender>();
            var phoneNotificationSender = new Mock<IPhoneNotificationTaskPerformer>();
            var contactProductAnlyticDataImporter = new Mock<IAddAnalyticProductDataForClient>();
            var createAnalyticDataTasksForClients = new Mock<ICreateAnalyticDataTasksForClients>();
            var customEventsImporter = new Mock<ICustomEventsImporter>();
            var seoDataImporter = new Mock<ISeoDataImporter>();
            var userEmailSender = new Mock<IUserNotificationEmailSender>();
            var importCustomEventsTaskCreator = new Mock<IImportCustomEventsTaskCreator>();
            var removeExpiredTokens = new Mock<IRemoveExpiredTokens>();
            var createUpdatePhoneCallsForClients = new Mock<ICreateUpdatePhonecallsTasksForClients>();
            var updatePhoneCalls = new Mock<IUpdatePhonecalls>();
            var smsNotificationTaskCreator = new Mock<ISmsNotificationTaskPerformer>();
            var userSmsNotificationTaskPerformer = new Mock<IUserSmsNotificationTaskPerformer>();

            var task1 = ModelHelper.Task1AllData;
            var taskId = task1.Id;
            task1.NumTries = 4;

            taskRepository.Add(task1);

            TaskQueueMessage taskQueueMessage = new TaskQueueMessage()
            {
                Id = "taskqueuemessageId",
                PopReceipt = "PopReceipt",
                Message = taskId.ToString()
            };
            
            var taskDelegate = new TaskDelegate(
                taskQueueStorage.Object,
                taskRepository,
                serverTime.Object,
                newContactEmailSender.Object,
                phoneNotificationSender.Object,
                contactProductAnlyticDataImporter.Object,
                createAnalyticDataTasksForClients.Object,
                customEventsImporter.Object,
                seoDataImporter.Object,
                userEmailSender.Object,
                importCustomEventsTaskCreator.Object,
                removeExpiredTokens.Object,
                createUpdatePhoneCallsForClients.Object,
                updatePhoneCalls.Object,
                smsNotificationTaskCreator.Object,
                userSmsNotificationTaskPerformer.Object);

            taskDelegate.PerformNextTask(taskQueueMessage.Message);

            seoDataImporter.Verify(wr => wr.PerformTask(It.IsAny<InsideModel.Models.Task>()), Times.Never);

            Assert.AreEqual(1, taskRepository.Where(t => t.Id == taskId).Count());
            Assert.AreEqual(TaskStatus.Failed, taskRepository.Where(t => t.Id == taskId).First().Status);
        }

        [TestMethod]
        public void TaskWithGreaterEarliestExecutionTimeInfutureShouldRequeue()
        {
            DateTime dateInTheFuture = new DateTime(2013, 1, 18);
            DateTime now = new DateTime(2013, 1, 17);

            var taskQueueStorage = new Mock<ITaskQueueStorage>();
            var taskRepository = new LocalRepository<InsideModel.Models.Task>();
            var serverTime = new Mock<IServerTime>();
            var newContactEmailSender = new Mock<INotificationEmailSender>();
            var phoneNotificationSender = new Mock<IPhoneNotificationTaskPerformer>();
            var contactProductAnlyticDataImporter = new Mock<IAddAnalyticProductDataForClient>();
            var createAnalyticDataTasksForClients = new Mock<ICreateAnalyticDataTasksForClients>();
            var customEventsImporter = new Mock<ICustomEventsImporter>();
            var seoDataImporter = new Mock<ISeoDataImporter>();

            var userEmailSender = new Mock<IUserNotificationEmailSender>();
            var importCustomEventsTaskCreator = new Mock<IImportCustomEventsTaskCreator>();
            var removeExpiredTokens = new Mock<IRemoveExpiredTokens>();
            var createUpdatePhoneCallsForClients = new Mock<ICreateUpdatePhonecallsTasksForClients>();
            var updatePhoneCalls = new Mock<IUpdatePhonecalls>();
            var smsNotificationTaskCreator = new Mock<ISmsNotificationTaskPerformer>();
            var userSmsNotificationTaskPerformer = new Mock<IUserSmsNotificationTaskPerformer>();

            serverTime.Setup(st => st.RequestStarted).Returns(now);

            var task1 = ModelHelper.Task1AllData;
            var taskId = task1.Id;
            task1.EarliestExecution = dateInTheFuture;

            taskRepository.Add(task1);

            TaskQueueMessage taskQueueMessage = new TaskQueueMessage()
            {
                Id = "taskqueuemessageId",
                PopReceipt = "PopReceipt",
                Message = taskId.ToString()
            };
            
            var taskDelegate = new TaskDelegate(
                taskQueueStorage.Object,
                taskRepository,
                serverTime.Object,
                newContactEmailSender.Object,
                phoneNotificationSender.Object,
                contactProductAnlyticDataImporter.Object,
                createAnalyticDataTasksForClients.Object,
                customEventsImporter.Object,
                seoDataImporter.Object,
                userEmailSender.Object,
                importCustomEventsTaskCreator.Object,
                removeExpiredTokens.Object,
                createUpdatePhoneCallsForClients.Object,
                updatePhoneCalls.Object,
                smsNotificationTaskCreator.Object,
                userSmsNotificationTaskPerformer.Object);

            taskDelegate.PerformNextTask(taskQueueMessage.Message);

            taskQueueStorage.Verify(tqs => tqs.ReQueue(It.IsAny<InsideModel.Models.Task>()));

            Assert.AreEqual(1, taskRepository.Where(t => t.Id == taskId).Count());
            Assert.AreEqual(TaskStatus.Queued, taskRepository.Where(t => t.Id == taskId).First().Status);
        }

        
        [TestMethod]
        public void NewContactEmailSenderTaskIsSentToINewContactEmailSenderAndDeletedFromQueue()
        {
            DateTime now = new DateTime(2013, 1, 17);
            var taskQueueStorage = new Mock<ITaskQueueStorage>();
            var taskRepository = new LocalRepository<InsideModel.Models.Task>();
            var serverTime = new Mock<IServerTime>();
            var newContactEmailSender = new Mock<INotificationEmailSender>();
            var phoneNotificationSender = new Mock<IPhoneNotificationTaskPerformer>();
            var contactProductAnlyticDataImporter = new Mock<IAddAnalyticProductDataForClient>();
            var createAnalyticDataTasksForClients = new Mock<ICreateAnalyticDataTasksForClients>();
            var customEventsImporter = new Mock<ICustomEventsImporter>();
            var seoDataImporter = new Mock<ISeoDataImporter>();
            var userEmailSender = new Mock<IUserNotificationEmailSender>();
            var importCustomEventsTaskCreator = new Mock<IImportCustomEventsTaskCreator>();
            var removeExpiredTokens = new Mock<IRemoveExpiredTokens>();
            var createUpdatePhoneCallsForClients = new Mock<ICreateUpdatePhonecallsTasksForClients>();
            var updatePhoneCalls = new Mock<IUpdatePhonecalls>();
            var smsNotificationTaskCreator = new Mock<ISmsNotificationTaskPerformer>();
            var userSmsNotificationTaskPerformer = new Mock<IUserSmsNotificationTaskPerformer>();

            serverTime.Setup(st => st.RequestStarted).Returns(now);

            var task1 = ModelHelper.NewContactTaskData;
            var taskId = task1.Id;
            taskRepository.Add(task1);

            var taskDelegate = new TaskDelegate(
                taskQueueStorage.Object,
                taskRepository,
                serverTime.Object,
                newContactEmailSender.Object,
                phoneNotificationSender.Object,
                contactProductAnlyticDataImporter.Object,
                createAnalyticDataTasksForClients.Object,
                customEventsImporter.Object,
                seoDataImporter.Object,
                userEmailSender.Object,
                importCustomEventsTaskCreator.Object,
                removeExpiredTokens.Object,
                createUpdatePhoneCallsForClients.Object,
                updatePhoneCalls.Object,
                smsNotificationTaskCreator.Object,
                userSmsNotificationTaskPerformer.Object);

            taskDelegate.PerformNextTask(taskId.ToString());

            newContactEmailSender.Verify(nc => nc.PerformTask(task1));
        }
    }
}
