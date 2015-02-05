using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InsideReporting.Controllers.Dev
{
    public class Dev3Controller : Controller
    {
        // GET: Dev3
        public ActionResult Index()
        {
            if (User.IsInRole("consultant"))
            {
                return Content("consultant");
            }
            else
            {
                return Content("Not Consultant");
            }
        }
    }
}