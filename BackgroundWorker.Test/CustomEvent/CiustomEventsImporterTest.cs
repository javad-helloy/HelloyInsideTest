using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Task.ImportCustomEvents;

namespace BackgroundWorker.Test.CustomEvent
{
    [TestClass]
    public class CiustomEventsImporterTest
    {
        [TestMethod]
        public void CanCreate()
        {
            var customEventExtractor = new Mock<ICustomEventsExtractor>();

            var task = new CustomEventsImporter(customEventExtractor.Object);
        }

        [TestMethod]
        public void CanPerform()
        {
            var customEventExtractor = new Mock<ICustomEventsExtractor>();

            var task = new CustomEventsImporter(customEventExtractor.Object);

            var result = task.CanPerformTask("ImportCustomEvents");
            Assert.IsTrue(result);

            var resultFalse = task.CanPerformTask("This is not a Task");
            Assert.IsFalse(resultFalse);
        }

        [TestMethod]
        public void PerformTask()
        {
            var customEventExtractor = new Mock<ICustomEventsExtractor>();

            var task = new CustomEventsImporter(customEventExtractor.Object);

            var taskMessage = "{\"ClientId\":1,\"StartDate\":\"2014-01-01\",\"EndDate\":\"2014-01-02\"}";

            task.PerformTask(new InsideModel.Models.Task() { Message = taskMessage });
           
            var startDate = new DateTime(2014, 01, 01);
            var endDate = new DateTime(2014, 01, 02);
            var clientId = 1;
            customEventExtractor.Verify(ce => ce.ImportEvents(clientId,startDate, endDate), Times.Once);
            

        }
    }
}
