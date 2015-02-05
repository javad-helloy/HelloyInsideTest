using System;
using System.Linq;
using System.Web.Mvc;
using Inside.membership;
using Inside.Time;
using InsideModel.Models;
using InsideModel.repositories;
using InsideReporting.Helpers;
using Microsoft.AspNet.Identity;

namespace InsideReporting.Controllers
{
    public class ReportController : AuthenticationController
    {
        private readonly IRepository<Client> clientRepository;
        private readonly IRepository<InsideUser> userRepository;
        private readonly IServerTime serverTime;
        private IIdentityMembershipProvider userManager;

        public ReportController(IRepository<Client> clientRepository,
            IRepository<InsideUser> userRepository,
            IServerTime serverTime,
            IIdentityMembershipProvider userManager)
            : base(userManager)
        {
            this.clientRepository = clientRepository;
            this.userRepository = userRepository;
            this.serverTime = serverTime;
            this.userManager = userManager;
        }

       //
        // GET: /Report/
        [Authorize(Roles = "demo,sales,consultant")]
        public ActionResult Index()
        {

            if (userManager.IsInRole(User.Identity.GetUserId(), "demo"))
            {
                return RedirectToAction("Demo", "Sirius");
            }
            else if (userManager.IsInRole(User.Identity.GetUserId(), "consultant"))
            {
                return RedirectToAction("Index", "Client");
            }
            else if (userManager.IsInRole(User.Identity.GetUserId(), "sales"))
            {
                return RedirectToAction("Index", "Client");
            }
            else if (userManager.IsInRole(User.Identity.GetUserId(), "client"))
            {
                var providerUserKey = User.Identity.GetUserId();

                var usersClientId = userRepository.Where(u => u.Id == providerUserKey).SingleOrDefault().Client.Id;
                return RedirectToRoute("reportcontacts", new { clientId = usersClientId });
            }

            return RedirectToAction("LogOn", "Account");
        }
         
        [AuthorizeClientAccess]
        public ActionResult Overview(int clientId, DateTime? startDate, DateTime? endDate)
        {

            return RedirectToAction("Overview", "Sirius", new {clientId=clientId});
        }

        //
        // GET: /HelloyReport/PhoneStatistics
        [Authorize(Roles = "consultant, client, demo")]
        [AuthorizeClientAccess]
        public ActionResult Lead(int clientId, DateTime? startDate, DateTime? endDate)
        {
            return RedirectToAction("ContactList", "Sirius", new { clientId = clientId });
        }


        //
        // GET: /HelloyReport/PhoneStatistics
        [Authorize(Roles = "consultant, client, demo")]
        [AuthorizeClientAccess]
        public ActionResult Contact(int clientId, DateTime? startDate, DateTime? endDate)
        {
            return RedirectToAction("ContactList", "Sirius", new { clientId = clientId });
        }

        [Authorize(Roles = "consultant, client, demo")]
        [AuthorizeClientAccess]
        public ActionResult Phone(int clientId, DateTime? startDate, DateTime? endDate)
        {
            return Redirect("/report/"+clientId+"/contact?type=Phone");
        }

        [Authorize(Roles = "consultant, client, demo")]
        [AuthorizeClientAccess]
        public ActionResult Chat(int clientId, DateTime? startDate, DateTime? endDate)
        {
            return Redirect("/report/" + clientId + "/contact?type=Chat");
        }

        [Authorize(Roles = "consultant, client, demo")]
        [AuthorizeClientAccess]
        public ActionResult Mail(int clientId, DateTime? startDate, DateTime? endDate)
        {

            return Redirect("/report/" + clientId + "/contact?type=Email");
        }

        [Authorize(Roles = "consultant, client, demo")]
        [AuthorizeClientAccess]
        public ActionResult Custom(int clientId, DateTime? startDate, DateTime? endDate)
        {
            return Redirect("/report/" + clientId + "/contact?type=Event");
        }

        [Authorize(Roles = "consultant, client, demo")]
        [AuthorizeClientAccess]
        public ActionResult Source(int clientId, DateTime? startDate, DateTime? endDate)
        {
            return RedirectToAction("ContactList", "Sirius", new { clientId = clientId });
        }

        //
        // GET: /HelloyReport/PhoneStatistics
        [Authorize(Roles = "consultant, client, demo")]
        [AuthorizeClientAccess]
        public ActionResult Organic(int clientId, DateTime? startDate, DateTime? endDate)
        {

            return Redirect("/report/" + clientId + "/contact?product=Organic");
        }


        //
        // GET: /HelloyReport/PhoneStatistics
        [Authorize(Roles = "consultant, client, demo")]
        [AuthorizeClientAccess]
        public ActionResult Search(int clientId, DateTime? startDate, DateTime? endDate)
        {

            return Redirect("/report/" + clientId + "/contact?product=Search");
        }

        //
        // GET: /HelloyReport/PhoneStatistics
        [Authorize(Roles = "consultant, client, demo")]
        [AuthorizeClientAccess]
        public ActionResult Retargeting(int clientId, DateTime? startDate, DateTime? endDate)
        {
            return Redirect("/report/" + clientId + "/contact?product=Retargeting");
        }

        //
        // GET: /HelloyReport/PhoneStatistics
        [Authorize(Roles = "consultant, client, demo")]
        [AuthorizeClientAccess]
        public ActionResult Display(int clientId, DateTime? startDate, DateTime? endDate)
        {

            return Redirect("/report/" + clientId + "/contact?product=Display");
        }

        //
        // GET: /HelloyReport/Latest
        
        [AuthorizeClientAccess]
        public ActionResult Latest(int clientId)
        {
            return RedirectToRoute("reportcontacts", new { clientId = clientId });
        }
	}
}