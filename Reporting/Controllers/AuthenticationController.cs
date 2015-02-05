using System.Web.Mvc;
using System.Web.SessionState;
using Inside.membership;
using Microsoft.AspNet.Identity;

namespace InsideReporting.Controllers
{
    [SessionState(SessionStateBehavior.ReadOnly)]
    public class AuthenticationController : Controller
    {
        private IIdentityMembershipProvider userManager;

        public AuthenticationController(IIdentityMembershipProvider userManager)
        {
            this.userManager = userManager;

        }


        public bool CurrentUserHasAccessToClient(int? clientId)
        {
            var userId = User.Identity.GetUserId();
            if (userId == null) { return false; }
            if (userManager.IsInRole(userId, "consultant"))
            {
                return true;
            }
            else if (userManager.IsInRole(userId,  "demo"))
            {
                return true;
            }
            else if (userManager.IsInRole(userId, "sales"))
            {
                return true;
            }
            else if (userManager.IsInRole(userId,  "client"))
            {
                if (clientId == null) { return true;}
                var user = userManager.FindById(userId);
                if (user.Client.Id == clientId)
                {
                    return true;
                }
                else { return false;}
            }
            else
            {
                return false;

            }
        }
    }
}