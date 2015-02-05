using System.Web.Mvc;
using InsideReporting.App_start;

namespace InsideReporting
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new CustomMvcExceptionFilter());
            filters.Add(new HandleErrorAttribute());
        }
    }
}
