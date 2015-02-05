using InsideModel.Models;
using Newtonsoft.Json;

namespace Task.ImportCustomEvents
{
    public class CustomEventsImporter : ICustomEventsImporter
    {
        private readonly ICustomEventsExtractor customEventsExtractor;

        public CustomEventsImporter(
            ICustomEventsExtractor customEventsExtractor)
        {
            this.customEventsExtractor = customEventsExtractor;
        }

        public bool CanPerformTask(string taskType)
        {
            return taskType == TaskType.ImportCustomEvents;
        }

        public void PerformTask(InsideModel.Models.Task taskMessage)
        {
            var contactProductTask =
                JsonConvert.DeserializeObject<ImportEventsTaskMessage>(taskMessage.Message);

            customEventsExtractor.ImportEvents(contactProductTask.ClientId, contactProductTask.StartDate, contactProductTask.EndDate);

        }
    }

}
