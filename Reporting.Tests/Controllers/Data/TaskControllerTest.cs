using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web.Mvc;
using Helpers.test;
using Inside.Time;
using InsideModel.Models;
using InsideModel.Models.TaskStorage;
using InsideModel.repositories;
using InsideReporting.Controllers.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Task.TaskCreator;

namespace InsideReporting.Tests.Controllers.Data
{
    [TestClass]
    public class TaskControllerTest
    {
        [TestMethod]
        public void CanConstruct()
        {
            var clientRepository = new Mock<IRepository<Client>>();
            var taskQueueStorage = new Mock<ITaskQueueStorage>();
            var taskRepository = new Mock<IRepository<InsideModel.Models.Task>>();
            var serverTime = new Mock<IServerTime>();
            var taskManager = new Mock<ITaskManager>();

            var taskController = new TaskController(
                clientRepository.Object,
                taskQueueStorage.Object,
                taskRepository.Object,
                serverTime.Object,
                taskManager.Object);
        }

        [TestMethod]
        public void CreateAppropriateTaskAndQueueMessageForOneOrMoreClientInClientRepository()
        {
            var clientRepository = new LocalRepository<Client>();
            var taskQueueStorage = new Mock<ITaskQueueStorage>();
            var taskRepository = new LocalRepository<InsideModel.Models.Task>();
            var serverTime = new Mock<IServerTime>();
            var taskManager = new Mock<ITaskManager>();

            var taskController = new TaskController(
                 clientRepository,
                 taskQueueStorage.Object,
                 taskRepository,
                 serverTime.Object,
                 taskManager.Object);

            var taskExecutionDate = new DateTime(2014, 01, 01);
            serverTime.Setup(st => st.RequestStarted).Returns(taskExecutionDate);

            var taskType = "CreateAnalyticDataTasksForClients";
            taskController.Create(taskType);

            taskQueueStorage.Verify(
                ts =>ts.Add(It.Is<InsideModel.Models.Task>(
                            t =>t.Message =="{\"StartDate\":\"2013-12-30T00:00:00\",\"EndDate\":\"2014-01-01T00:00:00\"}" &&
                                t.Type == "CreateAnalyticDataTasksForClients")), 
                            Times.Once);
        }

        [TestMethod]
        public void CanCreateCreateAnalyticDataTasksForClientsTask()
        {
            var clientRepository = new Mock<IRepository<Client>>();
            var taskQueueStorage = new Mock<ITaskQueueStorage>();
            var taskRepository = new LocalRepository<InsideModel.Models.Task>();
            var serverTime = new Mock<IServerTime>();
            var taskManager = new Mock<ITaskManager>();

            var controller = new TaskController(
                 clientRepository.Object,
                 taskQueueStorage.Object,
                 taskRepository,
                 serverTime.Object,
                 taskManager.Object);

            var taskExecutionDate = new DateTime(2014, 01, 01);
            serverTime.Setup(st => st.RequestStarted).Returns(taskExecutionDate);

            var taskType = "CreateAnalyticDataTasksForClients";
            controller.Create(taskType);

            taskQueueStorage.Verify(
                ts => ts.Add(It.Is<InsideModel.Models.Task>(
                            t => t.Message == "{\"StartDate\":\"2013-12-30T00:00:00\",\"EndDate\":\"2014-01-01T00:00:00\"}" &&
                                t.Type == "CreateAnalyticDataTasksForClients")),
                            Times.Once);

        }

        [TestMethod]
        public void CanCreateCreateImportSeoDataTask()
        {
            var clientRepository = new Mock<IRepository<Client>>();
            var taskQueueStorage = new Mock<ITaskQueueStorage>();
            var taskRepository = new LocalRepository<InsideModel.Models.Task>();
            var serverTime = new Mock<IServerTime>();
            var taskManager = new Mock<ITaskManager>();

            var controller = new TaskController(
                 clientRepository.Object,
                 taskQueueStorage.Object,
                 taskRepository,
                 serverTime.Object,
                 taskManager.Object);

            var taskExecutionDate = new DateTime(2014, 01, 01);
            serverTime.Setup(st => st.RequestStarted).Returns(taskExecutionDate);

            var taskType = "ImportSeoData";
            controller.Create(taskType);

            taskQueueStorage.Verify(
                ts => ts.Add(It.Is<InsideModel.Models.Task>(
                            t => t.Message == "" &&
                                t.Type == "ImportSeoData")),
                            Times.Once);
        }

