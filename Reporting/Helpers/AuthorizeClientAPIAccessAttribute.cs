using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using InsideModel.Models;
using InsideReporting.Controllers.Api;
using Microsoft.AspNet.Identity;

namespace InsideReporting.Helpers
{
    public class AuthorizeClientAPIAccessAttribute : System.Web.Http.Filters.ActionFilterAttribute
    {
        public ApiAuthenticationController controller;

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            controller = (ApiAuthenticationController) actionContext.ControllerContext.Controller;
            var hasClientIdInParameters = actionContext.ActionArguments.Any(a => a.Key == "clientId");

            if (hasClientIdInParameters)
            {
                var isAuthenticatedUser = actionContext.RequestContext.Principal.Identity.IsAuthenticated;
                if (!isAuthenticatedUser)
                {
                    actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                    base.OnActionExecuting(actionContext);
                    return;
                }

                var clientId = (int) actionContext.ActionArguments.First(a => a.Key == "clientId").Value;
                var context = new InsideContext();

                var userRepository = context.InsideUser;

               
                var userId = actionContext.RequestContext.Principal.Identity.GetUserId();
                if (userRepository.Any(u => u.Id == userId && u.Role.Any(r => r.Name == "consultant")))
                {
                    base.OnActionExecuting(actionContext);
                }
                else if (userRepository.Any(u => u.Id == userId && u.Role.Any(r => r.Name == "demo")))
                {
                    base.OnActionExecuting(actionContext);
                }
                else if (userRepository.Any(u => u.Id == userId && u.Role.Any(r => r.Name == "sales")))
                {
                    base.OnActionExecuting(actionContext);
                }
                else if (userRepository.Any(u => u.Id == userId && u.Role.Any(r => r.Name == "client")))
                {
                    var currentUserHasAccess = userRepository.Any((u => u.Id == userId && u.ClientId == clientId));
                    if (!currentUserHasAccess)
                    {
                        actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                        base.OnActionExecuting(actionContext);
                    }

                    base.OnActionExecuting(actionContext);
                }
                else
                {
                    actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                    base.OnActionExecuting(actionContext);
                }

            }
            else
            {
                base.OnActionExecuting(actionContext);
            }


        }

    }
}
    



   