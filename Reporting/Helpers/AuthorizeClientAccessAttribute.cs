using System.Linq;
using System.Web.Mvc;
using InsideReporting.Controllers;

namespace InsideReporting.Helpers
{
    public class AuthorizeClientAccessAttribute : AuthorizeAttribute
    {
        public AuthenticationController controller;
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            controller = (AuthenticationController) filterContext.Controller;

            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                HandleUnauthorizedRequest(filterContext);
                return;
            }
            var hasClientIdInParameters = filterContext.HttpContext.Request.Params.AllKeys.Any(key => key.ToLower() == "clientid");
            int? clientId =null;
            if (hasClientIdInParameters)
            {
                var clientSringValue = filterContext.HttpContext.Request.Params.GetValues("clientId").FirstOrDefault();
                if (clientSringValue != null)
                {
                    clientId = int.Parse(clientSringValue);
                }
            }

            if (controller.CurrentUserHasAccessToClient(clientId))
            {
                 base.AuthorizeCore(filterContext.HttpContext);
            }
            else
            {
                HandleUnauthorizedRequest(filterContext);
            }
        }
    }
}