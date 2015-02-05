using System.Web;
using System.Web.Security;

namespace InsideReporting.Helpers
{
    public static class MenuSecurity
    {
        public static bool IsLoggedInConsultant
        {
            get
            {
                try
                {
                    return HttpContext.Current.User != null &&
                           HttpContext.Current.User.Identity.IsAuthenticated &&
                           Roles.IsUserInRole(HttpContext.Current.User.Identity.Name, "consultant") &&
                           !Roles.IsUserInRole(HttpContext.Current.User.Identity.Name, "sales");
                }
                catch
                {
                    return false;
                }
            }
        }
        public static bool IsLoggedInSales
        {
            get
            {
                try
                {
                    return HttpContext.Current.User != null &&
                           HttpContext.Current.User.Identity.IsAuthenticated &&
                           Roles.IsUserInRole(HttpContext.Current.User.Identity.Name, "sales");
                }
                catch
                {
                    return false;
                }
            }
        }     
    }
}