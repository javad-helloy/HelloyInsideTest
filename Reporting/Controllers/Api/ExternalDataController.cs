using System;
using System.Net;
using System.Web.Http;
using RestSharp;

namespace InsideReporting.Controllers.Api
{
    public class ExternalDataController : ApiController
    {
        private readonly IRestClient restClient;

        public ExternalDataController(IRestClient restClient)
        {
            this.restClient = restClient;
        }
        // GET: CallTrackingMetrics
        [Route("api/externaldata/phone/account/{callTrackingAccountId}/calls")]
        public IHttpActionResult GetPhoneData(int callTrackingAccountId, DateTime startDate, DateTime endDate, int? page)
        {
            if (!User.IsInRole("consultant"))
            {
                return NotFound();    
            }
            restClient.BaseUrl = new Uri("https://api.calltrackingmetrics.com");
            var request =
                    new RestRequest("api/v1/accounts/" + callTrackingAccountId + "/calls.json",
                        Method.GET);

            var username = "dev@helloy.se";
            var password = "u=-Y5+rVrU5.$RQ";

            request.AddParameter("start_date", startDate.ToString("yyyy-MM-dd"));
            request.AddParameter("end_date", endDate.ToString("yyyy-MM-dd"));
            if (page != null)
            {
                request.AddParameter("page", page);
            }

            restClient.Authenticator = new HttpBasicAuthenticator(username, password);

            var result = restClient.Execute(request);

            if (result.StatusCode!= HttpStatusCode.OK)
            {
                return InternalServerError();
            }
            
            return Ok(result.Content);
        }
    }
}