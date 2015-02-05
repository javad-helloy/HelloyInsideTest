using System;
using System.Web.Mvc;
using RestSharp;

namespace InsideReporting.Controllers
{
    public class GeoController : Controller
    {
        private readonly IRestClient restClient;

        public GeoController(IRestClient restClient)
        {
            this.restClient = restClient;
        }

        //
        // GET: /Geo/Code
        public ActionResult Code(string address)
        {
            restClient.BaseUrl = new Uri("http://maps.googleapis.com");
            var request = new RestRequest("maps/api/geocode/json", Method.GET);

            request.AddParameter("address", address);
            request.AddParameter("sensor", "true");

            var result = restClient.Execute(request) ;

            return Content(result.Content, "application/json");
        }
    }
}
