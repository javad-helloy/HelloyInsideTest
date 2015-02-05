using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Http;
using Inside.Time;
using InsideModel.Models;
using InsideModel.repositories;
using Mandrill;
using Microsoft.AspNet.Identity;
using Task.Email.Sender;

namespace InsideReporting.Controllers.External
{
    public class SocialController : ApiController
    {
        private readonly IRepository<Contact> contactRepository;
        private readonly IRepository<InsideUser> userRepository;
        private readonly IEmailSender emailSender;
        private readonly IServerTime serverTime;

        public SocialController(
            IRepository<Contact> contactRepository, 
            IRepository<InsideUser> userRepository,
            IEmailSender emailSender, 
            IServerTime serverTime )
        {
            this.contactRepository = contactRepository;
            this.userRepository = userRepository;
            this.emailSender = emailSender;
            this.serverTime = serverTime;
        }

        [Authorize(Roles = "consultant,client")]
        [HttpGet]
        [Route("api/social/sendcontact")]
        public IHttpActionResult SendContact(int contactId, string emailAddress, string customerContact, string comment)
        {
            var id = this.User.Identity.GetUserId();
            var fromUser = userRepository.Single(u => u.Id == id);

            var contact = contactRepository.Where(c => c.Id == contactId).SingleOrDefault();

            if (contact == null || contact.Id != contactId)
            {
                return NotFound();
            }

            var isConsultant = User.IsInRole("consultant");

            var hasAccessToClient = false;
            hasAccessToClient = hasAccessToClient || isConsultant;
            var isUserForCurrentClient = contact.ClientId == fromUser.ClientId;
            hasAccessToClient = hasAccessToClient || isUserForCurrentClient;

            if (!hasAccessToClient)
            {
                return Unauthorized();
            }

            var fromName = fromUser.UserName;

            if (!string.IsNullOrEmpty(fromUser.Name))
            {
                fromName = fromUser.Name;
            }

            EmailMessage emailMessage = new EmailMessage(); 
            emailMessage.from_email = fromUser.Email;

            List<EmailAddress> emailAddresses = new List<EmailAddress>();
            emailAddresses.Add(new EmailAddress(emailAddress));
            emailMessage.to = emailAddresses;

            emailMessage.subject = "Kundkontakt från " + fromName;
            if (comment != null)
            {
                comment = comment.Replace("\n", "<br/>");
            }
            var ci = new CultureInfo("sv-SE");
            DateTime dateInUserTime = serverTime.ConvertServerTimeToStandardUserTime(contact.Date);
            var dateString = "den " + dateInUserTime.ToString("dd MMMM yyyy HH:mm", ci);

            var messageHtml = "";
            messageHtml += fromName + " har delat en kundkontakt från Helloy med dig: <br/><br/>";
            messageHtml += "<strong>Kontakt</strong>";
            messageHtml += "<p>" + customerContact + " " + dateString + "</p>";
            messageHtml += "<br/><br/>";
            messageHtml += "<strong>Kommentar</strong>";
            messageHtml += "<p>"+comment+"</p>";

            emailMessage.html = messageHtml;

            emailSender.Send(emailMessage);
            return Ok();
        }
    }
}