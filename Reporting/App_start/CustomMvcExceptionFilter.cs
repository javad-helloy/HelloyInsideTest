using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InsideReporting.App_start
{
    public class CustomMvcExceptionFilter :IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            NewRelic.Api.Agent.NewRelic.NoticeError(filterContext.Exception);
            Trace.TraceError(filterContext.Exception.Message);
        }
    }
}