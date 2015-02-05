using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web.Security;
using Helpers.test;
using Inside.AcceptToken;
using Inside.membership;
using InsideModel.Models;
using InsideModel.Models.Identity;
using InsideModel.repositories;
using Microsoft.AspNet.Identity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Task.Email.NotificationEmail;

namespace BackgroundWorker.Test.Email.Notification
{
    [TestClass]
    public class NotificationDefinitionBuilderTest
    {
        [TestMethod]
        public void CanConstruct()
        {
            var contactRepository = new Mock<IRepository<Contact>>();
            var adminRepository = new Mock<IRepository<InsideUser>>();
            var accessTokenProvider = new Mock<IAccessTokenProvider>();
            
            var newContactDefinitionBuilder = new NotificationEmailDefenitionBuilder(
                adminRepository.Object,
                contactRepository.Object,
                accessTokenProvider.Object
            );
        }


        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void EmailAddressInBadFormatDoesntCreateDefenitionThrowsException()
        {
            var contactRepository = new LocalRepository<Contact>();
            var adminRepository = new LocalRepository<InsideUser>();
            var accessTokenProvider = new Mock<IAccessTokenProvider>();
            
            var contact = new Contact()
            {
                Id = 1,
                Date = new DateTime(2014, 01, 01),
                LeadType = "Phone",
                ClientId = 1
            };
            var client = ModelHelper.TestClient1AllDataNoReferences;
            contact.Client = client;
            var guidId = Guid.NewGuid();
            var insideUser = new InsideUser()
            {
                Id = guidId.ToString(),
                ReceiveEmail = true,
                Email = "badFormatEmail"
            };
            
            contact.Client.InsideUserSets=new Collection<InsideUser>();
            contact.Client.InsideUserSets.Add(insideUser);
            contactRepository.Add(contact);

            var role = new InsideRole() {Name = "consultant"};

            //adminRepository.Setup(ar=>ar.Where(a=>a.ConsultantsForClients.Any(c=>c.Id==client.Id)).Single()).Returns(new Admin(){AdminRole = "Consultant"});
            adminRepository.Add(new InsideUser()
            {Role = new Collection<InsideRole>{role},
             ConsultantsForClients = new Collection<Client> {client }
            });
            var newContactDefinitionBuilder = new NotificationEmailDefenitionBuilder(
                adminRepository,
                contactRepository,
                accessTokenProvider.Object
            );
           
           var result = newContactDefinitionBuilder.GetDefinition(contact.Id, insideUser.Id);

            Assert.IsNull(result);
        }

       
        [TestMethod]
        public void CreatesValidDefintionForOkDataForPhone()
        {
            var contactRepository = new LocalRepository<Contact>();
            var adminRepository = new LocalRepository<InsideUser>();
            var accessTokenProvider = new Mock<IAccessTokenProvider>();
            
            var contact = new Contact()
            {
                Id = 1,
                Date = new DateTime(2014, 01, 01,12,30,00),
                LeadType = "Phone",
                ClientId = 1
            };

            var contactProperty = new ContactProperty()
            {
                Id = 1,
                ContactId = 1,
                Type = "CallerNumber",
                Value = "08123456"
            };
            var contactProperty2 = new ContactProperty()
            {
                Id = 2,
                ContactId = 1,
                Type = "Duration",
                Value = "90"
            };

            contact.Property.Add(contactProperty);
            contact.Property.Add(contactProperty2);

            var clientLable = new Label()
            {
                Id = 1,
                Name = "Helloypaket"
            };
            var client = ModelHelper.TestClient1AllDataNoReferences;
            client.Labels.Add(clientLable);
            contact.Client=client;


            contact.Client.InsideUserSets=new Collection<InsideUser>();
            var user1 = ModelHelper.TestUser1AllDataNoReferences;
            user1.Email = "email@test.se";
            contact.Client.InsideUserSets.Add(user1);

            contactRepository.Add(contact);

            var consultantInfo = new InsideUser();
            consultantInfo.Email = "consultant@helloy.se";
            consultantInfo.Name = "consultant name";
            consultantInfo.Phone = "08654321";
            consultantInfo.ImageUrl = "http://path.to/image.jpg";
            consultantInfo.ConsultantsForClients.Add(client);
            consultantInfo.Role = new Collection<InsideRole>{ModelHelper.TestConsultantRole};

            adminRepository.Add(consultantInfo);
            accessTokenProvider.Setup(at => at.GenerateAccessUrl(user1.Id, "/report/" + client.Id + "/contact/" + contact.Id)).Returns("http://inside.helloy.se/Account/AuthenticateToken?token=Aa1Bb2Cc3Dd4Ee5%26returnUrl=%2freport%2f1%2fcontact%2f1");
           var newContactDefinitionBuilder = new NotificationEmailDefenitionBuilder(
                adminRepository,
                contactRepository,
                accessTokenProvider.Object
            );

            var contactId = contact.Id;

            var result = newContactDefinitionBuilder.GetDefinition(contactId, user1.Id);

            
            Assert.AreEqual("2014-01-01 13:30", result.Date);
            Assert.AreEqual("consultant@helloy.se", result.ConsultantEmail);
            Assert.AreEqual("http://path.to/image.jpg", result.ConsultantImage);
            Assert.AreEqual("consultant name", result.ConsultantName);
            Assert.AreEqual("08654321", result.ConsultantPhone);
            Assert.AreEqual("08123456", result.From);
            Assert.AreEqual("http://inside.helloy.se/Account/AuthenticateToken?token=Aa1Bb2Cc3Dd4Ee5%26returnUrl=%2freport%2f1%2fcontact%2f1", result.InsideUrl);
            Assert.AreEqual("1:30", result.SubjectOrDuration);
            Assert.AreEqual("email@test.se", result.UserEmail);
            
        }

