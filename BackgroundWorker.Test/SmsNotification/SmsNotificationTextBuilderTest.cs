using System;
using Helpers.test;
using Inside.AcceptToken;
using Inside.GoogleService;
using InsideModel.Models;
using InsideModel.repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Task.PhoneNotification;
using Task.SmsNotification.SmsNotificationSenderToUser;

namespace BackgroundWorker.Test.SmsNotification
{
    [TestClass]
    public class SmsNotificationTextBuilderTest
    {

        [TestMethod]
        public void CanCreate()
        {
            var contactRepository = new Mock<IRepository<Contact>>();
            var accessTokenProvider = new Mock<IAccessTokenProvider>();
            var googleUrlShortner = new Mock<IGoogleUrlShortnerService>();
            var smsTextBuilder = new SmsNotificationTextBuilder(contactRepository.Object, 
                                                                googleUrlShortner.Object,
                                                                accessTokenProvider.Object);
        }

        [TestMethod]
        public void CreateTextForContactWithOkData()
        {
            
            var accessTokenProvider = new Mock<IAccessTokenProvider>();
            var googleUrlShortner = new Mock<IGoogleUrlShortnerService>();
            var contactRepository = new LocalRepository<Contact>();

            var contact = new Contact()
            {
                Id = 1,
                LeadType = "Phone"
            };

            var client = ModelHelper.TestClient1AllDataNoReferences;
            contact.Client = client;

            var contactProperty = new ContactProperty()
            {
                Id = 1,
                ContactId = 1,
                Type = "CallerNumber",
                Value = "08123456"
            };
            

            contact.Property.Add(contactProperty);
            contactRepository.Add(contact);

            var userId = "userId1";

            accessTokenProvider.Setup(ap => ap.GenerateAccessUrl(userId, "/report/1/contact/1"))
                .Returns("LongUrlWithAccessTokenAndEverything");

            googleUrlShortner.Setup(us => us.GetShortUrl("LongUrlWithAccessTokenAndEverything")).Returns("short.url");

            var smsTextBuilder = new SmsNotificationTextBuilder(contactRepository,
                                                                googleUrlShortner.Object,
                                                                accessTokenProvider.Object);

            
            var result = smsTextBuilder.GetDefinition(contact.Id, userId);

            Assert.AreEqual("Du har fått en ny Helloykontakt! Rejta eller dela " + "short.url", result);
        }

       

        [TestMethod]
        [ExpectedException(typeof (Exception))]
        public void ThrowExceptionWithNoContact()
        {
            var contactRepository = new LocalRepository<Contact>();
            var accessTokenProvider = new Mock<IAccessTokenProvider>();
            var googleUrlShortner = new Mock<IGoogleUrlShortnerService>();

            var contactNotInRepository = new Contact()
            {
                Id = 1,
                LeadType = "Email"
            };

            var smsTextBuilder = new SmsNotificationTextBuilder(contactRepository,
                                                                googleUrlShortner.Object,
                                                                accessTokenProvider.Object);

            var userId = "userId1";
            var result = smsTextBuilder.GetDefinition(contactNotInRepository.Id, userId);
        }
    }
}