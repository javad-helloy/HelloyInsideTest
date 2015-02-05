using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Cryptography;
using System.Web;
using System.Web.Razor.Generator;
using Inside.AutoRating;
using Inside.membership;
using InsideModel.Models;
using InsideModel.Models.Identity;
using InsideModel.repositories;
using InsideReporting.Service.Anonymize;
using Microsoft.AspNet.Identity;

namespace InsideReporting.Service.DemoWrapper
{
    public class AnonymisedContactRepositorySirius : Repository<Contact>
    {
        private readonly IAnonymizedDataHelper dataHelper;
        private readonly Random randomGenerator;
        private readonly IIdentityMembershipProvider userManager;
        private readonly IList<int> contactCountList ;
        private int generatedId;


        public AnonymisedContactRepositorySirius(
            IAnonymizedDataHelper dataHelper, IIdentityMembershipProvider userManager)
            : base(context => context.Lead)
        {
            this.dataHelper = dataHelper;
            randomGenerator = new Random(10);
            this.userManager = userManager;
            this.contactCountList = new List<int>{107,95,89,84,92,12,61,55,45,50,45,40,40,36,37,19,10,12,14,8,6,3,0,1};
            generatedId = 0;

        }

        public override IQueryable<Contact> Where(Expression<Func<Contact, bool>> expression)
        {
            var isAuthenticated = HttpContext.Current.User.Identity.IsAuthenticated;
            var isBasicAuthenticationForTaskCreating = HttpContext.Current.User.Identity.AuthenticationType == "BasicAuthenticationForTaskCreating";

            if (isAuthenticated && !isBasicAuthenticationForTaskCreating && userManager.IsInRole(HttpContext.Current.User.Identity.GetUserId(), "demo"))
            {
                return GenerateRandomContacts();
            }
            else
            {
                return base.Where(expression);
            }
        }

        private IQueryable<Contact> GenerateRandomContacts()
        {
           
            var endDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month , 1).AddMonths(1).AddDays(-1);
            var startDate = endDate.AddDays(1).AddMonths(-1);

            var leads = new List<Contact>();
            for (int i = 0; i < contactCountList.Count(); i++)
            {
                for (int j = 0; j < contactCountList[i]; j++)
                {
                    if (randomGenerator.NextDouble() > 0.4)
                    {
                        Contact randomPhoneCall = GetRandomPhoneCall(startDate,endDate);
                        randomPhoneCall.Id = generatedId++;
                        SetInteraction(randomPhoneCall);
                        SetProductProperty(randomPhoneCall);
                        SetAutoRating(randomPhoneCall);
                        leads.Add(randomPhoneCall);
                    }
                    else if (randomGenerator.NextDouble() > 0.5)
                    {
                        Contact randomMail = GetRandomMail(startDate, endDate);
                        randomMail.Id = generatedId++;
                        SetInteraction(randomMail);
                        SetProductProperty(randomMail);
                        SetAutoRating(randomMail);
                        leads.Add(randomMail);
                    }
                    else
                    {
                        Contact randomChat = GetRandomChat(startDate, endDate);
                        randomChat.Id = generatedId++;
                        SetInteraction(randomChat);
                        SetProductProperty(randomChat);
                        SetAutoRating(randomChat);
                        leads.Add(randomChat);
                    }
                }
                endDate = endDate.AddMonths(- 1);
                startDate = startDate.AddMonths(-1);
            }

            return leads.AsQueryable();
           
        }
        private void SetInteraction(Contact lead)
        {

            if (randomGenerator.NextDouble() > 0.7)
            {
                lead.RatingScore =3;
            }
            else if (randomGenerator.NextDouble() > 0.7)
            {
                lead.RatingScore = 4;
            }
            else if (randomGenerator.NextDouble() > 0.7)
            {
                lead.RatingScore = 5;
            }
        }

        private void SetAutoRating(Contact lead)
        {

            if (randomGenerator.NextDouble() > 0.5)
            {
                lead.AutoRatingScore = 3;
            }
            else if (randomGenerator.NextDouble() > 0.7)
            {
                lead.AutoRatingScore = 4;
            }
            else
            {
                lead.AutoRatingScore = 5;
            }
        }

