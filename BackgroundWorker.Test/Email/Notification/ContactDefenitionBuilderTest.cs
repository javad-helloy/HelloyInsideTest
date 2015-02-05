using System;
using System.Text;
using System.Collections.Generic;
using Helpers.test;
using Inside.AcceptToken;
using InsideModel.Models;
using InsideModel.repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Task.Email.ContactEmail;
using Task.Email.Sender;

namespace BackgroundWorker.Test.Email.NewContact
{
    /// <summary>
    /// Summary description for ContactBuilderTest
    /// </summary>
    [TestClass]
    public class ContactDefenitionBuilderTest
    {
        
        [TestMethod]
        public void CanCreate()
        {
            
            var accessTokenProvider = new Mock<IAccessTokenProvider>();
            var controller = new ContactDefenitionBuilder(accessTokenProvider.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void InvalidContactIdReturnsException()
        {
            var accessTokenProvider = new Mock<IAccessTokenProvider>();
            var controller = new ContactDefenitionBuilder(accessTokenProvider.Object);
            var userId = 1;
            Lead contact = null;
            controller.GetContact(userId, contact);
        }

        [TestMethod]
        public void ReturnDefenitionWithOkDataForPhoneContact()
        {
            
            var accessTokenProvider = new Mock<IAccessTokenProvider>();
            var userId = 1;
            var contactProperty = new LeadProperty()
            {
                LeadId = 1,
                Type = "Duration",
                Value = "90"
            };
            var contactProperty2 = new LeadProperty()
            {
                LeadId = 1,
                Type = "CallerNumber",
                Value = "08123456"
            };

            var contact = new Lead()
            {
                Id = 1,
                ClientId = 1,
                LeadType = "Phone",
                Date = new DateTime(2014,1,1,12,30,00)
            };
            contact.Property.Add(contactProperty);
            contact.Property.Add(contactProperty2);
            
            var controller = new ContactDefenitionBuilder(accessTokenProvider.Object);

            accessTokenProvider.Setup(at => at.GenerateAccessUrl(userId, "/report/phone?clientId=" + contact.ClientId))
                .Returns("Some Url With Access Token and Return Url");
            
            var result = controller.GetContact(userId, contact);

            Assert.AreEqual("Phone", result.Type);
            Assert.AreEqual("2014-01-01 12:30",result.Date);
            Assert.AreEqual("nyyt telefonsamtal",result.DisplayName);
            Assert.AreEqual("1:30", result.SubjectOrDuration);
            Assert.AreEqual("Samtalslängd",result.SubjectOrDurationDisplayName);
            Assert.AreEqual("08123456", result.From);
            Assert.AreEqual("Du har fått ett nyyt telefonsamtal från 08123456. <a href=Some Url With Access Token and Return Url style=\"color: #009dd2;\">Lyssna</a> " +
                            "eller <a href=Some Url With Access Token and Return Url style=\"color: #009dd2;\">Betygsätt</a> direkt!", result.Description);
        }

        [TestMethod]
        public void ReturnDefenitionWithOkDataForEmailContact()
        {
            var accessTokenProvider = new Mock<IAccessTokenProvider>();
            var userId = 1;
            var contactProperty = new LeadProperty()
            {
                LeadId = 1,
                Type = "FromEmail",
                Value = "test@company.com"
            };
            var contactProperty2 = new LeadProperty()
            {
                LeadId = 1,
                Type = "Subject",
                Value = "Email Subject"
            };

            var contact = new Lead()
            {
                Id = 1,
                ClientId = 1,
                LeadType = "Email",
                Date = new DateTime(2014, 1, 1, 12, 30, 00)
            };
            contact.Property.Add(contactProperty);
            contact.Property.Add(contactProperty2);
            
            var controller = new ContactDefenitionBuilder( accessTokenProvider.Object);

            accessTokenProvider.Setup(at => at.GenerateAccessUrl(userId, "/report/mail?clientId=" + contact.ClientId))
                .Returns("Some Url With Access Token and Return Url");

            var result = controller.GetContact(userId, contact);

            Assert.AreEqual("Email", result.Type);
            Assert.AreEqual("2014-01-01 12:30", result.Date);
            Assert.AreEqual("nyyt email", result.DisplayName);
            Assert.AreEqual("Email Subject", result.SubjectOrDuration);
            Assert.AreEqual("Subjekt", result.SubjectOrDurationDisplayName);
            Assert.AreEqual("test@company.com", result.From);
            Assert.AreEqual("Du har fått ett nyyt email från test@company.com. <a href=Some Url With Access Token and Return Url style=\"color: #009dd2;\">Visa</a>" +
                            " eller <a href=Some Url With Access Token and Return Url style=\"color: #009dd2;\">Betygsätt</a> direkt!", result.Description);
        }

        [TestMethod]
        public void ReturnDefenitionWithOkDataForChatContact()
        {
            
            var accessTokenProvider = new Mock<IAccessTokenProvider>();
            var userId = 1;
            var contactProperty = new LeadProperty()
            {
                LeadId = 1,
                Type = "Email",
                Value = "test@company.com"
            };
            var contactProperty2 = new LeadProperty()
            {
                LeadId = 1,
                Type = "Phone",
                Value = "08123456"
            };

            var contact = new Lead()
            {
                Id = 1,
                ClientId = 1,
                LeadType = "Chat",
                Date = new DateTime(2014, 1, 1, 12, 30, 00)
            };
            contact.Property.Add(contactProperty);
            contact.Property.Add(contactProperty2);
            
            var controller = new ContactDefenitionBuilder(accessTokenProvider.Object);

            accessTokenProvider.Setup(at => at.GenerateAccessUrl(userId, "/report/chat?clientId=" + contact.ClientId))
                .Returns("Some Url With Access Token and Return Url");

            var result = controller.GetContact(userId, contact);

            Assert.AreEqual("Chat", result.Type);
            Assert.AreEqual("2014-01-01 12:30", result.Date);
            Assert.AreEqual("ny chat", result.DisplayName);
            
            Assert.IsNull(result.SubjectOrDuration);
            Assert.IsNull(result.SubjectOrDurationDisplayName);
            Assert.AreEqual("08123456", result.From);
            Assert.AreEqual("Du har fått ett ny chat från 08123456. <a href=Some Url With Access Token and Return Url style=\"color: #009dd2;\">Visa</a> direkt!", result.Description);
        }
    }
}
