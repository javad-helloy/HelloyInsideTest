namespace Task.TaskPerformer.Performer
{
    public interface ITaskPerformer
    {
        bool CanPerformTask(string taskType);
        void PerformTask(InsideModel.Models.Task taskMessage);
    }
}
