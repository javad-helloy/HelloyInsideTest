using System;
using Helpers.test;
using InsideModel.Models;
using InsideModel.repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Task.PhoneNotification;

namespace BackgroundWorker.Test.PhoneNotification
{
    [TestClass]
    public class PhoneNotificationTextBuilderTest
    {
        [TestMethod]
        public void CanCreate()
        {
            var contactRepository = new Mock<IRepository<Contact>>();
            var notificationTextBuilder = new PhoneNotificationTextBuilder(contactRepository.Object);
        }

        [TestMethod]
        public void CreateTextForPhoneContactWithOkData()
        {
            var contactRepository = new LocalRepository<Contact>();
            
            var contact = new Contact()
            {
                Id = 1,
                LeadType = "Phone"
            };

            var contactProperty = new ContactProperty()
            {
                Id = 1,
                ContactId = 1,
                Type = "CallerNumber",
                Value = "08123456"
            };

            contact.Property.Add(contactProperty);
            contactRepository.Add(contact);

            var notificationTextBuilder = new PhoneNotificationTextBuilder(contactRepository);
            var result = notificationTextBuilder.GetDefinition(contact.Id);

            Assert.AreEqual("Du har fått ett nytt telefonsamtal från 08123456", result);
        }

        [TestMethod]
        public void CreateTextForChatContactWithOkData()
        {
            var contactRepository = new LocalRepository<Contact>();

            var contact = new Contact()
            {
                Id = 1,
                LeadType = "Chat"
            };

            var contactPropertyEmail = new ContactProperty()
            {
                Id = 1,
                ContactId = 1,
                Type = "Email",
                Value = "from@email.com"
            };

            var contactPropertyPhone = new ContactProperty()
            {
                Id = 1,
                ContactId = 1,
                Type = "Phone",
                Value = "08123456"
            };

            contact.Property.Add(contactPropertyEmail);
            contactRepository.Add(contact);

            var notificationTextBuilder = new PhoneNotificationTextBuilder(contactRepository);
            var result = notificationTextBuilder.GetDefinition(contact.Id);

            Assert.AreEqual("Du har fått ett ny chat från from@email.com", result);

            contactRepository.Delete(contact);
            contact.Property.Add(contactPropertyPhone);
            contactRepository.Add(contact);

            
            var result2 = notificationTextBuilder.GetDefinition(contact.Id);

            Assert.AreEqual("Du har fått ett ny chat från 08123456, from@email.com", result2);
        }

        [TestMethod]
        public void CreateTextForEmailContactWithOkData()
        {
            var contactRepository = new LocalRepository<Contact>();

            var contact = new Contact()
            {
                Id = 1,
                LeadType = "Email"
            };

            var contactProperty = new ContactProperty()
            {
                Id = 1,
                ContactId = 1,
                Type = "FromEmail",
                Value = "from@email.com"
            };

            contact.Property.Add(contactProperty);
            contactRepository.Add(contact);

            var notificationTextBuilder = new PhoneNotificationTextBuilder(contactRepository);
            var result = notificationTextBuilder.GetDefinition(contact.Id);

            Assert.AreEqual("Du har fått ett nytt email från from@email.com", result);
        }

        [TestMethod]
        public void CreateTextForContactWithOkDataWithNoContactProperties()
        {
            var contactRepository = new LocalRepository<Contact>();

            var contact = new Contact()
            {
                Id = 1,
                LeadType = "Email"
            };

            var contact2 = new Contact()
            {
                Id = 2,
                LeadType = "Chat"
            };

            var contact3 = new Contact()
            {
                Id = 3,
                LeadType = "Phone"
            };

            contactRepository.Add(contact);
            contactRepository.Add(contact2);
            contactRepository.Add(contact3);

            var notificationTextBuilder = new PhoneNotificationTextBuilder(contactRepository);
            
            var result = notificationTextBuilder.GetDefinition(contact.Id);
            Assert.AreEqual("Du har fått ett nytt email", result);

            var result2 = notificationTextBuilder.GetDefinition(contact2.Id);
            Assert.AreEqual("Du har fått ett ny chat", result2);

            var result3 = notificationTextBuilder.GetDefinition(contact3.Id);
            Assert.AreEqual("Du har fått ett nytt telefonsamtal", result3);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void ThrowExceptionWithNoContact()
        {
            var contactRepository = new LocalRepository<Contact>();

            var contactNotInRepository = new Contact()
            {
                Id = 1,
                LeadType = "Email"
            };

            var notificationTextBuilder = new PhoneNotificationTextBuilder(contactRepository);
            var result = notificationTextBuilder.GetDefinition(contactNotInRepository.Id);
        }

        [TestMethod]
        public void CreateCorrectTextForManyEmailContactInRepository()
        {
            var contactRepository = new LocalRepository<Contact>();

            var contactProperty = new ContactProperty()
            {
                Id = 1,
                ContactId = 1,
                Type = "FromEmail",
                Value = "from@email.com"
            };

            var contact1 = new Contact()
            {
                Id = 1,
                LeadType = "Email"
            };
            contact1.Property.Add(contactProperty);

            var contact2 = new Contact()
            {
                Id = 2,
                LeadType = "Email"
            };

            contactRepository.Add(contact2);
            contactRepository.Add(contact1);

            var notificationTextBuilder = new PhoneNotificationTextBuilder(contactRepository);
            var result = notificationTextBuilder.GetDefinition(contact1.Id);

            Assert.AreEqual("Du har fått ett nytt email från from@email.com", result);
        }
    }
}
