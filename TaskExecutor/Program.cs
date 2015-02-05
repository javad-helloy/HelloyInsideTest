using System;
using Ninject;
using Task.TaskPerformer.Delegate;
using Microsoft.Azure.WebJobs;
namespace TaskExecutor
{
    public class Program
    {
        
        static void Main(string[] args)
        {
            Console.WriteLine("Task executor started");
            JobHostConfiguration configuration = new JobHostConfiguration();

            JobHost host = new JobHost();
            host.RunAndBlock();

        }

        public static void ListenToQueue([QueueTrigger("task")] string message)
        {
            ITaskDelegate taskDelegate = Container.Get<ITaskDelegate>();
            
            try
            {
                taskDelegate.PerformNextTask(message);
            }
            catch (Exception taskError)
            {
                Console.WriteLine("Unable to perform task error: " + taskError.Message);
                throw;
            }
        }

        static IKernel _container;
        public static IKernel Container
        {
            get
            {
                if (_container == null)
                {
                    _container = new StandardKernel(new TaskExecutorModule());
                }
                return _container;
            }
        }
    }
}
