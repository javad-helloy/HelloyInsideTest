using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using Inside.AcceptToken;
using InsideModel.Models;
using InsideModel.repositories;

namespace Task.Email.NotificationEmail
{
    public class NotificationEmailDefenitionBuilder : INotificationEmailDefenitionBuilder
    {
        
        private readonly IRepository<InsideUser> adminRepository;
        
        private readonly IRepository<Contact> contactRepository;
        private readonly IAccessTokenProvider accessTokenProvider;
        
        public NotificationEmailDefenitionBuilder(
            IRepository<InsideUser> adminRepository,
            
            IRepository<Contact> contactRepository,
            IAccessTokenProvider accessTokenProvider)
        {
            
            this.adminRepository = adminRepository;
            
            this.contactRepository = contactRepository;
            this.accessTokenProvider = accessTokenProvider;
        }

        public NotificationEmailDefintion GetDefinition(int contactId, string userId)
        {
            var contact = contactRepository.Where(c => c.Id == contactId)
                .Include(l => l.Client)
                .Include(l => l.Client.InsideUserSets);

            var client = contact.First().Client;

            var insideUser = client.InsideUserSets.FirstOrDefault(u => u.Id == userId && (bool) u.ReceiveEmail);

            if (insideUser == null)
            {
                return null;
            }
            var consultantForUser =
                adminRepository.Where(
                    a => a.Role.Any(r => r.Name == "consultant") && a.ConsultantsForClients.Any(c => c.Id == client.Id))
                    .SingleOrDefault();

            var defenition = new NotificationEmailDefintion();

            var isValidEmailAdress = new Regex(@"^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}$", RegexOptions.IgnoreCase);
            if (isValidEmailAdress.IsMatch(insideUser.Email))
            {
                defenition.Type = contact.First().LeadType;
                defenition.UserName = client.Name;
                var contactDetails = GetContactDetails(insideUser.Id, contact.First());
                defenition.UserEmail = insideUser.Email;

                var clientLocalTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard Time");

                var utcDateTime = contact.First().Date;
                utcDateTime = DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc);

                var localDateTime =
                    TimeZoneInfo.ConvertTime(utcDateTime, clientLocalTimeZoneInfo).ToString("yyyy-MM-dd HH:mm");

                defenition.Date = localDateTime;
                defenition.InsideUrl = contactDetails.InsideUrl;
                defenition.From = contactDetails.From;
                defenition.SubjectOrDuration = contactDetails.SubjectOrDuration;

                if (consultantForUser != null)
                {
                    defenition.ConsultantEmail = consultantForUser.Email;
                    defenition.ConsultantPhone = consultantForUser.Phone;
                    defenition.ConsultantName = consultantForUser.Name;
                    if (consultantForUser.ImageUrl != null && !consultantForUser.ImageUrl.Contains("http://"))
                    {
                        defenition.ConsultantImage = "http://inside.helloy.se" + consultantForUser.ImageUrl;
                    }
                    else
                    {
                        defenition.ConsultantImage = consultantForUser.ImageUrl;
                    }
                }
                return defenition;
            }
            else
            {
                throw new Exception("User meail adress Was Invalid");
            }
        }

        private ContactDetails GetContactDetails(string userId, Contact contact)
        {
            var contactDefenition = new ContactDetails();
           if (contact.LeadType == "Phone")
            {
                var callerNumberProperty = contact.Property.SingleOrDefault(lp => lp.Type == "CallerNumber" && lp.ContactId == contact.Id);
                if (callerNumberProperty != null)
                    contactDefenition.From =
                        callerNumberProperty.Value;
                var durationProperty = contact.Property.SingleOrDefault(lp => lp.Type == "Duration" && lp.ContactId == contact.Id);
                if (durationProperty != null)
                {
                    var duration = durationProperty.Value;
                    if (!string.IsNullOrEmpty(duration))
                    {
                        contactDefenition.SubjectOrDuration = TimeSpan.FromSeconds(int.Parse(duration)).Minutes + ":" +
                                                              TimeSpan.FromSeconds(int.Parse(duration)).Seconds;
                    }
                    else
                    {
                        contactDefenition.SubjectOrDuration = "0:0";
                    }
                }
            }
            else if (contact.LeadType == "Email")
            {
                var fromEmailProperty = contact.Property.SingleOrDefault(lp => lp.Type == "FromEmail" && lp.ContactId == contact.Id);
                if (fromEmailProperty != null)
                    contactDefenition.From =
                        fromEmailProperty.Value;
                var subjectProperty = contact.Property.SingleOrDefault(lp => lp.Type == "Subject" && lp.ContactId == contact.Id);
                if (subjectProperty != null)
                {
                    var subject = subjectProperty.Value;
                    contactDefenition.SubjectOrDuration = subject;
                }
            }
            else if (contact.LeadType == "Chat")
            {
                var fromValues = new List<string>();
                var phoneProperty = contact.Property.SingleOrDefault(lp => lp.Type == "Phone" && lp.ContactId == contact.Id);
                if (phoneProperty != null)
                    fromValues.Add(phoneProperty.Value);
                var emailProperty = contact.Property.SingleOrDefault(lp => lp.Type == "Email" && lp.ContactId == contact.Id);
                if (emailProperty != null)
                    fromValues.Add(emailProperty.Value);

                contactDefenition.From = string.Join(", ", fromValues);
            }

           contactDefenition.InsideUrl = accessTokenProvider.GenerateAccessUrl(userId,
                   "/report/"+contact.ClientId+"/contact/" + contact.Id);
            contactDefenition.Date = contact.Date.ToString("yyyy-MM-dd HH:mm");
            return contactDefenition;
        }
    }
}
