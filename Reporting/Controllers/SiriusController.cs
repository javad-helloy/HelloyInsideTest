using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using Inside.membership;
using InsideModel.Models;
using InsideModel.repositories;
using InsideReporting.Helpers;
using InsideReporting.Models.Contact;
using Microsoft.AspNet.Identity;

namespace InsideReporting.Controllers
{
    public class SiriusController : AuthenticationController
    {
        private readonly IRepository<Client> clientRepository;
        private readonly IIdentityMembershipProvider userManager;

        public SiriusController(IRepository<Client> clientRepository, IIdentityMembershipProvider userManager)
            : base(userManager)
        {
            this.clientRepository = clientRepository;
            this.userManager = userManager;
        }

        // GET: Sirius

        [Authorize(Roles = "demo,sales,consultant,client")]
        [AuthorizeClientAccess]
        public ActionResult Overview(int clientId )
        {
            var client = clientRepository.Single(c => c.Id == clientId);
            var model = new SiriusReportModel();

            model.ClientId = client.Id;
            model.ClientName = client.Name;
            model.UserId = this.User.Identity.GetUserId();
            model.HasCustomEvents = client.Labels.Any(cl => cl.Name == "Custom Event");
            model.HasAdminMenu = this.User.IsInRole("sales") || this.User.IsInRole("consultant");
            model.HasWebTab = client.Labels.Any(l => l.Name == "Webbflik");

            return View(model);
        }

        [Authorize(Roles = "demo,sales,consultant,client")]
        [AuthorizeClientAccess]
        public ActionResult ContactList(int clientId)
        {
            var model = new SiriusReportModel();
            var client = clientRepository.Single(c => c.Id == clientId);
            model.ClientId = client.Id;
            model.ClientName = client.Name;
            model.UserId = this.User.Identity.GetUserId();
            model.HasCustomEvents = client.Labels.Any(cl => cl.Name == "Custom Event");
            model.HasAdminMenu = this.User.IsInRole("sales") || this.User.IsInRole("consultant");
            model.HasWebTab = client.Labels.Any(l => l.Name == "Webbflik");
            return View(model);
        }

        [Authorize(Roles = "demo,sales,consultant,client")]
        [AuthorizeClientAccess]
        public ActionResult WebTab(int clientId)
        {
            var model = new SiriusReportModel();
            var client = clientRepository.Single(c => c.Id == clientId);
            model.ClientId = client.Id;
            model.ClientName = client.Name;
            model.UserId = this.User.Identity.GetUserId();
            model.HasAdminMenu = this.User.IsInRole("sales") || this.User.IsInRole("consultant");
            model.HasWebTab = client.Labels.Any(cl => cl.Name == "Webbflik");
            return View(model);
        }

        [Authorize(Roles = "demo,sales,consultant,client")]
        [AuthorizeClientAccess]
        public ActionResult Campaign(int clientId)
        {
            var model = new SiriusReportModel();
            var client = clientRepository.Single(c => c.Id == clientId);
            model.ClientId = client.Id;
            model.ClientName = client.Name;
            model.UserId = this.User.Identity.GetUserId();
            model.HasAdminMenu = this.User.IsInRole("sales") || this.User.IsInRole("consultant");
            model.HasWebTab = client.Labels.Any(l => l.Name == "Webbflik");
            return View(model);
        }

        [Authorize(Roles = "demo,sales,consultant,client")]
        [AuthorizeClientAccess]
        public ActionResult Contact(int clientId, int contactId)
        {
            var model = new ContactViewModel();
            var client = clientRepository.Single(c => c.Id == clientId);
            model.ClientId = client.Id;
            model.ClientName = client.Name;
            model.UserId = this.User.Identity.GetUserId();
            model.ContactId = contactId;
            model.HasAdminMenu = this.User.IsInRole("sales") || this.User.IsInRole("consultant");
            model.HasWebTab = client.Labels.Any(l => l.Name == "Webbflik");
            return View(model);
        }

