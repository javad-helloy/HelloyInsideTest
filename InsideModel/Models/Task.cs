using System;
using System.Diagnostics.CodeAnalysis;

namespace InsideModel.Models
{
    [ExcludeFromCodeCoverage]
    public partial class Task
    {
        public Task()
        {
        }

        public Task(string taskMessage, string taskType, DateTime earliestExecution)
        {
            this.Message = taskMessage;
            this.Type = taskType;
            this.EarliestExecution = earliestExecution;
            this.Status = TaskStatus.Queued;
            this.StartDate = null;
            this.UpdateDate = null;
        }
        public int Id { get; set; }
        public string Type { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }
        public int NumTries { get; set; }
        public DateTime EarliestExecution { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? UpdateDate { get; set; }

    }
}
