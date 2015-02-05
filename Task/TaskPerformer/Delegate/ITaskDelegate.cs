namespace Task.TaskPerformer.Delegate
{
    public interface ITaskDelegate
    {
        void PerformNextTask(string messageOfTaskToPerform);
    }
}
