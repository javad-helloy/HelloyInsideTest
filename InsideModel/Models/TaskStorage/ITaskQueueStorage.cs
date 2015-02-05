using System;

namespace InsideModel.Models.TaskStorage
{
    public interface ITaskQueueStorage
    {
        void Add(Task queueTask);
        void ReQueue(Task queueTask);
    }

    public class TaskQueueMessage 
    {
        public string Message { get; set; }
        public string Id { get; set; }
        public string PopReceipt { get; set; }
    }
}