using System;
using System.Collections.ObjectModel;
using System.Linq;
using Helpers.test;
using Inside.Time;
using InsideModel.Models;
using InsideModel.Models.TaskStorage;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Task.ImportCustomEvents;
using Task.TaskCreator;

namespace BackgroundWorker.Test.CustomEvent
{
    [TestClass]
    public class CustomEventTaskCreatorTest
    {
        [TestMethod]
        public void CanConstruct()
        {
            var clientRepository = new LocalRepository<Client>();
            var taskQueueStorage = new Mock<ITaskQueueStorage>();
            var taskRepository = new LocalRepository<InsideModel.Models.Task>();
            var serverTime = new Mock<IServerTime>();
            var taskManager = new Mock<ITaskManager>();

            var task = new ImportCustomEventsTaskCreator(
                 serverTime.Object,
                 taskManager.Object,
                 taskRepository,
                 clientRepository,
                 taskQueueStorage.Object);
        }

        [TestMethod]
        public void CanPerform()
        {
            var clientRepository = new LocalRepository<Client>();
            var taskQueueStorage = new Mock<ITaskQueueStorage>();
            var taskRepository = new LocalRepository<InsideModel.Models.Task>();
            var serverTime = new Mock<IServerTime>();
            var taskManager = new Mock<ITaskManager>();

            var task = new ImportCustomEventsTaskCreator(
                 serverTime.Object,
                 taskManager.Object,
                 taskRepository,
                 clientRepository,
                 taskQueueStorage.Object);

            var result = task.CanPerformTask("ImportCustomEventsTaskCreator");
            Assert.IsTrue(result);

            var resultFalse = task.CanPerformTask("This is not a Task");
            Assert.IsFalse(resultFalse);
        }

        [TestMethod]
        public void CanCreateCreateImportWebEventsTaskForActiveClientsWithCorrectLabelAndAlayticsId()
        {
            var clientRepository = new LocalRepository<Client>();
            var taskQueueStorage = new Mock<ITaskQueueStorage>();
            var taskRepository = new LocalRepository<InsideModel.Models.Task>();
            var serverTime = new Mock<IServerTime>();
            var taskManager = new Mock<ITaskManager>();

            var task = new ImportCustomEventsTaskCreator(
                 serverTime.Object,
                 taskManager.Object,
                 taskRepository,
                 clientRepository,
                 taskQueueStorage.Object);

            var clientLable = new Label() { Id = 1, Name = "Custom Event" };
            var client1 = new Client() { Id = 1, Labels = new Collection<Label>() { clientLable }, IsActive = true, AnalyticsTableId = "someId"};
            var client2 = new Client() { Id = 2, Labels = new Collection<Label>() { clientLable }, IsActive = true, AnalyticsTableId = "someId" };
            var clientWithoutLabel = new Client() { Id = 3 };
            clientRepository.Add(client1);
            clientRepository.Add(client2);
            clientRepository.Add(clientWithoutLabel);

            var taskExecutionDate = new DateTime(2014, 01, 01);
            serverTime.Setup(st => st.RequestStarted).Returns(taskExecutionDate);

            task.PerformTask(new InsideModel.Models.Task() { Message = "" });

            taskQueueStorage.Verify(ts => ts.Add(It.Is<InsideModel.Models.Task>(t => t.Message == "{\"ClientId\":1,\"StartDate\":\"2013-12-29T00:00:00\",\"EndDate\":\"2014-01-01T00:00:00\"}" && t.Type == "ImportCustomEvents")), Times.Exactly(1));
            taskQueueStorage.Verify(ts => ts.Add(It.Is<InsideModel.Models.Task>(t => t.Message == "{\"ClientId\":2,\"StartDate\":\"2013-12-29T00:00:00\",\"EndDate\":\"2014-01-01T00:00:00\"}" && t.Type == "ImportCustomEvents")), Times.Exactly(1));

        }

        [TestMethod]
        public void InActiveClientOrClientWithNoAnalyticIdDontCreateTask()
        {
            var clientRepository = new LocalRepository<Client>();
            var taskQueueStorage = new Mock<ITaskQueueStorage>();
            var taskRepository = new LocalRepository<InsideModel.Models.Task>();
            var serverTime = new Mock<IServerTime>();
            var taskManager = new Mock<ITaskManager>();

            var task = new ImportCustomEventsTaskCreator(
                 serverTime.Object,
                 taskManager.Object,
                 taskRepository,
                 clientRepository,
                 taskQueueStorage.Object);

            var clientLable = new Label() { Id = 1, Name = "Custom Event" };
            var inActiveClient = new Client() { Id = 1, Labels = new Collection<Label>() { clientLable }, AnalyticsTableId = "someId" };
            var noAnalyticIdclient = new Client() { Id = 2, Labels = new Collection<Label>() { clientLable }, IsActive = true};
            var clientWithoutLabel = new Client() { Id = 3 };
            clientRepository.Add(inActiveClient);
            clientRepository.Add(noAnalyticIdclient);
            clientRepository.Add(clientWithoutLabel);

            var taskExecutionDate = new DateTime(2014, 01, 01);
            serverTime.Setup(st => st.RequestStarted).Returns(taskExecutionDate);

            task.PerformTask(new InsideModel.Models.Task() { Message = "" });

            Assert.AreEqual(0, taskRepository.All().Count());
            taskQueueStorage.Verify(ts => ts.Add(It.IsAny<InsideModel.Models.Task>()), Times.Never);

        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void TaskInRepositoryDoesntCreatTaskForImportCustomEvents()
        {
            var clientRepository = new LocalRepository<Client>();
            var taskQueueStorage = new Mock<ITaskQueueStorage>();
            var taskRepository = new LocalRepository<InsideModel.Models.Task>();
            var serverTime = new Mock<IServerTime>();
            var taskManager = new Mock<ITaskManager>();

            var task = new ImportCustomEventsTaskCreator(
                  serverTime.Object,
                 taskManager.Object,
                 taskRepository,
                 clientRepository,
                 taskQueueStorage.Object);

            var taskExecutionDate = new DateTime(2014, 01, 01);
            serverTime.Setup(st => st.RequestStarted).Returns(taskExecutionDate);

            var clientLable = new Label() { Id = 1, Name = "Custom Event" };
            var client1 = new Client() { Id = 1, Labels = new Collection<Label>() { clientLable }, IsActive = true, AnalyticsTableId = "someId" };
            clientRepository.Add(client1);


            taskManager.Setup(
                tm =>
                    tm.HasTaskInRepository(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(),
                        It.IsAny<DateTime>())).Returns(true);

            task.PerformTask(new InsideModel.Models.Task() { Message = "" });

            Assert.AreEqual(0, taskRepository.All().Count());
            taskQueueStorage.Verify(ts => ts.Add(It.IsAny<InsideModel.Models.Task>()), Times.Never);

        }
    }
}
