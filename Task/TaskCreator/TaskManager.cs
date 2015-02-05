using System;
using System.Linq;
using InsideModel.repositories;

namespace Task.TaskCreator
{
    public class TaskManager:ITaskManager
    {
        private readonly IRepository<InsideModel.Models.Task> taskRepository;
        
        public TaskManager(IRepository<InsideModel.Models.Task> taskRepository)
        {
            this.taskRepository = taskRepository;
        }

        public bool HasTaskInRepository(string taskMessage, string taskType, DateTime lowerBoundDate, DateTime upperBoundDate)
        {
            var hasTaskInRepository = taskRepository.Where(
                tr => tr.Message == taskMessage
                      && tr.EarliestExecution >= lowerBoundDate
                      && tr.EarliestExecution < upperBoundDate
                      && tr.Type == taskType).Any();
            return hasTaskInRepository;
        }
    }
}
