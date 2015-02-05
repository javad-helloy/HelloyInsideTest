using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Http.Results;
using System.Web.Security;
using Helpers.test;
using Inside.AcceptToken;
using Inside.membership;
using Inside.Time;
using InsideModel.Models;
using InsideModel.Models.Identity;
using InsideModel.repositories;
using InsideReporting.Controllers.Api;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace InsideReporting.Tests.Controllers.Api
{
    
    [TestClass]
    public class ExternalApiTest
    {

        [TestMethod]
        public void CanCreate()
        {
            var userRepository = new Mock<IRepository<InsideUser>>();
            var contactRepository = new Mock<IRepository<Contact>>();
            var tokenProvider = new Mock<IAccessTokenProvider>();
            var userManager = new Mock<IIdentityMembershipProvider>();
            var budgetRepositoryMock = new Mock<IRepository<Budget>>();
            var clientRepositoryMock = new Mock<IRepository<Client>>();
            var serverTimeMock = new Mock<IServerTime>();

            var controller = new ExternalController(
                userRepository.Object,
                contactRepository.Object,
                tokenProvider.Object,
                userManager.Object,
                budgetRepositoryMock.Object, 
                clientRepositoryMock.Object,
                serverTimeMock.Object);
        }

        [TestMethod]
        public void CanGetAccessToken()
        {
            var userRepository = new LocalRepository<InsideUser>();
            var contactRepository = new Mock<IRepository<Contact>>();

            var userManager = new Mock<IIdentityMembershipProvider>();
            var tokenGenerator = new Mock<IAccessTokenProvider>();
            var budgetRepositoryMock = new Mock<IRepository<Budget>>();
            var clientRepositoryMock = new Mock<IRepository<Client>>();
            var serverTimeMock = new Mock<IServerTime>();

            var username = "username";
            var password = "password";

            var membershipProviderId = new Guid();
            var memebershipUser = new Mock<MembershipUser>();
            memebershipUser.Setup(mu => mu.ProviderUserKey).Returns(membershipProviderId);
            var userInRepository = new InsideUser()
            {
                UserId= 1,
                ClientId = 1,
                Id = membershipProviderId.ToString()
            };
            
            userRepository.Add(userInRepository);
            userManager.Setup(mp => mp.ValidateAndReturnUser(username, password)).Returns(userInRepository);
            
            tokenGenerator.Setup(tg => tg.GetToken(It.IsAny<string>())).Returns(new Token(){AccessToken = "TokenGenerated"});
            
            var controller = new ExternalController(
                userRepository,
                contactRepository.Object,
                tokenGenerator.Object,
                userManager.Object,
                budgetRepositoryMock.Object,
                clientRepositoryMock.Object,
                serverTimeMock.Object);

            var result = controller.GetAccessToken(username, password) as OkNegotiatedContentResult<AcccessTokenForClient>;

            Assert.AreEqual("TokenGenerated",result.Content.AccessToken);

        }

        [TestMethod]
        public void InvalidUserCredentialsReturnsUnAuthorized()
        {
            var userRepository = new Mock<IRepository<InsideUser>>();
            var contactRepository = new Mock<IRepository<Contact>>();

            var userManager = new Mock<IIdentityMembershipProvider>();
            var tokenGenerator = new Mock<IAccessTokenProvider>();
            var budgetRepositoryMock = new Mock<IRepository<Budget>>();
            var clientRepositoryMock = new Mock<IRepository<Client>>();
            var serverTimeMock = new Mock<IServerTime>();

            var username = "username";
            var password = "password";

            userManager.Setup(mp => mp.ValidateAndReturnUser(username, password)).Returns(null as InsideUser);

            var controller = new ExternalController(
                userRepository.Object,
                contactRepository.Object,
                tokenGenerator.Object,
                userManager.Object,
                budgetRepositoryMock.Object,
                clientRepositoryMock.Object,
                serverTimeMock.Object);

            Assert.IsInstanceOfType(controller.GetAccessToken(username, password), typeof(UnauthorizedResult));
        }

        [TestMethod]
        public void GetContactListReturnsContactsBasedOnPageNumber()
        {

            var userRepository = new Mock<IRepository<InsideUser>>();
            var contactRepository = new LocalRepository<Contact>();

            var userManager = new Mock<IIdentityMembershipProvider>();
            var tokenGenerator = new Mock<IAccessTokenProvider>();

            var budgetRepositoryMock = new Mock<IRepository<Budget>>();
            var clientRepositoryMock = new Mock<IRepository<Client>>();
            var serverTimeMock = new Mock<IServerTime>();

            var accessToken = "AccessToken";
            
            var insideUser = new InsideUser()
            {
                UserId = 1,
                ClientId = 1,
            };
            
            var client = new Client()
            {
                Id = 1,
            };

            client.InsideUserSets.Add(insideUser);

            var startDate = new DateTime(2014, 1, 1, 12, 0, 0);

            for (int i = 1; i <= 23; i++)
            {
                var contact = new Contact()
                {
                    Id = i,
                    ClientId = client.Id,
                    Client = client,
                    Date = startDate.AddHours(-10*i)
                };

                contactRepository.Add(contact);
            }

            tokenGenerator.Setup(tg => tg.Validate((int) insideUser.ClientId, accessToken)).Returns(true);

            var controller = new ExternalController(
                userRepository.Object,
                contactRepository,
                tokenGenerator.Object,
                userManager.Object,
                budgetRepositoryMock.Object,
                clientRepositoryMock.Object,
                serverTimeMock.Object);


            var result = controller.GetContactList((int) insideUser.ClientId, accessToken, 20,0) as OkNegotiatedContentResult<List<Contact>>;
            Assert.AreEqual(20, result.Content.Count());
            Assert.AreEqual(1, result.Content.ToArray()[0].Id);
            Assert.AreEqual(10, result.Content.ToArray()[9].Id);


            var result2 = controller.GetContactList((int)insideUser.ClientId, accessToken, 10, 0) as OkNegotiatedContentResult<List<Contact>>;
            Assert.AreEqual(10, result2.Content.Count());
            Assert.AreEqual(1, result2.Content.ToArray()[0].Id);
            Assert.AreEqual(10, result2.Content.ToArray()[9].Id);

            var result3 = controller.GetContactList((int)insideUser.ClientId, accessToken, 10, 5) as OkNegotiatedContentResult<List<Contact>>;
            Assert.AreEqual(10, result3.Content.Count());
            Assert.AreEqual(6, result3.Content.ToArray()[0].Id);
            Assert.AreEqual(15, result3.Content.ToArray()[9].Id);
        }

        [TestMethod]
        public void SetContactInteractionForReadStatusAndRating()
        {
            var userRepository = new Mock<IRepository<InsideUser>>();
            var contactRepository = new LocalRepository<Contact>();

            var userManager = new Mock<IIdentityMembershipProvider>();
            var tokenGenerator = new Mock<IAccessTokenProvider>();

            var budgetRepositoryMock = new Mock<IRepository<Budget>>();
            var clientRepositoryMock = new Mock<IRepository<Client>>();
            var serverTimeMock = new Mock<IServerTime>();

            var clientId = 13;
            var accessToken = "AccessToken";

            var contact = new Contact()
            {
                Id = 1,
                ClientId = clientId
            };

            contactRepository.Add(contact);

            var interaction1 = new Interaction()
            {
                Type = "Rating",
                Value = "Lead"
            };

            var interaction2 = new Interaction()
            {
                Type = "ReadStatus",
                Value = "Read"
            };

            
            tokenGenerator.Setup(tg => tg.Validate(clientId, accessToken)).Returns(true);

            var controller = new ExternalController(
                userRepository.Object,
                contactRepository,
                tokenGenerator.Object,
                userManager.Object,
                budgetRepositoryMock.Object,
                clientRepositoryMock.Object,
                serverTimeMock.Object);


            var result = controller.SetContactInteraction(clientId, accessToken, contact.Id, interaction1) as OkNegotiatedContentResult<Contact>;

            Assert.IsTrue(
                contactRepository.Where(l => l.Id == l.Id)
                    .First()
                    .Interaction.Any(li => li.Type == "Rating" && li.Value == "Lead"));

            Assert.AreEqual(1,result.Content.Id);
            Assert.AreEqual(1, result.Content.Interaction.Count);
            Assert.AreEqual(1, result.Content.Interaction.Count(li => li.Type=="Rating"));
            Assert.AreEqual("Lead", result.Content.Interaction.Single(li => li.Type == "Rating").Value);

            var result2 = controller.SetContactInteraction(clientId, accessToken, contact.Id, interaction2) as OkNegotiatedContentResult<Contact>;

            Assert.IsTrue(
                contactRepository.Where(l => l.Id == l.Id)
                    .First()
                    .Interaction.Any(li => li.Type == "ReadStatus" && li.Value == "Read"));

            Assert.AreEqual(1, result2.Content.Id);
            Assert.AreEqual(2, result2.Content.Interaction.Count);
            Assert.AreEqual(1, result2.Content.Interaction.Count(li => li.Type == "Rating"));
            Assert.AreEqual("Lead", result2.Content.Interaction.Single(li => li.Type == "Rating").Value);
            Assert.AreEqual("Lead", result2.Content.Interaction.Single(li => li.Type == "Rating").Value);

            Assert.AreEqual(1, result2.Content.Interaction.Count(li => li.Type == "ReadStatus"));
            Assert.AreEqual("Read", result2.Content.Interaction.Single(li => li.Type == "ReadStatus").Value);

        }

        [TestMethod]
        public void SetContactInteractionUpdatesExistingInteraction()
        {
            var userRepository = new Mock<IRepository<InsideUser>>();
            var contactRepository = new LocalRepository<Contact>();

            var userManager = new Mock<IIdentityMembershipProvider>();
            var tokenGenerator = new Mock<IAccessTokenProvider>();

            var budgetRepositoryMock = new Mock<IRepository<Budget>>();
            var clientRepositoryMock = new Mock<IRepository<Client>>();
            var serverTimeMock = new Mock<IServerTime>();

            var accessToken = "AccessToken";
            var clientId = 13;
            var contact = new Contact()
            {
                Id = 1,
                ClientId = clientId
            };

            contactRepository.Add(contact);

            var interaction1 = new ContactInteraction
            {
                Type = "Rating",
                Value = "Lead"
            };

            contact.Interaction.Add(interaction1);

            var interaction2 = new Interaction()
            {
                Type = "Rating",
                Value = "Ej Lead"
            };

            
            tokenGenerator.Setup(tg => tg.Validate(clientId, accessToken)).Returns(true);

            var controller = new ExternalController(
                userRepository.Object,
                contactRepository,
                tokenGenerator.Object,
                userManager.Object,
                budgetRepositoryMock.Object,
                clientRepositoryMock.Object,
                serverTimeMock.Object);


            var result = controller.SetContactInteraction(clientId, accessToken, contact.Id, interaction2) as OkNegotiatedContentResult<Contact>;

            Assert.IsTrue(
                contactRepository.Where(l => l.Id == l.Id)
                    .First()
                    .Interaction.Any(li => li.Type == "Rating" && li.Value == "Ej Lead"));

            Assert.IsFalse(
                contactRepository.Where(l => l.Id == l.Id)
                    .First()
                    .Interaction.Any(li => li.Type == "Rating" && li.Value == "Lead"));
        }


        [TestMethod]
        public void BadAccessTokenReturnsNotAutorized()
        {
            var userRepository = new LocalRepository<InsideUser>();
            var contactRepository = new Mock<IRepository<Contact>>();
            var tokenProvider = new Mock<IAccessTokenProvider>();
            var userManager = new Mock<IIdentityMembershipProvider>();
            var budgetRepositoryMock = new Mock<IRepository<Budget>>();
            var clientRepositoryMock = new Mock<IRepository<Client>>();
            var serverTimeMock = new Mock<IServerTime>();

            var controller = new ExternalController(
                userRepository,
                contactRepository.Object,
                tokenProvider.Object,
                userManager.Object,
                budgetRepositoryMock.Object,
                clientRepositoryMock.Object,
                serverTimeMock.Object);

            

            InsideUser testUser1 = ModelHelper.TestUser1AllDataNoReferences;
            InsideUser testUser2 = ModelHelper.TestUser1AllDataNoReferences;
            
            testUser1.Role.Add(new InsideRole(){Name = "consultant"});
            testUser1.Token.Add(new Token(){AccessToken = "abc123"});

            userRepository.Add(testUser1);
            userRepository.Add(testUser2);

            var result = controller.GetBudgetSummary("bad acces token");

            Assert.IsInstanceOfType(result, typeof(UnauthorizedResult));
        }

        [TestMethod]
        public void GoodAccessTokenReturnsResult()
        {
            var userRepository = new LocalRepository<InsideUser>();
            var contactRepository = new Mock<IRepository<Contact>>();
            var tokenProvider = new Mock<IAccessTokenProvider>();
            var userManager = new Mock<IIdentityMembershipProvider>();
            var budgetRepository = new LocalRepository<Budget>();
            var clientRepository = new LocalRepository<Client>();
            var serverTimeMock = new Mock<IServerTime>();

            var controller = new ExternalController(
                userRepository,
                contactRepository.Object,
                tokenProvider.Object,
                userManager.Object,
                budgetRepository,
                clientRepository,
                serverTimeMock.Object);

            InsideUser testUser1 = ModelHelper.TestUser1AllDataNoReferences;
            InsideUser testUser2 = ModelHelper.TestUser1AllDataNoReferences;

            var client = ModelHelper.TestClient1AllDataNoReferences;
            client.Labels.Add(new Label() { Name = "Kampanjplaneras" });
            client.Consultant = ModelHelper.TestConsultant;

            var client2 = ModelHelper.TestClient2AllDataNoReferences;
            client2.Consultant = ModelHelper.TestConsultant;

            testUser1.Role.Add(new InsideRole() { Name = "consultant" });
            testUser1.Token.Add(new Token() { AccessToken = "good acces token" });

            userRepository.Add(testUser1);
            userRepository.Add(testUser2);

            var jan = new DateTime(2014, 1, 1);
            var feb = new DateTime(2014, 2, 1);
            var mars = new DateTime(2014, 3, 1);

            serverTimeMock.Setup(s => s.RequestStarted).Returns(jan.AddDays(10));


            Budget budget1 = new Budget() { Month = feb, Value = 1500 }; ;
            Budget budget2 = new Budget() { Month = jan, Value = 2000 };
            budget1.Client = client;
            budget2.Client = client;

            budgetRepository.Add(budget1);
            budgetRepository.Add(budget2);

            clientRepository.Add(client);
            clientRepository.Add(client2);

            client.Budgets.Add(budget1);
            client.Budgets.Add(budget2);
            
            Budget budget3 = new Budget() { Month = feb, Value = 1000 };
            Budget budget4 = new Budget() { Month = jan, Value = 3000 };
            client.Budgets.Add(budget3);
            client.Budgets.Add(budget4);
            budget3.Client = client2;

            budgetRepository.Add(budget4);
            budgetRepository.Add(budget3);


            var result = controller.GetBudgetSummary("good acces token") as OkNegotiatedContentResult<BudgetMonthSummary>;

            Assert.AreEqual(-500, result.Content.diff);
            Assert.AreEqual(1, result.Content.numClients);
            Assert.AreEqual(1, result.Content.TotalClients);

        }
    }
}