        private void SetProductProperty(Contact lead)
        {
            if (randomGenerator.NextDouble() > 0.4)
            {
                lead.Product = "Search";
            }
            else if (randomGenerator.NextDouble() > 0.4)
            {
                lead.Product = "Retargeting";
            }
            else
            {
                lead.Product = "Display";
            }

        }

        private Contact GetRandomMail(DateTime startRangeDate, DateTime endRangeDate)
        {
            var mailContact = new Contact();
            mailContact.LeadType = "Email";
            mailContact.Date = dataHelper.NextRandomDateInIntevall(startRangeDate, endRangeDate);

            mailContact.Property.Add(new ContactProperty() { Type = "FromEmail", Value =dataHelper.NextEmailAdress() });
            mailContact.Property.Add(new ContactProperty() { Type = "Html", Value = dataHelper.NextEmailContent() });
            mailContact.Property.Add(new ContactProperty() { Type = "MandrillId", Value = 0.ToString() });
            mailContact.Property.Add(new ContactProperty() { Type = "State", Value ="sent" });
            mailContact.Property.Add(new ContactProperty() { Type = "Subject", Value = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(dataHelper.NextRandomSearchPhrase()) });
            mailContact.Property.Add(new ContactProperty() { Type = "Tags", Value = "" });
            mailContact.Property.Add(new ContactProperty() { Type = "Text", Value = "" });
            mailContact.Property.Add(new ContactProperty() { Type = "ToEmail", Value = "info@företag.se" });
            mailContact.Property.Add(new ContactProperty() { Type = "Updated", Value = mailContact.Date.ToString() });
            mailContact.SearchPhrase= dataHelper.NextRandomSearchPhrase();
            mailContact.AutoRatingScore = 3;

            return mailContact;
        }

        private Contact GetRandomPhoneCall(DateTime startRangeDate, DateTime endRangeDate)
        {
            var phoneContact = new Contact();
            phoneContact.LeadType = "Phone";
            phoneContact.Date = dataHelper.NextRandomDateInIntevall(startRangeDate, endRangeDate);

            phoneContact.Property.Add(new ContactProperty() { Type = "CTMId", Value = 1.ToString() });
            phoneContact.Property.Add(new ContactProperty() { Type = "Audio", Value = "/Content/audio/samplecall.mp3" });
            phoneContact.Property.Add(new ContactProperty() { Type = "BusinessNumber", Value = "+468123456" });
            phoneContact.Property.Add(new ContactProperty() { Type = "CallerNumber", Value = dataHelper.NextRandomPhoneNumber() });
            phoneContact.Property.Add(new ContactProperty() { Type = "City", Value = "" });
            phoneContact.Property.Add(new ContactProperty() { Type = "Country", Value = "SE" });
            phoneContact.Property.Add(new ContactProperty() { Type = "LocationUrl", Value = "/kontakt/" });
            phoneContact.Property.Add(new ContactProperty() { Type = "PostalCode", Value = "" });
            phoneContact.Property.Add(new ContactProperty() { Type = "ReferalUrl", Value = "https://www.google.se/" });
            phoneContact.Property.Add(new ContactProperty() { Type = "State", Value =  "Stockholm" });
            phoneContact.Property.Add(new ContactProperty() { Type = "Status", Value = "completed" });
            phoneContact.Property.Add(new ContactProperty() { Type = "TrackingNumber", Value = "+46101389158" });
            phoneContact.Property.Add(new ContactProperty() { Type = "CTMAccoutId", Value = 1.ToString() });
            phoneContact.Property.Add(new ContactProperty() { Type = "Duration", Value = dataHelper.NextRandomTimeSpan(60 * 9).Seconds.ToString() });
            phoneContact.SearchPhrase= dataHelper.NextRandomSearchPhrase() ;
            phoneContact.Source = dataHelper.NextRandomSearchSource();

            return phoneContact;
        }