        [TestMethod]
        public void CanCreateCreateImportWebEventsTaskCreator()
        {
            var clientRepository = new LocalRepository<Client>();
            var taskQueueStorage = new Mock<ITaskQueueStorage>();
            var taskRepository = new LocalRepository<InsideModel.Models.Task>();
            var serverTime = new Mock<IServerTime>();
            var taskManager = new Mock<ITaskManager>();

            var controller = new TaskController(
                 clientRepository,
                 taskQueueStorage.Object,
                 taskRepository,
                 serverTime.Object,
                 taskManager.Object);

            var taskExecutionDate = new DateTime(2014, 01, 01);
            serverTime.Setup(st => st.RequestStarted).Returns(taskExecutionDate);

            var taskType = "ImportCustomEvents";
            controller.Create(taskType);

            taskQueueStorage.Verify(
                ts => ts.Add(It.Is<InsideModel.Models.Task>(
                            t => t.Message == "" &&
                                t.Type == "ImportCustomEventsTaskCreator")),
                            Times.Once);

        }

        [TestMethod]
        
        public void TaskInRepositoryDoesntCreatTaskForAnalytics()
        {
            var clientRepository = new Mock<IRepository<Client>>();
            var taskQueueStorage = new Mock<ITaskQueueStorage>();
            var taskRepository = new LocalRepository<InsideModel.Models.Task>();
            var serverTime = new Mock<IServerTime>();
            var taskManager = new Mock<ITaskManager>();

            var controller = new TaskController(
                 clientRepository.Object,
                 taskQueueStorage.Object,
                 taskRepository,
                 serverTime.Object,
                 taskManager.Object);

            var taskExecutionDate = new DateTime(2014, 01, 01);
            serverTime.Setup(st => st.RequestStarted).Returns(taskExecutionDate);

            taskManager.Setup(
                tm =>
                    tm.HasTaskInRepository(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(),
                        It.IsAny<DateTime>())).Returns(true);

            var taskType = "CreateAnalyticDataTasksForClients";
            var reult  = controller.Create(taskType) as HttpStatusCodeResult;

            Assert.AreEqual(400, reult.StatusCode);
            Assert.AreEqual("Task Already in Database: {\"StartDate\":\"2013-12-30T00:00:00\",\"EndDate\":\"2014-01-01T00:00:00\"}", reult.StatusDescription);

            taskQueueStorage.Verify(ts => ts.Add(It.IsAny<InsideModel.Models.Task>()), Times.Never);

        }

        [TestMethod]
        
        public void TaskInRepositoryDoesntCreatTaskForImportCustomEvents()
        {
            var clientRepository = new LocalRepository<Client>();
            var taskQueueStorage = new Mock<ITaskQueueStorage>();
            var taskRepository = new LocalRepository<InsideModel.Models.Task>();
            var serverTime = new Mock<IServerTime>();
            var taskManager = new Mock<ITaskManager>();

            var controller = new TaskController(
                 clientRepository,
                 taskQueueStorage.Object,
                 taskRepository,
                 serverTime.Object,
                 taskManager.Object);

            var taskExecutionDate = new DateTime(2014, 01, 01);
            serverTime.Setup(st => st.RequestStarted).Returns(taskExecutionDate);

            var clientLable = new Label() { Id = 1, Name = "Custom Event" };
            var client1 = new Client() { Id = 1, Labels = new Collection<Label>() { clientLable } };
            clientRepository.Add(client1);
            

            taskManager.Setup(
                tm =>
                    tm.HasTaskInRepository(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(),
                        It.IsAny<DateTime>())).Returns(true);

            var taskType2 = "ImportCustomEvents";
            var reult = controller.Create(taskType2) as HttpStatusCodeResult;

            Assert.AreEqual(400, reult.StatusCode);
            Assert.AreEqual("Task Already in Database", reult.StatusDescription);

            taskQueueStorage.Verify(ts => ts.Add(It.IsAny<InsideModel.Models.Task>()), Times.Never);

        }

        [TestMethod]
        
        public void TaskInRepositoryDoesntCreatTaskForImportSeoData()
        {
            var clientRepository = new Mock<IRepository<Client>>();
            var taskQueueStorage = new Mock<ITaskQueueStorage>();
            var taskRepository = new LocalRepository<InsideModel.Models.Task>();
            var serverTime = new Mock<IServerTime>();
            var taskManager = new Mock<ITaskManager>();

            var controller = new TaskController(
                 clientRepository.Object,
                 taskQueueStorage.Object,
                 taskRepository,
                 serverTime.Object,
                 taskManager.Object);

            var taskExecutionDate = new DateTime(2014, 01, 01);
            serverTime.Setup(st => st.RequestStarted).Returns(taskExecutionDate);

            taskManager.Setup(
                tm =>
                    tm.HasTaskInRepository(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(),
                        It.IsAny<DateTime>())).Returns(true);

            var taskType3 = "ImportSeoData";
            var reult = controller.Create(taskType3) as HttpStatusCodeResult;

            Assert.AreEqual(400, reult.StatusCode);
            Assert.AreEqual("Task Already in Database", reult.StatusDescription);
            Assert.AreEqual(0, taskRepository.All().Count());
            taskQueueStorage.Verify(ts => ts.Add(It.IsAny<InsideModel.Models.Task>()), Times.Never);
        }

        
        
    }
}