        [TestMethod]
        public void CreatesValidDefintionForOkPhoneWithoutDurationProperty()
        {
            var contactRepository = new LocalRepository<Contact>();
            var adminRepository = new LocalRepository<InsideUser>();
            var accessTokenProvider = new Mock<IAccessTokenProvider>();
            
            var contact = new Contact()
            {
                Id = 1,
                Date = new DateTime(2014, 01, 01, 12, 30, 00),
                LeadType = "Phone",
                ClientId = 1
            };

            var contactProperty = new ContactProperty()
            {
                Id = 1,
                ContactId = 1,
                Type = "CallerNumber",
                Value = "08123456"
            };
            
            contact.Property.Add(contactProperty);
           
            var clientLable = new Label()
            {
                Id = 1,
                Name = "Helloypaket"
            };
            var client = ModelHelper.TestClient1AllDataNoReferences;
            client.Labels.Add(clientLable);
            contact.Client = client;


            contact.Client.InsideUserSets = new Collection<InsideUser>();
            var user1 = ModelHelper.TestUser1AllDataNoReferences;
            user1.Email = "email@test.se";
            contact.Client.InsideUserSets.Add(user1);

            contactRepository.Add(contact);

            var consultantInfo = new InsideUser();
            consultantInfo.Email = "consultant@helloy.se";
            consultantInfo.Name = "consultant name";
            consultantInfo.Phone = "08654321";
            consultantInfo.ImageUrl = "http://path.to/image.jpg";
            consultantInfo.ConsultantsForClients.Add(client);
            consultantInfo.Role = new Collection<InsideRole>{ModelHelper.TestConsultantRole};

            adminRepository.Add(consultantInfo);
            accessTokenProvider.Setup(at => at.GenerateAccessUrl(user1.Id, "/report/" + client.Id + "/contact/" + contact.Id)).Returns("http://inside.helloy.se/Account/AuthenticateToken?token=Aa1Bb2Cc3Dd4Ee5%26returnUrl=%2freport%2f1%2fcontact%2f1");
            var newContactDefinitionBuilder = new NotificationEmailDefenitionBuilder(
                 adminRepository,
                 contactRepository,
                 accessTokenProvider.Object
             );

            var contactId = contact.Id;

            var result = newContactDefinitionBuilder.GetDefinition(contactId, user1.Id);

            
            Assert.AreEqual("2014-01-01 13:30", result.Date);
            Assert.AreEqual("consultant@helloy.se", result.ConsultantEmail);
            Assert.AreEqual("http://path.to/image.jpg", result.ConsultantImage);
            Assert.AreEqual("consultant name", result.ConsultantName);
            Assert.AreEqual("08654321", result.ConsultantPhone);
            Assert.AreEqual("08123456", result.From);
            Assert.AreEqual("http://inside.helloy.se/Account/AuthenticateToken?token=Aa1Bb2Cc3Dd4Ee5%26returnUrl=%2freport%2f1%2fcontact%2f1", result.InsideUrl);
            Assert.IsNull(result.SubjectOrDuration);
            Assert.AreEqual("email@test.se", result.UserEmail);

        }

