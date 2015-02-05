using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Web.Mvc;
using System.Web.Security;
using Inside;
using Inside.AcceptToken;
using Inside.AutoRating;
using Inside.ContactProductService;
using Inside.membership;
using InsideModel.Models;
using InsideModel.repositories;
using InsideReporting.Helpers;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using Twilio;


namespace InsideReporting.Controllers
{

    public class DevController : AuthenticationController
    {
        private readonly InsideContext context;

        public DevController(InsideContext context, IIdentityMembershipProvider userManager): base(userManager)
        {
            this.context = context;
        }

        //
        // GET: /Dev/
        public ActionResult CheckDB()
        {
            var createdDatabase = context.Database.CreateIfNotExists();
            context.Database.Initialize(true);
            return Json(createdDatabase, JsonRequestBehavior.AllowGet);
        }
        
        [Authorize(Roles = "consultant")]
        public ActionResult AddUser()
        {
            try
            {
                if (Membership.GetUser("Jakob") == null)
                {
                    /*var newUserMembership = Membership.CreateUser("Jakob", "helloy123", "jakob.aronsson@helloy.se");
                    newUserMembership.IsApproved = true;
                    Roles.AddUserToRole("Jakob", "sales");

                    var newUser = new InsideUser()
                    {
                        AdminRole = "AccountManager",
                        Email = "jakob.aronsson@helloy.se",
                        Name = "Jakob Aronsson",
                        MembershipProviderId =  (newUserMembership.ProviderUserKey.ToString()),
                        ImageUrl = "http://inside.helloy.se/Content/images/Employees/Jakob.jpg",
                        Phone = "0701-99 491"
                    };

                    context.Admin.Add(newUser);
                    context.SaveChanges();

                    return Json(newUser, JsonRequestBehavior.AllowGet);*/
                }
                return Json("User Already in DB ", JsonRequestBehavior.AllowGet);
            }
            catch (MembershipCreateUserException e)
            {
               return Json("User Not Created because: "+e, JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize(Roles = "consultant")]
        public ActionResult sampleSend()
        {
            var jsonCoverter = new JsonUtcConverter();

            var testClass = new SimpleTestClassForJsonNetConverterTest();

            testClass.Date = new DateTime(2013, 1, 2, 3, 4, 5);
            testClass.Lead = 250;
            
            return Content(jsonCoverter.Serilize(testClass), "application/json");
        }

        public string GetShortURL(string longUrl)
        {
            var jsonCoverter = new JsonUtcConverter();
            WebRequest request = WebRequest.Create("https://www.googleapis.com/urlshortener/v1/url?key=AIzaSyCdO7g2yHUU_CyufRND_MWvQmPubImrb2g");
            request.Method = "POST";
            request.ContentType = "application/json";
            var jsonData = jsonCoverter.Serilize(new GoogleUrlShortnerMessage{longUrl = longUrl});
            string requestData = jsonData;
            byte[] requestRawData = Encoding.ASCII.GetBytes(requestData);
            request.ContentLength = requestRawData.Length;
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(requestRawData, 0, requestRawData.Length);
            requestStream.Close();

            WebResponse response = request.GetResponse();
            StreamReader responseReader = new StreamReader(response.GetResponseStream());
            string responseData = responseReader.ReadToEnd();
            responseReader.Close();

            var shortnerResult = JsonConvert.DeserializeObject<GoogleUrlShortnerResponse>(responseData);
            return shortnerResult.id;
        }

        [Authorize(Roles = "consultant")]
        public ActionResult SendSms()
        {
            var jsonCoverter = new JsonUtcConverter();

            var tokenRepository = new Repository<Token>(db=>context.Token, context);
            var userRepository = new Repository<InsideUser>(db => context.InsideUser, context);
            var accessTokenProvider = new AccessTokenProvider(tokenRepository,userRepository);
            var url = accessTokenProvider.GenerateAccessUrl(User.Identity.GetUserId(), "/report/" + 10 + "/contact/" + 111818);


            var shortUrl = GetShortURL(url);


            string AccountSid = "ACf527710731ad6e3852be97b937c418b5";
            string AuthToken = "51653777a7b97d349e0da8cb966359a1";
            var twilio = new TwilioRestClient(AccountSid, AuthToken);
            var message = twilio.SendMessage("+46769438884", "0722524969", "Du har fått ett nytt telefonsamtal från 0722524969: " + shortUrl);
            return Content(jsonCoverter.Serilize(message), "application/json");
        }

        [Authorize(Roles = "consultant")]
        public ActionResult SetAutoRating()
        {
            var contactAutoRating = new ContactAutoRating();
            var contactRepository = new Repository<Contact>(db => context.Lead, context);
            foreach (var contact in contactRepository.Where(c=>c.Property.All(cp => cp.Type != "AutoRatingScore")).Take(100))
            {
                contactAutoRating.SetAutoRating(contact);
            }
            contactRepository.SaveChanges();
            var autoRatingInDb = contactRepository.Where(c => c.Property.Any(cp => cp.Type == "AutoRatingScore")).Count();
            return Content("Ok, number of auto rating so far:"+ autoRatingInDb, "application/json");
        }

        public class GoogleUrlShortnerResponse
        {
            public string kind { get; set; }
            public string id { get; set; }
            public string longUrl { get; set; }
        }
        public class GoogleUrlShortnerMessage
        {
            public string longUrl { get; set; }
        }
        [Authorize(Roles = "consultant")]
        public ActionResult UpdateSearchProductToOrganic()
        {
            try
            {
                var productService = new ProductService();
                var organicCount = 0;
                var contactsWithSearchProduct = context.Lead.Where(l=> l.Property.Any(lp => lp.Type == "Product" && lp.Value == "Helloy Search") );
                foreach (var contact in contactsWithSearchProduct)
                {
                    
                    if (!contact.Property.Any(c=>c.Type == "Medium")) continue;

                    var contactMedium = contact.Property.Single(c => c.Type == "Medium").Value;

                    if (productService.IsOrganic(contactMedium))
                    {
                        organicCount++;
                        contact.SetPropertyValue("Product", productService.GetProduct("",contactMedium));
                    }
                }
                context.SaveChanges();
                return Json("Out of "+contactsWithSearchProduct.Count()+" Helloy Search Products, Updated "+organicCount+" With Organic", JsonRequestBehavior.AllowGet);
            }
            catch (MembershipCreateUserException e)
            {
                return Json("Erro in Update: " + e, JsonRequestBehavior.AllowGet);
            }
        }


        [Authorize(Roles = "consultant")]
        public ActionResult TestBlock()
        {
            return View();
        }

        [AuthorizeClientAccess]
        public ActionResult LongCall(int callid, int clientId)
        {
            Thread.Sleep(3000);
            return Json("Waited 3 seconds", JsonRequestBehavior.AllowGet);
        }
    }
    internal class SimpleTestClassForJsonNetConverterTest
    {
        public DateTime Date { get; set; }
        public int Lead { get; set; }
    }
}
