using System;
using System.Collections.Generic;
using System.Linq;
using Inside.Time;
using InsideModel.Models;
using InsideModel.Models.TaskStorage;
using InsideModel.repositories;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using Task.TaskCreator;
using Task.TaskPerformer.Performer;

namespace Task.ImportCustomEvents
{
    public interface IImportCustomEventsTaskCreator : ITaskPerformer
    {
    }

    public class ImportCustomEventsTaskCreator:IImportCustomEventsTaskCreator
    {
        private readonly IServerTime serverTime;
        private readonly ITaskManager taskManager;
        private readonly IRepository<InsideModel.Models.Task> taskRepository;
        private readonly IRepository<Client> clientRepository;
        private readonly ITaskQueueStorage taskQueueStorage;

        public ImportCustomEventsTaskCreator(IServerTime serverTime,
            ITaskManager taskManager,
            IRepository<InsideModel.Models.Task> taskRepository,
            IRepository<Client> clientRepository,
            ITaskQueueStorage taskQueueStorage)
        {
            this.serverTime = serverTime;
            this.taskManager = taskManager;
            this.taskRepository = taskRepository;
            this.clientRepository = clientRepository;
            this.taskQueueStorage = taskQueueStorage;
        }
        public bool CanPerformTask(string taskType)
        {
            return taskType == TaskType.ImportCustomEventsTaskCreator;
        }

        public void PerformTask(InsideModel.Models.Task taskMessage)
        {
            var dayOfExecutionLowerBound = serverTime.RequestStarted.Date;
            var dayOfExecutionUpperBound = dayOfExecutionLowerBound.AddDays(1);

            var clientsForImporting =
                clientRepository.Where(cr => cr.IsActive && cr.Labels.Any(l => l.Name == "Custom Event")).ToList();
            var addedTask = new List<InsideModel.Models.Task>();

            foreach (var client in clientsForImporting)
            {
                if (client.AnalyticsTableId.IsNullOrWhiteSpace())
                {
                    continue;
                }

                var endDate = serverTime.RequestStarted;
                var startDate = endDate.AddDays(-3);
                var clientId = client.Id;
                var createTaskForImportingWebEvents = new ImportEventsTaskMessage()
                {
                    ClientId = clientId,
                    StartDate = startDate.Date,
                    EndDate = endDate.Date
                };
                var messageToAdd = JsonConvert.SerializeObject(createTaskForImportingWebEvents);

                if (!taskManager.HasTaskInRepository(messageToAdd, TaskType.ImportCustomEvents,
                    dayOfExecutionLowerBound, dayOfExecutionUpperBound))
                {
                    var taskToAdd = new InsideModel.Models.Task(messageToAdd, TaskType.ImportCustomEvents,serverTime.RequestStarted);
                    taskQueueStorage.Add(taskToAdd);
                }
                else
                {
                   throw  new Exception("Task Already in Database: " + messageToAdd);
                }
            }
        }
    }
}