        [TestMethod]
        public void CanCreateEmailWithOnePropertyForChats()
        {
            var contactRepository = new LocalRepository<Contact>();
            var adminRepository = new LocalRepository<InsideUser>();
            var accessTokenProvider = new Mock<IAccessTokenProvider>();
            
            var contact = new Contact()
            {
                Id = 1,
                Date = new DateTime(2014, 01, 01, 12, 30, 00),
                LeadType = "Chat",
                ClientId = 1
            };

            var contactProperty = new ContactProperty()
            {
                Id = 1,
                ContactId = 1,
                Type = "Phone",
                Value = "08123456"
            };
           
            contact.Property.Add(contactProperty);
           
            var clientLable = new Label()
            {
                Id = 1,
                Name = "Helloypaket"
            };
            var client = ModelHelper.TestClient1AllDataNoReferences;
            client.Labels.Add(clientLable);
            contact.Client = client;


            contact.Client.InsideUserSets = new Collection<InsideUser>();
            var user1 = ModelHelper.TestUser1AllDataNoReferences;
            user1.Email = "email@test.se";
            contact.Client.InsideUserSets.Add(user1);

            contactRepository.Add(contact);

            var consultantInfo = new InsideUser();
            consultantInfo.Email = "consultant@helloy.se";
            consultantInfo.Name = "consultant name";
            consultantInfo.Phone = "08654321";
            consultantInfo.ImageUrl = "http://path.to/image.jpg";
            consultantInfo.ConsultantsForClients.Add(client);
            consultantInfo.Role = new Collection<InsideRole>{ModelHelper.TestConsultantRole};

            adminRepository.Add(consultantInfo);
            accessTokenProvider.Setup(at => at.GenerateAccessUrl(user1.Id, "/report/" + client.Id + "/contact/" + contact.Id)).Returns("http://inside.helloy.se/Account/AuthenticateToken?token=Aa1Bb2Cc3Dd4Ee5%26returnUrl=%2freport%2f1%2fcontact%2f1");
            var newContactDefinitionBuilder = new NotificationEmailDefenitionBuilder(
                 adminRepository,
                 contactRepository,
                 accessTokenProvider.Object
             );

            var contactId = contact.Id;

            var result = newContactDefinitionBuilder.GetDefinition(contactId ,user1.Id);

            
            Assert.AreEqual("2014-01-01 13:30", result.Date);
            Assert.AreEqual("consultant@helloy.se", result.ConsultantEmail);
            Assert.AreEqual("http://path.to/image.jpg", result.ConsultantImage);
            Assert.AreEqual("consultant name", result.ConsultantName);
            Assert.AreEqual("08654321", result.ConsultantPhone);
            Assert.AreEqual("08123456", result.From);
            Assert.AreEqual("http://inside.helloy.se/Account/AuthenticateToken?token=Aa1Bb2Cc3Dd4Ee5%26returnUrl=%2freport%2f1%2fcontact%2f1", result.InsideUrl);
            Assert.AreEqual("email@test.se", result.UserEmail);

        }

        [TestMethod]
        public void CanCreateEmailWithTwoPropertyForChats()
        {
            var contactRepository = new LocalRepository<Contact>();
            var adminRepository = new LocalRepository<InsideUser>();
            var accessTokenProvider = new Mock<IAccessTokenProvider>();
            
            var contact = new Contact()
            {
                Id = 1,
                Date = new DateTime(2014, 01, 01, 12, 30, 00),
                LeadType = "Chat",
                ClientId = 1
            };

            var contactProperty = new ContactProperty()
            {
                Id = 1,
                ContactId = 1,
                Type = "Phone",
                Value = "08123456"
            };

            var contactProperty2 = new ContactProperty()
            {
                Id = 1,
                ContactId = 1,
                Type = "Email",
                Value = "from@email.com"
            };

            contact.Property.Add(contactProperty);
            contact.Property.Add(contactProperty2);

            var clientLable = new Label()
            {
                Id = 1,
                Name = "Helloypaket"
            };
            var client = ModelHelper.TestClient1AllDataNoReferences;
            client.Labels.Add(clientLable);
            contact.Client = client;


            contact.Client.InsideUserSets = new Collection<InsideUser>();
            var user1 = ModelHelper.TestUser1AllDataNoReferences;
            user1.Email = "email@test.se";
            contact.Client.InsideUserSets.Add(user1);

            contactRepository.Add(contact);

            var consultantInfo = new InsideUser();
            consultantInfo.Email = "consultant@helloy.se";
            consultantInfo.Name = "consultant name";
            consultantInfo.Phone = "08654321";
            consultantInfo.ImageUrl = "http://path.to/image.jpg";
            consultantInfo.ConsultantsForClients.Add(client);
            consultantInfo.Role = new Collection<InsideRole>{ModelHelper.TestConsultantRole};

            adminRepository.Add(consultantInfo);
            accessTokenProvider.Setup(at => at.GenerateAccessUrl(user1.Id, "/report/" + client.Id + "/contact/" + contact.Id)).Returns("http://inside.helloy.se/Account/AuthenticateToken?token=Aa1Bb2Cc3Dd4Ee5%26returnUrl=%2freport%2f1%2fcontact%2f1");
            var newContactDefinitionBuilder = new NotificationEmailDefenitionBuilder(
                 adminRepository,
                 contactRepository,
                 accessTokenProvider.Object
             );

            var contactId = contact.Id;

            var result = newContactDefinitionBuilder.GetDefinition(contactId, user1.Id);

            Assert.AreEqual("2014-01-01 13:30", result.Date);
            Assert.AreEqual("consultant@helloy.se", result.ConsultantEmail);
            Assert.AreEqual("http://path.to/image.jpg", result.ConsultantImage);
            Assert.AreEqual("consultant name", result.ConsultantName);
            Assert.AreEqual("08654321", result.ConsultantPhone);
            Assert.AreEqual("08123456, from@email.com", result.From);
            Assert.AreEqual("http://inside.helloy.se/Account/AuthenticateToken?token=Aa1Bb2Cc3Dd4Ee5%26returnUrl=%2freport%2f1%2fcontact%2f1", result.InsideUrl);
            Assert.AreEqual("email@test.se", result.UserEmail);

        }

