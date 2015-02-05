using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Task.ContactProduct.AnalyticData;

namespace BackgroundWorker.Test.Analytics
{
    [TestClass]
    public class AddAnalyticProductDataForClientTest
    {
        [TestMethod]
        public void CanCreate()
        {
            var analyticsDataExtractor = new Mock<IAnalyticDataPropertyExtractor>() ;

            var task = new AddAnalyticProductDataForClient(analyticsDataExtractor.Object);
        }

        [TestMethod]
        public void CanPerform()
        {
            var analyticsDataExtractor = new Mock<IAnalyticDataPropertyExtractor>();

            var task = new AddAnalyticProductDataForClient(analyticsDataExtractor.Object);

            var result = task.CanPerformTask("AddProductAnalyticData");
            Assert.IsTrue(result);

            var resultFalse = task.CanPerformTask("This is not a Task");
            Assert.IsFalse(resultFalse);
        }

        [TestMethod]
        public void PerformTask()
        {
            var analyticsDataExtractor = new Mock<IAnalyticDataPropertyExtractor>();

            var task = new AddAnalyticProductDataForClient(analyticsDataExtractor.Object);

            var taskMessage = "{\"ClientId\":\"1\",\"StartDate\":\"2014-01-01\",\"EndDate\":\"2014-01-02\"}";
            var startDate = new DateTime(2014, 01, 01);
            var endDate = new DateTime(2014, 01, 02);
            var clientId = 1;

            task.PerformTask(new InsideModel.Models.Task() { Message = taskMessage });
            analyticsDataExtractor.Verify(ce => ce.UpdateContact(clientId, startDate, endDate), Times.Once);
            
            
        }
    }
}
