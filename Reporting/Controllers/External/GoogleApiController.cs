using System.Collections.Generic;
using System.Web.Mvc;
using Inside;
using Inside.GoogleService;
using Mandrill;
using Task.Email.Sender;

namespace InsideReporting.Controllers.External
{
    [Authorize(Roles = "consultant")]
    public class GoogleApiController : Controller
    {
        private readonly IGoogleAuthentication googleAuthentication;
        private readonly IJsonConverter jsonConverter;
       
        public GoogleApiController(IGoogleAuthentication googleAuthentication, IJsonConverter jsonConverter)
        {
            this.googleAuthentication = googleAuthentication;
            this.jsonConverter = jsonConverter;
        }

        // GET: GoogleApi
        public ActionResult Index()
        {
            return View();
        }

        // GET: GoogleApi
        public ActionResult OutPut(string message)
        {
            var model = new GoogleApiCodeResponse {Response = message};

            return View(model);
        }

        // Post: GoogleApi
        
        public ActionResult GetToken(string  code)
        {
            var refreshToken = googleAuthentication.GetRefreshToken(code, "http://inside.helloy.se/GoogleApi/CodeCallback");
            return Content(jsonConverter.Serilize(refreshToken));

        }

        // GET: GoogleApi
        [HttpGet]
        public ActionResult CodeCallback(string code)
        {
            return RedirectToAction("OutPut", new { message = code });
        }
        //Not Called
        [HttpPost]
        public ActionResult CodeCallback(GoogleApiTokenResponse data)
        {
            var emailSender = new MandrilEmailSender();

            var emailMessage = new EmailMessage();

            emailMessage.from_email = "javad.bakhshi@helloy.se";
            List<EmailAddress> emailAddresses = new List<EmailAddress>();
            var emailAddress = new EmailAddress();
            emailAddress.email = "javad.bakhshi@helloy.se";
            emailAddresses.Add(emailAddress);
            emailMessage.to = emailAddresses;
            emailMessage.text = "access_token = " + data.access_token;
            emailSender.Send(emailMessage);

            return Content("Ok");
        }

        // GET: GoogleApi
        public ActionResult Success(string message)
        {
            var model = new GoogleApiCodeResponse { Response = message };

            return View(model);
        }
        // GET: GoogleApi
        public ActionResult GetCode()
        {
            var url = googleAuthentication.GetAuthorizationCodeUrl("http://inside.helloy.se/GoogleApi/CodeCallback");

            return Redirect(url);
        }

    }

    public class GoogleApiCodeResponse
    {
        public string Response { get; set; }
    }
}