        private Contact GetRandomChat(DateTime startRangeDate, DateTime endRangeDate)
        {
            var chatContact = new Contact();
            chatContact.LeadType = "Chat";
            chatContact.Date = dataHelper.NextRandomDateInIntevall(startRangeDate, endRangeDate);

            chatContact.Property.Add(new ContactProperty() { Type = "Description", Value = dataHelper.NextRandomChatDescription()});
            chatContact.Property.Add(new ContactProperty() { Type = "Email", Value = dataHelper.NextEmailAdress() });
            chatContact.Property.Add(new ContactProperty() { Type = "Phone", Value = dataHelper.NextRandomPhoneNumber() });
            chatContact.SearchPhrase= dataHelper.NextRandomSearchPhrase();
            chatContact.AutoRatingScore = 5 ;

            return chatContact;
        }

        private Contact GetModifiedMail(Contact realMailContact)
        {

            var mailContact = GetModifiedContact(realMailContact);

            foreach (var contactInteraction in realMailContact.Interaction)
            {
                mailContact.Interaction.Add(new ContactInteraction() { Type = contactInteraction.Type, Value = contactInteraction.Value });
            }

            if (realMailContact.HasProperty("FromEmail"))
            {
                mailContact.Property.Add(new ContactProperty()
                {
                    Type = "FromEmail",
                    Value = realMailContact.GetProperty("FromEmail").Value
                });
            }
            mailContact.Property.Add(new ContactProperty() {Type = "Html", Value = ""});

            if (realMailContact.HasProperty("MandrillId"))
            {
                mailContact.Property.Add(new ContactProperty()
                {
                    Type = "MandrillId",
                    Value = realMailContact.GetProperty("MandrillId").Value
                });
            }
            if (realMailContact.HasProperty("State"))
            {
                mailContact.Property.Add(new ContactProperty()
                {
                    Type = "State",
                    Value = realMailContact.GetProperty("State").Value
                });
            }
            if (realMailContact.HasProperty("Subject"))
            {
                mailContact.Property.Add(new ContactProperty()
                {
                    Type = "Subject",
                    Value = realMailContact.GetProperty("Subject").Value
                });
            }
            mailContact.Property.Add(new ContactProperty() { Type = "Tags", Value = "" });
            mailContact.Property.Add(new ContactProperty() { Type = "Text", Value = "" });
            if (realMailContact.HasProperty("ToEmail"))
            {
                mailContact.Property.Add(new ContactProperty()
                {
                    Type = "ToEmail",
                    Value = realMailContact.GetProperty("ToEmail").Value
                });
            }
            if (realMailContact.HasProperty("Updated"))
            {
                mailContact.Property.Add(new ContactProperty()
                {
                    Type = "Updated",
                    Value = realMailContact.GetProperty("Updated").Value
                });
            }

            return mailContact;
        }

        private Contact GetModifiedContact(Contact realContact)
        {
            var newContact = new Contact();
            newContact.LeadType = realContact.LeadType;
            newContact.Date = realContact.Date;

            if (realContact.HasProperty("Product"))
            {
                newContact.Property.Add(new ContactProperty()
                {
                    Type = "Product",
                    Value = realContact.GetProperty("Product").Value
                });
            }

            return newContact;
        }
        

