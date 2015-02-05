using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using System.Web.SessionState;
using Inside.Analytics;
using Inside.Extenssions;
using Inside.GoogleService;
using InsideModel.Models;
using InsideModel.repositories;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace InsideReporting.Controllers
{
    [SessionState(SessionStateBehavior.ReadOnly)]
    public class LiveLeadsController : Controller
    {
        //
        // GET: /LiveLeads/
        [Authorize(Roles = "consultant")]
        public ActionResult Index()
        {
            var model = new LiveLeadUserModel();
            model.UserId = this.User.Identity.GetUserId();

            return View(model);
        }


        public class LiveLeadUserModel
        {
            public string UserId { get; set; }

        }
    }
}
