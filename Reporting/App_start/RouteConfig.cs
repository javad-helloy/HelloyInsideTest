using System.Web.Mvc;
using System.Web.Routing;
using LowercaseRoutesMVC;

namespace InsideReporting
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("reportresult", "report/{clientId}/result",
                new { controller = "Sirius", action = "Overview", clientId = "" });

            routes.MapRoute("reportcontacts", "report/{clientId}/contact",
                new { controller = "Sirius", action = "ContactList", clientId = "" });

            routes.MapRoute("reportcontact", "report/{clientId}/contact/{contactId}",
                new { controller = "Sirius", action = "Contact", clientId = "", contactId = "" });

            routes.MapRoute("reportcampaign", "report/{clientId}/campaign",
                new { controller = "Sirius", action = "Campaign", clientId = "" });

            routes.MapRoute("reportmyaccount", "report/{clientId}/myaccount",
                new { controller = "Sirius", action = "MyAccount", clientId = "" });

            routes.MapRoute("reportwebtab", "report/{clientId}/webb",
               new { controller = "Sirius", action = "WebTab", clientId = "" });

            routes.MapRouteLowercase(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
