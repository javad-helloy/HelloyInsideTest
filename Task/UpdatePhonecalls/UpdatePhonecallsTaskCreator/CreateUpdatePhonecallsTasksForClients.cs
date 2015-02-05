using System.Collections.Generic;
using Inside.Time;
using InsideModel.Models;
using InsideModel.Models.TaskStorage;
using InsideModel.repositories;
using Newtonsoft.Json;
using Task.TaskCreator;
using Task.UpdatePhonecalls.UpdatePhoneCalls;

namespace Task.UpdatePhonecalls.UpdatePhonecallsTaskCreator
{
    public class CreateUpdatePhonecallsTasksForClients : ICreateUpdatePhonecallsTasksForClients
    {
        private readonly IRepository<Client> clientRepository;
        private readonly ITaskQueueStorage taskQueueStorage;
        private readonly IRepository<InsideModel.Models.Task> taskRepository;
        private readonly IServerTime serverTime;
        private readonly ITaskManager taskManager;

        public CreateUpdatePhonecallsTasksForClients(
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
            return taskType == TaskType.CreateUpdatePhonecallsTasksForClients;
        }

        public void PerformTask(InsideModel.Models.Task taskMessage)
        {
            var createUpdatePhoneCallTask = JsonConvert.DeserializeObject<UpdatePhonecallsTaskMesage>(taskMessage.Message);

            var clients = clientRepository.Where(c => c.IsActive && c.CallTrackingMetricId != null);
            var dayOfExecutionLowerBound = serverTime.RequestStarted.Date;
            var dayOfExecutionUpperBound = serverTime.RequestStarted.AddDays(1).Date;
            var addedTask = new List<InsideModel.Models.Task>();
            foreach (var client in clients)
            {
                var messageToAdd =
                JsonConvert.SerializeObject(new UpdatePhonecallsForClientsTaskMessage
                {
                    ClientId = client.Id,
                    StartDate = createUpdatePhoneCallTask.StartDate,
                    EndDate = createUpdatePhoneCallTask.EndDate
                });

                if (!taskManager.HasTaskInRepository(messageToAdd, TaskType.UpdatePhonecalls, dayOfExecutionLowerBound,
                        dayOfExecutionUpperBound))
                {
                    var taskToAdd = new InsideModel.Models.Task(messageToAdd, TaskType.UpdatePhonecalls, serverTime.RequestStarted);
                    taskQueueStorage.Add(taskToAdd);
                }

            }
        }
    }
}
