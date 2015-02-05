using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Http.ExceptionHandling;

namespace InsideReporting.App_start
{
    public class TraceExceptionLogger : ExceptionLogger
    {
        public override void Log(ExceptionLoggerContext context)
        {
           NewRelic.Api.Agent.NewRelic.NoticeError(context.Exception);
           Trace.TraceError(context.ExceptionContext.Exception.ToString());
        }
    }
}