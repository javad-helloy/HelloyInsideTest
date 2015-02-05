using System;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Threading;
using Microsoft.WindowsAzure.ServiceRuntime;
using Ninject;
using Task.TaskPerformer.Delegate;

namespace BackgroundWorker
{
    public class WorkerRole : RoleEntryPoint 
    {
        public override void Run()
        {
            Trace.Listeners.Add(new Microsoft.WindowsAzure.Diagnostics.DiagnosticMonitorTraceListener());

            Trace.TraceInformation("BackgroundWorker entry point called", "Information");

            while (true)
            {
                ITaskDelegate taskDelegate = Container.Get<ITaskDelegate>();

                try
                {
                    taskDelegate.PerformNextTask();
                }
                catch (Exception taskError)
                {
                    Trace.TraceError("Unable to perform task", taskError);
                }
                

                Thread.Sleep(5*1000);
                Trace.TraceInformation("Working", "Information");
            }
        }

        static IKernel _container;
        public static IKernel Container
        {
            get
            {
                if (_container == null)
                {
                    _container = new StandardKernel(new BackgroundWorkerModule());
                }
                return _container;
            }
        }

        public override bool OnStart()
        {
            ServicePointManager.DefaultConnectionLimit = 12;

            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            return base.OnStart();
        }
    }
}
