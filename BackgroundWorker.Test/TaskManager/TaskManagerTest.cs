using System;
using System.Linq;
using Helpers.test;
using Inside.Time;
using InsideModel.Models;
using InsideModel.Models.TaskStorage;
using InsideModel.repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;


namespace BackgroundWorker.Test.TaskManager
{
    [TestClass]
    public class TaskManagerTest
    {
        [TestMethod]
        public void CanConstruct()
        {
            
            var taskRepository = new Mock<IRepository<InsideModel.Models.Task>>();
            var taskManager = new Task.TaskCreator.TaskManager(taskRepository.Object);
        }


        [TestMethod]
        public void CanCheckIfTaskInRepository()
        {
            var taskRepository = new LocalRepository<InsideModel.Models.Task>();
            var taskManager = new Task.TaskCreator.TaskManager(taskRepository);

            var now = new DateTime(2013, 01, 01, 08, 0, 0);
            var lowerBoundDate = now.Date;
            var upperBoundDate = lowerBoundDate.AddDays(1);

            var taskMessage = "{\"ClientId\":1,\"StartDate\":\"2013-12-30T00:00:00\",\"EndDate\":\"2014-01-02T00:00:00\"}";

            var result = taskManager.HasTaskInRepository(taskMessage, TaskType.AddProductAnalyticData,lowerBoundDate,upperBoundDate);

            Assert.IsFalse(result);

            taskRepository.Add(new InsideModel.Models.Task()
            {
                EarliestExecution = now.AddHours(2),
                Message = taskMessage,
                Type = TaskType.AddProductAnalyticData
            });

            Assert.AreEqual(1, taskRepository.All().Count());
            var result2 = taskManager.HasTaskInRepository(taskMessage, TaskType.AddProductAnalyticData, lowerBoundDate, upperBoundDate);



            Assert.IsTrue(result2);
        }
    }
}
