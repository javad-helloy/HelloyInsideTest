using System;
using System.Configuration;
using Inside.Time;
using InsideModel.Models.TaskStorage;
using InsideModel.repositories;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;


namespace Task.TaskPerformer.Storage
{
    public class AzureQueueTaskQueue : ITaskQueueStorage
    {
        private readonly IRepository<InsideModel.Models.Task> taskRepository;
        private readonly IServerTime serverTime;
        private CloudQueue queue;
        
        public AzureQueueTaskQueue(IRepository<InsideModel.Models.Task> taskRepository, IServerTime serverTime)
        {
            this.taskRepository = taskRepository;
            this.serverTime = serverTime;
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["StorageConnectionString"].ConnectionString);

            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

            queue = queueClient.GetQueueReference("task");
            
            queue.CreateIfNotExist();
        }

        
        public void Add(InsideModel.Models.Task queueTask)
        {
            taskRepository.Add(queueTask);
            taskRepository.SaveChanges();

            TimeSpan? initialInvisibility = null;
            if (serverTime.Now < queueTask.EarliestExecution)
            {
                initialInvisibility = queueTask.EarliestExecution - serverTime.Now;
            }

            var taskQueueMessage = new TaskQueueMessage() { Message = queueTask.Id.ToString() };
            var message = new CloudQueueMessage(taskQueueMessage.Message);
            queue.AddMessage(message, null, initialInvisibility);
        }

        public void ReQueue(InsideModel.Models.Task queueTask)
        {
            if (queueTask.NumTries > 0)
            {
                queueTask.EarliestExecution = serverTime.Now + GetNextExecutionDelay(queueTask.NumTries);
            }
            var taskQueueMessage = new TaskQueueMessage() { Message = queueTask.Id.ToString() };
            var message = new CloudQueueMessage(taskQueueMessage.Message);
            TimeSpan? initialInvisibility = null;
            if (serverTime.Now < queueTask.EarliestExecution)
            {
                initialInvisibility = queueTask.EarliestExecution - serverTime.Now;
            }

            
            queue.AddMessage(message, null, initialInvisibility);
        }

        private TimeSpan GetNextExecutionDelay(int numTries)
        {
            var numSeconds = (int)(Math.Pow(2, numTries+1) - 1) / 2 * 60;
            return new TimeSpan(0, 0, 0, numSeconds);
        }
    }
}

