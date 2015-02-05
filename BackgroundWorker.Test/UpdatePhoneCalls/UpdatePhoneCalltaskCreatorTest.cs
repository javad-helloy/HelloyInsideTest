using System;
using System.Linq;
using Helpers.test;
using Inside.Time;
using InsideModel.Models;
using InsideModel.Models.TaskStorage;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Task.ImportCustomEvents;
using Task.TaskCreator;
using Task.UpdatePhonecalls.UpdatePhonecallsTaskCreator;

namespace BackgroundWorker.Test.UpdatePhoneCalls
{
    [TestClass]
    public class UpdatePhoneCalltaskCreatorTest
    {
        [TestMethod]
        public void CanConstruct()
        {
            var clientRepository = new LocalRepository<Client>();
            var taskQueueStorage = new Mock<ITaskQueueStorage>();
            var taskRepository = new LocalRepository<InsideModel.Models.Task>();
            var serverTime = new Mock<IServerTime>();
            var taskManager = new Mock<ITaskManager>();

            var task = new CreateUpdatePhonecallsTasksForClients(
                 clientRepository,
                 taskQueueStorage.Object,
                 taskRepository,
                 serverTime.Object,
                 taskManager.Object);
        }

        [TestMethod]
        public void CanPerformTask()
        {
            var clientRepository = new LocalRepository<Client>();
            var taskQueueStorage = new Mock<ITaskQueueStorage>();
            var taskRepository = new LocalRepository<InsideModel.Models.Task>();
            var serverTime = new Mock<IServerTime>();
            var taskManager = new Mock<ITaskManager>();

            var task = new CreateUpdatePhonecallsTasksForClients(
                 clientRepository,
                 taskQueueStorage.Object,
                 taskRepository,
                 serverTime.Object,
                 taskManager.Object);

            var result = task.CanPerformTask("CreateUpdatePhonecallsTasksForClients");
            Assert.IsTrue(result);

            var result2 = task.CanPerformTask("Not Update Phonecall Task");
            Assert.IsFalse(result2);
        }

        [TestMethod]
        public void CanCreateTaskForApprpriateClients()
        {
            var clientRepository = new LocalRepository<Client>();
            var taskQueueStorage = new Mock<ITaskQueueStorage>();
            var taskRepository = new LocalRepository<InsideModel.Models.Task>();
            var serverTime = new Mock<IServerTime>();
            var taskManager = new Mock<ITaskManager>();

            var task = new CreateUpdatePhonecallsTasksForClients(
                 clientRepository,
                 taskQueueStorage.Object,
                 taskRepository,
                 serverTime.Object,
                 taskManager.Object);

            clientRepository.Add(ModelHelper.TestClient1AllDataNoReferences);
            clientRepository.Add(ModelHelper.TestClient2AllDataNoReferences);

            var taskExecutionDate = new DateTime(2014, 01, 01);
            serverTime.Setup(st => st.RequestStarted).Returns(taskExecutionDate);

            var taskMessage = "{\"StartDate\":\"2014-01-01\",\"EndDate\":\"2014-02-01\"}";
            var message = new InsideModel.Models.Task() { Message = taskMessage };

            task.PerformTask(message);

            taskQueueStorage.Verify(ts => ts.Add(It.Is<InsideModel.Models.Task>(t=>t.Message=="{\"ClientId\":1,\"StartDate\":\"2014-01-01T00:00:00\",\"EndDate\":\"2014-02-01T00:00:00\"}")), Times.Exactly(1));
            taskQueueStorage.Verify(ts => ts.Add(It.Is<InsideModel.Models.Task>(t=>t.Message=="{\"ClientId\":3,\"StartDate\":\"2014-01-01T00:00:00\",\"EndDate\":\"2014-02-01T00:00:00\"}")), Times.Exactly(1));
        }

        [TestMethod]
        public void NoTaskCreatedForInactiveClientsOrClientsWithNoTrackingId()
        {
            var clientRepository = new LocalRepository<Client>();
            var taskQueueStorage = new Mock<ITaskQueueStorage>();
            var taskRepository = new LocalRepository<InsideModel.Models.Task>();
            var serverTime = new Mock<IServerTime>();
            var taskManager = new Mock<ITaskManager>();

            var task = new CreateUpdatePhonecallsTasksForClients(
                 clientRepository,
                 taskQueueStorage.Object,
                 taskRepository,
                 serverTime.Object,
                 taskManager.Object);

            clientRepository.Add(new Client()
            {
                IsActive = false,
                CallTrackingMetricId = 1

            });
            clientRepository.Add(new Client()
            {
                IsActive = true,

            });

            var taskExecutionDate = new DateTime(2014, 01, 01);
            serverTime.Setup(st => st.RequestStarted).Returns(taskExecutionDate);

            var taskMessage = "{\"StartDate\":\"2014-01-01\",\"EndDate\":\"2014-02-01\"}";
            var message = new InsideModel.Models.Task() { Message = taskMessage };

            task.PerformTask(message);

            Assert.AreEqual(0, taskRepository.All().Count());
            taskQueueStorage.Verify(ts => ts.Add(It.IsAny<InsideModel.Models.Task>()), Times.Never);
        }
    }
}
