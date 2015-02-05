using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Filters;
using Inside.AcceptToken;
using Inside.membership;
using Inside.Time;
using InsideModel.Models;
using InsideModel.repositories;
using Ninject.Infrastructure.Language;
using ActionFilterAttribute = System.Web.Http.Filters.ActionFilterAttribute;

namespace InsideReporting.Controllers.Api
{
    
    public class ExternalController : ApiController
    {
        private IRepository<InsideUser> userRepository;
        private IRepository<Contact> contactRepository;
        private IAccessTokenProvider tokenGenerator;
        private IIdentityMembershipProvider userManager;
        private readonly IRepository<Budget> budgetRepository;
        private readonly IRepository<Client> clientRepository;
        private readonly IServerTime serverTime;

        public ExternalController(
            IRepository<InsideUser> userRepository,
            IRepository<Contact> contactRepository,
            IAccessTokenProvider tokenGenerator,
            IIdentityMembershipProvider userManager,
            IRepository<Budget> budgetRepository,
            IRepository<Client> clientRepository,
            IServerTime serverTime)
        {
            this.userRepository = userRepository;
            this.contactRepository = contactRepository;
            this.tokenGenerator = tokenGenerator;
            this.userManager = userManager;
            this.budgetRepository = budgetRepository;
            this.clientRepository = clientRepository;
            this.serverTime = serverTime;
        }


        //
        // GET: api/client/External/accesstoken
        [Route("api/external/accesstoken")]
        [AllowCrossSiteJson]
        public IHttpActionResult GetAccessToken(string username, string password)
        {
            var user = userManager.ValidateAndReturnUser(username, password);

            if (user != null && !user.IsLockedOut)
            {
                userManager.Lock(user.Id, false);
                var userId = user.Id;
                var clientId = user.ClientId;
                var newToken = tokenGenerator.GetToken(userId);

                var accessTokenForClient = new AcccessTokenForClient(newToken, (int) clientId);

                return Ok(accessTokenForClient);                    
            }
            else
            {
                return Unauthorized();
            }
        }

        /*[Route("api/external/user/{userId}/contact")]*/
        [Route("api/external/client/{clientId}/contact")]
        [AllowCrossSiteJson]
        public IHttpActionResult GetContactList(int clientId, string accessToken, int take, int skip)
        {
            var isAccessTokenValid = tokenGenerator.Validate(clientId, accessToken);

            if (isAccessTokenValid)
            {
                var contactList = contactRepository.Where(
                        contact => contact.Client.Id==clientId)
                    .OrderByDescending(l => l.Date)
                    .Skip(skip).Take(take)
                    .AsNoTracking()
                    .Include(l => l.Interaction)
                    .Include(l => l.Property);

                return Ok(contactList.ToList());
            }
            else
            {
                return Unauthorized();
            }
        }

        /*[Route("api/external/user/{userId}/interaction")]*/
        [Route("api/external/client/{clientId}/contact/{contactId}/interaction")]
        [AllowCrossSiteJson]
        [HttpPost]
        public IHttpActionResult SetContactInteraction(int clientId, string accessToken, int contactId, Interaction interaction)
        {
            var isAccessTokenValid = tokenGenerator.Validate(clientId, accessToken);

            if (isAccessTokenValid)
            {
                var matchedContact = contactRepository.Where(contact => contact.Id == contactId).Include(contact => contact.Interaction).SingleOrDefault();

                if (matchedContact == null)
                {
                    return NotFound();
                }

                if (matchedContact.Interaction.Any(lp => lp.Type == interaction.Type))
                {
                    matchedContact.Interaction.First(lp => lp.Type == interaction.Type).Value = interaction.Value;

                }
                else
                {
                    matchedContact.Interaction.Add(new ContactInteraction() { Type = interaction.Type, ContactId = contactId, Value = interaction.Value });
                }

                contactRepository.SaveChanges();
                return Ok(matchedContact);
            }

            else
            {
                return Unauthorized();
            }
        }


        /*[Route("api/external/user/{userId}/contact")]*/
        [Route("api/external/v{versionNumber}/check")]
        [AllowCrossSiteJson]
        public IHttpActionResult GetVersionCheck(string versionNumber)
        {
            string[] blackListVersions = {};
            string[] grayListVersions = { "1.0", "1.0.1" };
            string currentVersion = "1.0.2";
            var messageToSend = new ApiValidationMessage();
            if (versionNumber.Equals(currentVersion))
            {
                messageToSend.Status = 100;
                messageToSend.Message = "Current";
                return Ok(messageToSend);
            }
            if (grayListVersions.Contains(versionNumber))
            {
                messageToSend.Status = 200;
                messageToSend.Message = "Helloy GO har nu blivit ännu bättre!";
                return Ok(messageToSend);
            }
            if (blackListVersions.Contains(versionNumber))
            {
                messageToSend.Status = 300;
                messageToSend.Message = "Din versions av Inside GO stöds ej längre. Uppgradera för att fortsätta använda.";
                return Ok(messageToSend);
            }

            messageToSend.Status = 400;
            messageToSend.Message = "Invalid Version";
            return Ok(messageToSend);
        }

