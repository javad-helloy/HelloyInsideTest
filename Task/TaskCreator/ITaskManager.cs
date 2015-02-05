using System;

namespace Task.TaskCreator
{
    public interface ITaskManager
    {
        bool HasTaskInRepository(string taskMessage, string taskType, DateTime lowerBoundDate, DateTime upperBoundDate);
    }
}
