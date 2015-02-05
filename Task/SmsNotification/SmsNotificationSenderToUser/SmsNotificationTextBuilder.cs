using System;
using System.Data.Entity;
using System.Linq;
using Inside.AcceptToken;
using Inside.GoogleService;
using InsideModel.Models;
using InsideModel.repositories;

namespace Task.SmsNotification.SmsNotificationSenderToUser
{
    public class SmsNotificationTextBuilder : ISmsNotificationTextBuilder
    {
        private readonly IRepository<Contact> contactRepository;
        private readonly IGoogleUrlShortnerService urlShortner;
        private readonly IAccessTokenProvider accessTokenProvider;

        public SmsNotificationTextBuilder(IRepository<Contact> contactRepository,
                                          IGoogleUrlShortnerService urlShortner,
                                          IAccessTokenProvider accessTokenProvider)
        {
            this.contactRepository = contactRepository;
            this.urlShortner = urlShortner;
            this.accessTokenProvider = accessTokenProvider;
        }

        public string GetDefinition(int contactId, string userId)
        {
            var contactNotificationToSend = contactRepository.Where(c => c.Id == contactId)
                .Include(l => l.Client);

            
            if (!contactNotificationToSend.Any())
            {
                throw new Exception("No contact information found");
            }

            var contact = contactNotificationToSend.Single();
            var url = accessTokenProvider.GenerateAccessUrl(userId, "/report/" + contact.Client.Id + "/contact/" + contact.Id);
            var shortUrl = urlShortner.GetShortUrl(url);
            var smsText = "Du har fått en ny Helloykontakt! Rejta eller dela " + shortUrl;
            return smsText;
        }
    }
}