        [Route("api/external/BudgetSummary")]
        [AllowCrossSiteJson]
        [HttpGet]
        public IHttpActionResult GetBudgetSummary(string accessToken, string am = null)
        {
            var isConsultant =
                userRepository.Any(
                    u => u.Token.Any(t => t.AccessToken == accessToken) && u.Role.Any(r => r.Name == "consultant"));

            if (!isConsultant)
            {
                return Unauthorized();
            }

            var now = serverTime.RequestStarted;
            var month = new DateTime(now.Year, now.Month, 1);
            var nextMonth = month.AddMonths(1);

            int numberOfClients;
            int numberOfKPedClients;

            if (String.IsNullOrEmpty(am))
            {
                numberOfClients = clientRepository.Where(c => c.IsActive && c.Labels.Any(l => l.Name == "Kampanjplaneras")).Count();
                numberOfKPedClients = clientRepository.Where(c => c.IsActive && c.Labels.Any(l => l.Name == "Kampanjplaneras") && c.Budgets.Any(b => b.Month == nextMonth)).Count();
            }
            else
            {
                numberOfClients = clientRepository.Where(c => c.AccountManager.Name == am && c.IsActive && c.Labels.Any(l => l.Name == "Kampanjplaneras")).Count();
                numberOfKPedClients = clientRepository.Where(c => c.AccountManager.Name == am && c.IsActive && c.Labels.Any(l => l.Name == "Kampanjplaneras") && c.Budgets.Any(b => b.Month == nextMonth)).Count();
            }

            var model = new BudgetMonthSummary();
            model.month = month;
            model.TotalClients = numberOfClients;
            model.numClients = numberOfKPedClients;
            var nextMonthsBudget = budgetRepository.Where(b => b.Month == nextMonth && b.Client.Consultant.Name == am);
            if (nextMonthsBudget.Any())
            {
                model.value = nextMonthsBudget.Sum(b => b.Value);
            }
            else
            {
                model.value = 0;
            }

            decimal diff = 0;
            IEnumerable<Client> clientsWithBudgetsBothMonths;
            if (String.IsNullOrEmpty(am))
            {
                clientsWithBudgetsBothMonths =
                    clientRepository.Where(
                        c => c.Budgets.Any(b => b.Month == month) && c.Budgets.Any(b => b.Month == nextMonth))
                        .Include(c => c.Budgets)
                        .ToEnumerable();
            }
            else
            {
                clientsWithBudgetsBothMonths = 
                    clientRepository.Where(
                        c => c.AccountManager.Name == am && c.Budgets.Any(b => b.Month == month) && c.Budgets.Any(b => b.Month == nextMonth))
                        .Include(c => c.Budgets)
                        .ToEnumerable();
            }
            
            foreach (var client in clientsWithBudgetsBothMonths)
            {
                var diffForClient = client.Budgets.First(b => b.Month == nextMonth).Value -
                                    client.Budgets.First(b => b.Month == month).Value;
                diff =  diff + diffForClient;
            }
            model.diff = diff;
            
            return Ok(model);
        }
    }

    public class BudgetSummary
    {
        public BudgetSummary()
        {
            BudgetMonths = new List<BudgetMonthSummary>();

        }
        public IList<BudgetMonthSummary> BudgetMonths { get; private set; }
        public int TotalClients { get; set; }
    }

    public class BudgetMonthSummary
    {
        public BudgetMonthSummary(){}

        public BudgetMonthSummary(
            DateTime month, 
            decimal value, 
            int numClients, 
            int totalClients,
            decimal diff)
        {
            this.month = month;
            this.value = value;
            this.numClients = numClients;
            this.TotalClients = totalClients;
            this.diff = diff;
        }
        public int TotalClients { get; set; }
        public DateTime month { get; set; }
        public decimal value { get; set; }
        public decimal diff { get; set; }
        public int numClients { get; set; }
    }


    public class Interaction
    {
        public string Type { get; set; }
        public string Value { get; set; }
    }

    public class ApiValidationMessage
    {
        public int Status { get; set; }
        public string Message { get; set; }
    }

    public class AcccessTokenForClient : Token
    {
        public AcccessTokenForClient(Token token, int clientId)
        {
            ClientId = clientId;
            AccessToken = token.AccessToken;
            ExpirationDate = token.ExpirationDate;
            Id = token.Id;
            UserId = token.UserId;
        }

        public int ClientId { get; set; }
    }

    public class AllowCrossSiteJsonAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext.Response != null && !actionExecutedContext.Response.Headers.Contains("Access-Control-Allow-Origin"))
                actionExecutedContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");

            base.OnActionExecuted(actionExecutedContext);
        }
    }
}