        [TestMethod]
        public void CanCreateEmailWithTwoPropertyForEmails()
        {
            var contactRepository = new LocalRepository<Contact>();
            var adminRepository = new LocalRepository<InsideUser>();
            var accessTokenProvider = new Mock<IAccessTokenProvider>();

            var contact = new Contact()
            {
                Id = 1,
                Date = new DateTime(2014, 01, 01, 12, 30, 00),
                LeadType = "Email",
                ClientId = 1
            };

            var contactProperty = new ContactProperty()
            {
                Id = 1,
                ContactId = 1,
                Type = "Subject",
                Value = "This is a Subject"
            };

            var contactProperty2 = new ContactProperty()
            {
                Id = 1,
                ContactId = 1,
                Type = "FromEmail",
                Value = "from@email.com"
            };

            contact.Property.Add(contactProperty);
            contact.Property.Add(contactProperty2);

            var clientLable = new Label()
            {
                Id = 1,
                Name = "Helloypaket"
            };
            var client = ModelHelper.TestClient1AllDataNoReferences;
            client.Labels.Add(clientLable);
            contact.Client = client;


            contact.Client.InsideUserSets = new Collection<InsideUser>();
            var user1 = ModelHelper.TestUser1AllDataNoReferences;
            user1.Email = "email@test.se";
            contact.Client.InsideUserSets.Add(user1);

            contactRepository.Add(contact);

            var consultantInfo = new InsideUser();
            consultantInfo.Email = "consultant@helloy.se";
            consultantInfo.Name = "consultant name";
            consultantInfo.Phone = "08654321";
            consultantInfo.ImageUrl = "http://path.to/image.jpg";
            consultantInfo.ConsultantsForClients.Add(client);
            consultantInfo.Role = new Collection<InsideRole> {ModelHelper.TestConsultantRole};

            adminRepository.Add(consultantInfo);
            accessTokenProvider.Setup(at => at.GenerateAccessUrl(user1.Id, "/report/" + client.Id + "/contact/" + contact.Id))
                .Returns(
                    "http://inside.helloy.se/Account/AuthenticateToken?token=Aa1Bb2Cc3Dd4Ee5%26returnUrl=%2freport%2f1%2fcontact%2f1");
            var newContactDefinitionBuilder = new NotificationEmailDefenitionBuilder(
                adminRepository,
                contactRepository,
                accessTokenProvider.Object
                );

            var contactId = contact.Id;

            var result = newContactDefinitionBuilder.GetDefinition(contactId, user1.Id);

            Assert.AreEqual("2014-01-01 13:30", result.Date);
            Assert.AreEqual("consultant@helloy.se", result.ConsultantEmail);
            Assert.AreEqual("http://path.to/image.jpg", result.ConsultantImage);
            Assert.AreEqual("consultant name", result.ConsultantName);
            Assert.AreEqual("08654321", result.ConsultantPhone);
            Assert.AreEqual("from@email.com", result.From);
            Assert.AreEqual(
                "http://inside.helloy.se/Account/AuthenticateToken?token=Aa1Bb2Cc3Dd4Ee5%26returnUrl=%2freport%2f1%2fcontact%2f1",
                result.InsideUrl);
            Assert.AreEqual("email@test.se", result.UserEmail);
            Assert.AreEqual("This is a Subject", result.SubjectOrDuration);
        }
    }
}
