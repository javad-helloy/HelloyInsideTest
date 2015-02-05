using System.Linq;
using System.Web.Mvc;
using Inside.membership;
using InsideModel.Models;
using InsideModel.repositories;
using InsideReporting.Helpers;
using Microsoft.AspNet.Identity;

namespace InsideReporting.Controllers
{
    public class HomeController : AuthenticationController
    {
        private readonly IRepository<InsideUser> userRepository;
        private IIdentityMembershipProvider userManager;

        public HomeController(
            IRepository<InsideUser> userRepository,
            IIdentityMembershipProvider userManager)
            : base(userManager)
        {
            this.userRepository = userRepository;
            this.userManager = userManager;
        }

        [AuthorizeClientAccess]
        public ActionResult Index()
        {
            if (userManager.IsInRole(User.Identity.GetUserId(),"demo"))
            {
                return RedirectToRoute("reportcontacts", new {clientId = 1});
            }
            else if (userManager.IsInRole(User.Identity.GetUserId(), "sales"))
            {
                return RedirectToAction("Index", "Client");
            }
            else if (userManager.IsInRole(User.Identity.GetUserId(), "consultant"))
            {
                return RedirectToAction("Index", "Client");
            }
            else if (userManager.IsInRole(User.Identity.GetUserId(), "client"))
            {
                var providerUserKey = User.Identity.GetUserId();
                var usersClientId = userRepository.Where(u => u.Id == providerUserKey).SingleOrDefault().Client.Id;
                return RedirectToRoute("reportcontacts", new {clientId = usersClientId});

            }

            return RedirectToAction("LogOn", "Account");
        }
    }
}
