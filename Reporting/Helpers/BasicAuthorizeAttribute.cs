using System;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace InsideReporting.Helpers
{
    public class BasicAuthorizeAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }

            string auth = filterContext.HttpContext.Request.Headers["Authorization"];

            if (!String.IsNullOrEmpty(auth))
            {
                byte[] encodedDataAsBytes = Convert.FromBase64String(auth.Replace("Basic ", ""));
                string value = Encoding.ASCII.GetString(encodedDataAsBytes);
                string username = value.Substring(0, value.IndexOf(':'));
                string password = value.Substring(value.IndexOf(':') + 1);

                if (CheckPassword(username, password))
                {
                    filterContext.HttpContext.User = new GenericPrincipal(new GenericIdentity(username,"BasicAuthenticationForTaskCreating"), null);
                }
                else
                {
                    filterContext.Result = new HttpStatusCodeResult(400);
                }
            }
            else
            {
                if (AuthorizeCore(filterContext.HttpContext))
                {
                    HttpCachePolicyBase cachePolicy = filterContext.HttpContext.Response.Cache;
                    cachePolicy.SetProxyMaxAge(new TimeSpan(0));
                    cachePolicy.AddValidationCallback(CacheValidateHandler, null);
                }
                else
                {
                    filterContext.HttpContext.Response.Clear();
                    filterContext.HttpContext.Response.StatusDescription = "BadRequest";
                    filterContext.HttpContext.Response.AddHeader("WWW-Authenticate", "Basic realm=\"Secure Area\"");
                    filterContext.HttpContext.Response.Write("400, please authenticate");
                    filterContext.HttpContext.Response.StatusCode = 400;
                    filterContext.Result = new EmptyResult();
                    filterContext.HttpContext.Response.End();
                }
            }
        }

        private void CacheValidateHandler(HttpContext context, object data, ref HttpValidationStatus validationStatus)
        {
            validationStatus = OnCacheAuthorization(new HttpContextWrapper(context));
        }

        private static bool CheckPassword(string username, string password)
        {
            return username == "helloy" && password == "helloy123";
        }

    }
}