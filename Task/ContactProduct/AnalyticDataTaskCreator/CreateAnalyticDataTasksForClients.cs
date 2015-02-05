using System;
using System.Collections.Generic;
using System.Threading;
using Inside.Time;
using InsideModel.Models;
using InsideModel.Models.TaskStorage;
using InsideModel.repositories;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using Task.ContactProduct.AnalyticData;
using Task.TaskCreator;

namespace Task.ContactProduct.AnalyticDataTaskCreator
{
    public class CreateAnalyticDataTasksForClients : ICreateAnalyticDataTasksForClients
    {
         
        private readonly IRepository<Client> clientRepository;
        private ITaskQueueStorage taskQueueStorage;
        private IRepository<InsideModel.Models.Task> taskRepository;
        private IServerTime serverTime;
        private readonly ITaskManager taskManager;

        public CreateAnalyticDataTasksForClients(
            IRepository<Client> clientRepository,
            ITaskQueueStorage taskQueueStorage,
            IRepository<InsideModel.Models.Task> taskRepository,
            IServerTime serverTime,
            ITaskManager taskManager)
        {
            this.clientRepository = clientRepository;
            this.taskQueueStorage = taskQueueStorage;
            this.taskRepository = taskRepository;
            this.serverTime = serverTime;
            this.taskManager = taskManager;
        }

        public bool CanPerformTask(string taskType)
        {
            return taskType == TaskType.CreateAnalyticDataTasksForClients;
        }

        public void PerformTask(InsideModel.Models.Task taskMessage)
        {
            var createProductAnalyticTask = JsonConvert.DeserializeObject<TaskMessageWithStartDateAndEndDate>(taskMessage.Message);

            var clients = clientRepository.Where(c => c.IsActive);
            var dayOfExecution = serverTime.RequestStarted.Date;
            var dayAfterExecution = serverTime.RequestStarted.AddDays(1).Date;
            var addedTask = new List<InsideModel.Models.Task>();
            var taskNum = 0;
            foreach (var client in clients)
            {
                if (client.AnalyticsTableId.IsNullOrWhiteSpace())
                {
                    continue;
                }
                
                var messageToAdd =
                JsonConvert.SerializeObject(new ImportContactProductAnalyticDataTaskMessage()
                {
                    ClientId = client.Id,
                    StartDate = createProductAnalyticTask.StartDate,
                    EndDate = createProductAnalyticTask.EndDate
                });

                if (!taskManager.HasTaskInRepository(messageToAdd, TaskType.AddProductAnalyticData, dayOfExecution,
                        dayAfterExecution))
                {
                    taskNum++;
                    var earliestExecution = serverTime.RequestStarted + GetNextExecutionDelay(taskNum);;
                    var taskToAdd = new InsideModel.Models.Task(messageToAdd, TaskType.AddProductAnalyticData, earliestExecution);
                    taskQueueStorage.Add(taskToAdd);
                }
            }

        }

        private TimeSpan GetNextExecutionDelay(int taskNum)
        {
            var numSeconds = 15*(taskNum-1);

            return new TimeSpan(0, 0, 0, numSeconds);
        }
    }
}