        private Contact GetModifiedPhoneCall(Contact realPhoneContact)
        {
            var phoneContact = GetModifiedContact(realPhoneContact);

            if (realPhoneContact.HasProperty("Updated"))
            {
                phoneContact.Property.Add(new ContactProperty()
                {
                    Type = "CTMId",
                    Value = realPhoneContact.GetProperty("CTMId").Value
                });
            }

            phoneContact.Property.Add(new ContactProperty() { Type = "Audio", Value = "" });
            if (realPhoneContact.HasProperty("BusinessNumber"))
            {
                phoneContact.Property.Add(new ContactProperty()
                {
                    Type = "BusinessNumber",
                    Value = realPhoneContact.GetProperty("BusinessNumber").Value
                });
            }

            if (realPhoneContact.HasProperty("CallerNumber"))
            {
                phoneContact.Property.Add(new ContactProperty()
                {
                    Type = "CallerNumber",
                    Value = realPhoneContact.GetProperty("CallerNumber").Value
                });
            }
            if (realPhoneContact.HasProperty("City"))
            {
                phoneContact.Property.Add(new ContactProperty()
                {
                    Type = "City",
                    Value = realPhoneContact.GetProperty("City").Value
                });
            }
            if (realPhoneContact.HasProperty("Country"))
            {
                phoneContact.Property.Add(new ContactProperty()
                {
                    Type = "Country",
                    Value = realPhoneContact.GetProperty("Country").Value
                });
            }
            if (realPhoneContact.HasProperty("LocationUrl"))
            {
                phoneContact.Property.Add(new ContactProperty()
                {
                    Type = "LocationUrl",
                    Value = realPhoneContact.GetProperty("LocationUrl").Value
                });
            }
            if (realPhoneContact.HasProperty("PostalCode"))
            {
                phoneContact.Property.Add(new ContactProperty()
                {
                    Type = "PostalCode",
                    Value = realPhoneContact.GetProperty("PostalCode").Value
                });
            }

            if (realPhoneContact.HasProperty("ReferalUrl"))
            {
                phoneContact.Property.Add(new ContactProperty()
                {
                    Type = "ReferalUrl",
                    Value = realPhoneContact.GetProperty("ReferalUrl").Value
                });
            }

            if (realPhoneContact.HasProperty("SearchPhrase"))
            {
                phoneContact.Property.Add(new ContactProperty()
                {
                    Type = "SearchPhrase",
                    Value = realPhoneContact.GetProperty("SearchPhrase").Value
                });
            }

            if (realPhoneContact.HasProperty("Source"))
            {
                phoneContact.Property.Add(new ContactProperty()
                {
                    Type = "Source",
                    Value = realPhoneContact.GetProperty("Source").Value
                });
            }
            if (realPhoneContact.HasProperty("State"))
            {
                phoneContact.Property.Add(new ContactProperty()
                {
                    Type = "State",
                    Value = realPhoneContact.GetProperty("State").Value
                });
            }
            if (realPhoneContact.HasProperty("Status"))
            {
                phoneContact.Property.Add(new ContactProperty()
                {
                    Type = "Status",
                    Value = realPhoneContact.GetProperty("Status").Value
                });
            }
            if (realPhoneContact.HasProperty("TrackingNumber"))
            {
                phoneContact.Property.Add(new ContactProperty()
                {
                    Type = "TrackingNumber",
                    Value = realPhoneContact.GetProperty("TrackingNumber").Value
                });
            }
            if (realPhoneContact.HasProperty("CTMAccoutId"))
            {
                phoneContact.Property.Add(new ContactProperty()
                {
                    Type = "CTMAccoutId",
                    Value = realPhoneContact.GetProperty("CTMAccoutId").Value
                });
            }
            if (realPhoneContact.HasProperty("Duration"))
            {
                phoneContact.Property.Add(new ContactProperty()
                {
                    Type = "Duration",
                    Value = realPhoneContact.GetProperty("Duration").Value
                });
            }

            return phoneContact;
        }

        private Contact GetModifiedChat(Contact realChatContact)
        {
            var chatContact = GetModifiedContact(realChatContact);
            
            chatContact.Property.Add(new ContactProperty() { Type = "Description", Value = "" });
            if (realChatContact.HasProperty("Email"))
            {
                chatContact.Property.Add(new ContactProperty()
                {
                    Type = "Email",
                    Value = realChatContact.GetProperty("Email").Value
                });
            }
            if (realChatContact.HasProperty("Phone"))
            {
                chatContact.Property.Add(new ContactProperty()
                {
                    Type = "Phone",
                    Value = realChatContact.GetProperty("Phone").Value
                });
            }

            return chatContact;
        }
        public override void SaveChanges()
        {
            var isLoggedIn = HttpContext.Current.User.Identity.IsAuthenticated;
            var isBasicAuthenticationForTaskCreating = HttpContext.Current.User.Identity.AuthenticationType == "BasicAuthenticationForTaskCreating";
            if (isLoggedIn &&  !isBasicAuthenticationForTaskCreating && userManager.IsInRole(HttpContext.Current.User.Identity.GetUserId(), "demo"))
            {
                throw new Exception("Not allowed for demo");
            }
            else if (isLoggedIn && !isBasicAuthenticationForTaskCreating && userManager.IsInRole(HttpContext.Current.User.Identity.GetUserId(), "sales"))
            {
                throw new Exception("Not allowed for sales");
            }
            else
            {
                base.SaveChanges();
            }
        }
    }
}