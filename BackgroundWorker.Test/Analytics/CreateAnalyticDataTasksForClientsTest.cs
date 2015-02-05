using System;
using System.Linq;
using Helpers.test;
using Inside.Time;
using InsideModel.Models;
using InsideModel.Models.TaskStorage;
using InsideModel.repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Task.ContactProduct.AnalyticDataTaskCreator;
using Task.TaskCreator;

namespace BackgroundWorker.Test.Analytics
{
    [TestClass]
    public class CreateAnalyticDataTasksForClientsTest
    {
        [TestMethod]
        public void CanCreate()
        {
            var clientRepository= new Mock<IRepository<Client>>() ;
            var taskQueueStorage = new Mock<ITaskQueueStorage>() ;
            var taskRepository = new Mock<IRepository<InsideModel.Models.Task>>() ;
            var serverTime = new Mock<IServerTime>();
            var taskManagere = new Mock<ITaskManager>();
                var task = new CreateAnalyticDataTasksForClients(
                    clientRepository.Object,
                    taskQueueStorage.Object,
                    taskRepository.Object,
                    serverTime.Object,
                    taskManagere.Object);
        }

        [TestMethod]
        public void CanPerform()
        {
            var clientRepository = new Mock<IRepository<Client>>();
            var taskQueueStorage = new Mock<ITaskQueueStorage>();
            var taskRepository = new Mock<IRepository<InsideModel.Models.Task>>();
            var serverTime = new Mock<IServerTime>();
            var taskManagere = new Mock<ITaskManager>();
            var task = new CreateAnalyticDataTasksForClients(
                clientRepository.Object,
                taskQueueStorage.Object,
                taskRepository.Object,
                serverTime.Object,taskManagere.Object);

            var result = task.CanPerformTask("CreateAnalyticDataTasksForClients");
            Assert.IsTrue(result);

            var resultFalse = task.CanPerformTask("This is not a Task");
            Assert.IsFalse(resultFalse);
        }

        [TestMethod]
        public void CanCreateTaskForActiveClientsWithAnalyticId()
        {
            var clientRepository = new LocalRepository<Client>();
            var taskQueueStorage = new Mock<ITaskQueueStorage>();
            var taskRepository = new LocalRepository<InsideModel.Models.Task>();
            var serverTime = new Mock<IServerTime>();
            var taskManagere = new Mock<ITaskManager>();
            
            var task = new CreateAnalyticDataTasksForClients(
                clientRepository,
                taskQueueStorage.Object,
                taskRepository,
                serverTime.Object,taskManagere.Object);

            var client1 = new Client()
            {
                Id = 1,
                IsActive = true,
                AnalyticsTableId = "SomeId"
            };

            var client2 = new Client()
            {
                Id = 2,
                IsActive = true,
                AnalyticsTableId = "SomeId"
            };
            
            clientRepository.Add(client1);
            clientRepository.Add(client2);
            
            var taskExecutionDate = new DateTime(2014, 01, 01);
            serverTime.Setup(st => st.RequestStarted).Returns(taskExecutionDate);
            
            var taskMessage = "{\"StartDate\":\"2013-12-30\",\"EndDate\":\"2014-01-02\"}";
            var message = new InsideModel.Models.Task() { Message = taskMessage};

            task.PerformTask(message);

            //Assert.AreEqual(2, taskRepository.All().Count());
            taskQueueStorage.Verify(ts => ts.Add(It.Is<InsideModel.Models.Task>(t => t.Message == "{\"ClientId\":1,\"StartDate\":\"2013-12-30T00:00:00\",\"EndDate\":\"2014-01-02T00:00:00\"}" && t.Type == "AddProductAnalyticData")), Times.Exactly(1));
            taskQueueStorage.Verify(ts => ts.Add(It.Is<InsideModel.Models.Task>(t => t.Message == "{\"ClientId\":2,\"StartDate\":\"2013-12-30T00:00:00\",\"EndDate\":\"2014-01-02T00:00:00\"}" && t.Type == "AddProductAnalyticData")), Times.Exactly(1));
            
        }

        [TestMethod]
        public void IgnoreInactiveClients()
        {
            var clientRepository = new LocalRepository<Client>();
            var taskQueueStorage = new Mock<ITaskQueueStorage>();
            var taskRepository = new LocalRepository<InsideModel.Models.Task>();
            var serverTime = new Mock<IServerTime>();
            var taskManagere = new Mock<ITaskManager>();
            var task = new CreateAnalyticDataTasksForClients(
                clientRepository,
                taskQueueStorage.Object,
                taskRepository,
                serverTime.Object,
                taskManagere.Object);

            var client1 = new Client()
            {
                Id = 1,
                IsActive = false,
                AnalyticsTableId = "SomeId"
            };


            clientRepository.Add(client1);

            var taskExecutionDate = new DateTime(2014, 01, 01);
            serverTime.Setup(st => st.RequestStarted).Returns(taskExecutionDate);

            var taskMessage = "{\"StartDate\":\"2013-12-30\",\"EndDate\":\"2014-01-02\"}";
            task.PerformTask(new InsideModel.Models.Task() { Message = taskMessage });

            Assert.AreEqual(0, taskRepository.All().Count());

            taskQueueStorage.Verify(ts => ts.Add(It.IsAny<InsideModel.Models.Task>()), Times.Never);

        }

        [TestMethod]
        public void IgnoreClientsWithNoAnalyticId()
        {
            var clientRepository = new LocalRepository<Client>();
            var taskQueueStorage = new Mock<ITaskQueueStorage>();
            var taskRepository = new LocalRepository<InsideModel.Models.Task>();
            var serverTime = new Mock<IServerTime>();
            var taskManagere = new Mock<ITaskManager>();
            var task = new CreateAnalyticDataTasksForClients(
                clientRepository,
                taskQueueStorage.Object,
                taskRepository,
                serverTime.Object,
                taskManagere.Object);

            var client1 = new Client()
            {
                Id = 1,
                IsActive = true,
                AnalyticsTableId = ""
            };


            clientRepository.Add(client1);

            var taskExecutionDate = new DateTime(2014, 01, 01);
            serverTime.Setup(st => st.RequestStarted).Returns(taskExecutionDate);

            var taskMessage = "{\"StartDate\":\"2013-12-30\",\"EndDate\":\"2014-01-02\"}";
            task.PerformTask(new InsideModel.Models.Task() { Message = taskMessage });

            Assert.AreEqual(0, taskRepository.All().Count());

            taskQueueStorage.Verify(ts => ts.Add(It.IsAny<InsideModel.Models.Task>()), Times.Never);

        }
    }
}
