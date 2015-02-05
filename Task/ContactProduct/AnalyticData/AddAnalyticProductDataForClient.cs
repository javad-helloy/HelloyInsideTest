using InsideModel.Models;
using Newtonsoft.Json;


namespace Task.ContactProduct.AnalyticData
{
    public class AddAnalyticProductDataForClient : IAddAnalyticProductDataForClient
    {
        private readonly IAnalyticDataPropertyExtractor analyticsDataExtractor;

        public AddAnalyticProductDataForClient(
            IAnalyticDataPropertyExtractor analyticsDataExtractor)
        {
            this.analyticsDataExtractor = analyticsDataExtractor;
        }

        public bool CanPerformTask(string taskType)
        {
            return taskType == TaskType.AddProductAnalyticData;
        }

        public void PerformTask(InsideModel.Models.Task taskMessage)
        {
            var contactProductTask =
                JsonConvert.DeserializeObject<ImportContactProductAnalyticDataTaskMessage>(taskMessage.Message);

            analyticsDataExtractor.UpdateContact(contactProductTask.ClientId, contactProductTask.StartDate,
                contactProductTask.EndDate);

        }
    }

}
