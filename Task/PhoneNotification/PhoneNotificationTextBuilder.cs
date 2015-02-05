using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using InsideModel.Models;
using InsideModel.repositories;

namespace Task.PhoneNotification
{
    public class PhoneNotificationTextBuilder : IPhoneNotificationTextBuilder
    {
        private readonly IRepository<Contact> contactRepository;

        public PhoneNotificationTextBuilder(IRepository<Contact> contactRepository)
        {
            this.contactRepository = contactRepository;
        }

        public string GetDefinition(int contactId)
        {
            var contactNotificationToSend = contactRepository.Where(c => c.Id == contactId)
                .Include(l => l.Client);

            var contact = contactNotificationToSend.SingleOrDefault();
            if (contact == null)
            {
                throw new Exception("No contact information found");
            }
            var phoneNotificationText = "";

            if (contact.LeadType == "Phone")
            {
                var callerNumberProperty = contact.Property.SingleOrDefault(lp => lp.Type == "CallerNumber" && lp.ContactId == contact.Id);
                phoneNotificationText = "Du har fått ett nytt telefonsamtal";

                if (callerNumberProperty != null)
                {
                    phoneNotificationText += " från " + callerNumberProperty.Value;
                }
            }
            else if (contact.LeadType == "Email")
            {
                phoneNotificationText = "Du har fått ett nytt email";
                
                var fromEmailProperty = contact.Property.SingleOrDefault(lp => lp.Type == "FromEmail" && lp.ContactId == contact.Id);
                if (fromEmailProperty != null)
                {
                    phoneNotificationText += " från " + fromEmailProperty.Value;
                }
            }
            else if (contact.LeadType == "Chat")
            {
                phoneNotificationText = "Du har fått ett ny chat";
                var fromValues = new List<string>();
                
                var phoneProperty = contact.Property.SingleOrDefault(lp => lp.Type == "Phone" && lp.ContactId == contact.Id);
                if (phoneProperty != null)
                {
                    fromValues.Add(phoneProperty.Value);
                }

                var emailProperty = contact.Property.SingleOrDefault(lp => lp.Type == "Email" && lp.ContactId == contact.Id);
                if (emailProperty != null)
                {
                    fromValues.Add(emailProperty.Value);
                }

                if (fromValues.Any())
                {
                    phoneNotificationText += " från " + string.Join(", ", fromValues);
                }
            }
            return phoneNotificationText;
        }
    }
}