        [Authorize(Roles = "demo,sales,consultant,client")]
        [AuthorizeClientAccess]
        public ActionResult MyAccount(int clientId)
        {
            var userId = User.Identity.GetUserId();
            var user = userManager.FindById(userId);
            var client = clientRepository.Single(c => c.Id == clientId);

            var model = new SiriusAccountModel();
            model.ClientId = clientId;
            model.ClientName = client.Name;
            model.UserId = this.User.Identity.GetUserId();
            model.ReceiveEmail = user.ReceiveEmail != null && (bool)user.ReceiveEmail;
            model.ReceiveSms = user.ReceiveSms != null && (bool)user.ReceiveSms;
            model.Phone = user.Phone;
            model.HasAdminMenu = this.User.IsInRole("sales") || this.User.IsInRole("consultant");
            model.HasWebTab = client.Labels.Any(l => l.Name == "Webbflik");

            return View(model);
        }

        [Authorize(Roles = "sales,consultant,client")]
        [AuthorizeClientAccess]
        [HttpPost]
        public ActionResult MyAccount(SiriusAccountModel model)
        {
            var updateMessage = new SettingUpdateMessage();
            if (ModelState.IsValid)
            {
                var userId = User.Identity.GetUserId();
                var user = userManager.FindById(userId);

                model.ReceiveEmail = user.ReceiveEmail != null && (bool)user.ReceiveEmail;
                model.ReceiveSms = user.ReceiveSms != null && (bool)user.ReceiveSms;
                model.Phone = user.Phone;

                var validUser = userManager.ValidateAndReturnUser(user.UserName, model.OldPassword);
                if (validUser != null)
                {
                    var succeed = userManager.UpdatePassword(validUser.Id, model.Password);
                    if (succeed)
                    {
                        updateMessage.Type = AccountMessageType.Success;
                        updateMessage.MessageText = "Uppdaterade lösenordet";
                    }
                    else
                    {
                        updateMessage.Type = AccountMessageType.Error;
                        updateMessage.MessageText = "Lösenordet uppdaterades inte";
                    }
                }
                else
                {

                    updateMessage.Type = AccountMessageType.Error;
                    updateMessage.MessageText = "Felaktigt lösenord";
                }
                model.Message = updateMessage;
                return View(model);
            }

            updateMessage.Type = AccountMessageType.Error;
            updateMessage.MessageText = "Lösenordet uppdaterades inte";
            model.Message = updateMessage;
            return View(model);
        }

        [Authorize(Roles = "sales,consultant,client")]
        [AuthorizeClientAccess]
        [HttpPost]
        public ActionResult UpdateSettings(bool receiveEmail, bool receiveSms, string phone)
        {
            var userId = User.Identity.GetUserId();
            var user = userManager.FindById(userId);

            var updateMessage = new SettingUpdateMessage();
            if (user != null)
            {
                user.ReceiveEmail = receiveEmail;
                user.ReceiveSms = receiveSms;
                user.Phone = phone;
                userManager.Update(user);
                updateMessage.Type = "Success";
                updateMessage.MessageText = "Uppdaterade inställningar";
                return Json(updateMessage, JsonRequestBehavior.AllowGet);
            }

            updateMessage.Type = "Error";
            updateMessage.MessageText = "Oväntat fel";
            return Json(updateMessage, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "demo,sales,consultant")]
        [AuthorizeClientAccess]
        public ActionResult Demo()
        {
            return RedirectToAction("ContactList", new { clientId = 1 });
        }
    }

    public class SiriusReportModel
    {
        public int ClientId { get; set; }
        public string ClientName { get; set; }
        public string UserId { get; set; }
        public bool HasCustomEvents { get; set; }
        public bool HasAdminMenu { get; set; }
        public bool HasWebTab { get; set; }
    }

    public class SiriusAccountModel : SiriusReportModel
    {
        public SettingUpdateMessage Message { get; set; }

        [Display(Name = "Telefonnumber")]
        public string Phone { get; set; }

        [Display(Name = "Jag vill ha e-post")]
        public bool ReceiveEmail { get; set; }

        [Display(Name = "Jag vill ha SMS")]
        public bool ReceiveSms { get; set; }

        [Required(ErrorMessage = "Du måste ange ditt gamla lösenord")]
        [DataType(DataType.Password)]
        [Display(Name = "Gammalt lösenord")]
        public string OldPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Nytt lösenord")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Bekräfta ditt lösenord")]
        [System.Web.Mvc.Compare("Password", ErrorMessage = "Lösenorden stämmer inte överens.")]
        public string ConfirmPassword { get; set; }
    }

    public class SettingUpdateMessage 
    {
        public string Type { get; set; }
        public string MessageText { get; set; }
    }

    public static class AccountMessageType
    {
        public static string Success = "Success";
        public static string Error = "Error";
    }

}