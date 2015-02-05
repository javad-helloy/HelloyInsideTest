using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web.Mvc;
using Helpers.test;
using Inside.AutoRating;
using Inside.ContactService;
using Inside.membership;
using Inside.Time;
using InsideModel.Models;
using InsideModel.repositories;
using InsideReporting.Controllers.Data;
using InsideReporting.Models;
using InsideReporting.Models.Chat;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RestSharp;

namespace InsideReporting.Tests.Controllers.Data
{
    [TestClass]
    public class ChatControllerTest
    {
        [TestMethod]
        public void CanConstruct()
        {
            var clientRepository = new Mock<IRepository<Client>>();
            var chatRepository = new Mock<IRepository<Contact>>();
            var contactService = new Mock<IContactService>();
            var restClient = new Mock<IRestClient>();
            var userManager = new Mock<IIdentityMembershipProvider>();
            var contactAutoRating = new Mock<IContactAutoRating>();

            var controller = new ChatController(
                chatRepository.Object, 
                clientRepository.Object,
                contactService.Object,
                new ServerTime(),
                restClient.Object,
                userManager.Object, contactAutoRating.Object);

        }

        [TestMethod]
        public void IndexCanShowData()
        {
            var clientRepository = RepositoryHelper.RepositoryLocalWithOneClient;
            var client = clientRepository.All().First();
            var chatRepository = new LocalRepository<Contact>();
            var contactService = new Mock<IContactService>();
            var restClient = new Mock<IRestClient>();
            var userManager = new Mock<IIdentityMembershipProvider>();
            var contactAutoRating = new Mock<IContactAutoRating>();

            var chat = new Contact()
            {
                Id = 1,
                ClientId = client.Id,
                LeadType = "Chat",
                Date = new DateTime(2014,01,01,12,00,00)
            };

            var chatProperty = new ContactProperty()
            {
                Id = 1,
                Type = "Description",
                Value = "this is the description"
            };
            var chatProperty2 = new ContactProperty()
            {
                Id = 1,
                Type = "Email",
                Value = "test@email.com"
            };
            var chatProperty3 = new ContactProperty()
            {
                Id = 1,
                Type = "Phone",
                Value = "08123456"
            };

            chat.Property.Add(chatProperty);
            chat.Property.Add(chatProperty2);
            chat.Property.Add(chatProperty3);
            chatRepository.Add(chat);

            var claim = new Claim("test", "AnyId");
            userManager.Setup(um => um.GetRoles(It.IsAny<string>())).Returns(new List<string> { "mockRole" });

            var controller = new ChatController(
                chatRepository, 
                clientRepository,
                contactService.Object,
                new ServerTime(),
                restClient.Object, userManager.Object, contactAutoRating.Object);

            ModelHelper.SetClaimAsCurrentUserForController(claim, controller);
            var result = controller.Index() as ViewResult;
            var model = result.Model as ChatListViewModel;
            
            Assert.AreEqual(client.Name, model.Collection.Single(m => m.Id == chat.Id).ClientName);
            Assert.AreEqual("this is the description", model.Collection.Single(m => m.Id == chat.Id).Description);
            Assert.AreEqual("test@email.com", model.Collection.Single(m => m.Id == chat.Id).Email);
            Assert.AreEqual("08123456", model.Collection.Single(m => m.Id == chat.Id).Phone);
            Assert.AreEqual("13:00", model.Collection.Single(m => m.Id == chat.Id).Date.ToString("HH:mm"));

        }

        [TestMethod]
        public void CreateCanMakeClientIdsViewBag()
        {
            var clientRepository = RepositoryHelper.RepositoryLocalWithOneClient;
            var chatRepository = new LocalRepository<Contact>();
            clientRepository.Add(new Client(){Id = 10, IsActive = false});
            var contactService = new Mock<IContactService>();
            var restClient = new Mock<IRestClient>();
            var userManager = new Mock<IIdentityMembershipProvider>();
            var contactAutoRating = new Mock<IContactAutoRating>();


            var claim = new Claim("test", "AnyId");
            
            userManager.Setup(um => um.GetRoles(It.IsAny<string>())).Returns(new List<string> { "mockRole" });

            var controller = new ChatController(
                chatRepository, 
                clientRepository,
                contactService.Object,
                new ServerTime(),
                restClient.Object, userManager.Object, contactAutoRating.Object);

            ModelHelper.SetClaimAsCurrentUserForController(claim, controller);
            var result = controller.Create() as ViewResult;

            var clients = result.ViewBag.ClientIds as SelectList;

            Assert.AreEqual(clientRepository.Where(cr=>cr.IsActive).Count(), clients.Count());

        }

        [TestMethod]
        public void CreateCanMakeNewChat()
        {
            var clientRepository = RepositoryHelper.RepositoryLocalWithOneClient;
            var chatRepository = new LocalRepository<Contact>();
            var client = clientRepository.All().First();
            var contactService = new Mock<IContactService>();
            var restClient = new Mock<IRestClient>();
            var userManager = new Mock<IIdentityMembershipProvider>();
            var contactAutoRating = new Mock<IContactAutoRating>();

            var chat = new ChatViewModel
            {
                ClientId = client.Id,
                ClientName = client.Name,
                Date = new DateTime(2013, 1, 02),
                Description = "this is the description",
                LiveChatId = "LC123456",
                Email = "test2@email.com",
                Phone = "08654321"
            };

            var controller = new ChatController(
                chatRepository, 
                clientRepository,
                contactService.Object,
                new ServerTime(),
                restClient.Object, userManager.Object, contactAutoRating.Object);
            var result = controller.Create(chat) as ViewResult;

            Assert.AreEqual(1,chatRepository.All().Count());
            Assert.AreEqual("this is the description", chatRepository.Where(c => c.Id == chat.Id).First().Property.Single(lp => lp.Type == "Description").Value);
            Assert.AreEqual("test2@email.com", chatRepository.Where(c => c.Id == chat.Id).First().Property.Single(lp => lp.Type == "Email").Value);
            Assert.AreEqual("08654321", chatRepository.Where(c => c.Id == chat.Id).First().Property.Single(lp => lp.Type == "Phone").Value);
            Assert.AreEqual("LC123456", chatRepository.Where(c => c.Id == chat.Id).First().Property.Single(lp => lp.Type == "LiveChatId").Value);
        }

        [TestMethod]
        public void CreateMakesNewChatAndIgnoresNullProperties()
        {
            var clientRepository = RepositoryHelper.RepositoryLocalWithOneClient;
            var chatRepository = new LocalRepository<Contact>();
            var client = clientRepository.All().First();
            var contactService = new Mock<IContactService>();
            var restClient = new Mock<IRestClient>();
            var userManager = new Mock<IIdentityMembershipProvider>();
            var contactAutoRating = new Mock<IContactAutoRating>();

            var chat = new ChatViewModel
            {
                ClientId = client.Id,
                ClientName = client.Name,
                Date = new DateTime(2013, 1, 02),
                Description = "this is the description",
                LiveChatId = "LC123456",
                Email = "",
                Phone = "08654321"
            };

            var controller = new ChatController(
                chatRepository,
                clientRepository,
                contactService.Object,
                new ServerTime(),
                restClient.Object, userManager.Object, contactAutoRating.Object);
            var result = controller.Create(chat) as ViewResult;

            Assert.AreEqual(1, chatRepository.All().Count());
            Assert.AreEqual("this is the description", chatRepository.Where(c => c.Id == chat.Id).First().Property.Single(lp => lp.Type == "Description").Value);
            Assert.AreEqual("08654321", chatRepository.Where(c => c.Id == chat.Id).First().Property.Single(lp => lp.Type == "Phone").Value);
            Assert.AreEqual("LC123456", chatRepository.Where(c => c.Id == chat.Id).First().Property.Single(lp => lp.Type == "LiveChatId").Value);
            Assert.IsFalse(chatRepository.Where(c => c.Id == chat.Id).First().Property.Any(lp => lp.Type == "Email"));
        }

        [TestMethod]
        public void CreateWithInvalidModelStateNotChangeRepository()
        {
            var clientRepository = RepositoryHelper.RepositoryLocalWithOneClient;
            var chatRepository = new LocalRepository<Contact>();
            var client = clientRepository.All().First();
            var contactService = new Mock<IContactService>();
            var restClient = new Mock<IRestClient>();
            var userManager = new Mock<IIdentityMembershipProvider>();
            var contactAutoRating = new Mock<IContactAutoRating>();

            var chat = new ChatViewModel
            {
                ClientId = client.Id,
                ClientName = client.Name,
                Date = new DateTime(2013 - 1 - 02),
                Description = "this is the description"
            };

            var controller = new ChatController(
                chatRepository, 
                clientRepository,
                contactService.Object,
                new ServerTime(),
                restClient.Object, userManager.Object, contactAutoRating.Object);
            controller.ModelState.AddModelError("error","SomeError");
            var result = controller.Create(chat) as ViewResult;

            Assert.AreEqual(0, chatRepository.All().Count());

        }

        [TestMethod]
        public void EditChatGetShowsCorrectData()
        {
            var clientRepository = RepositoryHelper.RepositoryLocalWithOneClient;
            var chatRepository = new LocalRepository<Contact>();
            var client = clientRepository.All().First();
            var contactService = new Mock<IContactService>();
            var restClient = new Mock<IRestClient>();
            var userManager = new Mock<IIdentityMembershipProvider>();
            var contactAutoRating = new Mock<IContactAutoRating>();

            var chat = new Contact()
            {
                Id = 1,
                ClientId = client.Id,
                Date = new DateTime(2013 - 1 - 02)
            };

            var chatProperty = new ContactProperty()
            {
                Id = 1,
                Type = "Description",
                Value = "this is the description"
            };
            var chatProperty2 = new ContactProperty()
            {
                Id = 1,
                Type = "Email",
                Value = "test@email.com"
            };
            var chatProperty3 = new ContactProperty()
            {
                Id = 1,
                Type = "Phone",
                Value = "08123456"
            };

            chat.Property.Add(chatProperty);
            chat.Property.Add(chatProperty2);
            chat.Property.Add(chatProperty3);
            chatRepository.Add(chat);

            var claim = new Claim("test", "AnyId");
            
            userManager.Setup(um => um.GetRoles(It.IsAny<string>())).Returns(new List<string> { "mockRole" });

            var controller = new ChatController(
                chatRepository, 
                clientRepository,
                contactService.Object,
                new ServerTime(),
                restClient.Object, userManager.Object, contactAutoRating.Object);

            ModelHelper.SetClaimAsCurrentUserForController(claim, controller);
            var result = controller.Edit(chat.Id) as ViewResult;
            var model = result.Model as ChatViewModel;

            Assert.AreEqual(model.ClientName,client.Name);
            Assert.AreEqual(model.Id, chat.Id);
            Assert.AreEqual("test@email.com", model.Email);
            Assert.AreEqual("08123456", model.Phone);

        }

        [TestMethod]
        public void EditChatPostModifiesDataInRepositoryWithLiveChatUpdate()
        {
            var clientRepository = RepositoryHelper.RepositoryLocalWithOneClient;
            var chatRepository = new LocalRepository<Contact>();
            var client = clientRepository.All().First();
            var contactService = new Mock<IContactService>();
            var restClient = new Mock<IRestClient>();
            var userManager = new Mock<IIdentityMembershipProvider>();
            var contactAutoRating = new Mock<IContactAutoRating>();

            var chat = new Contact()
            {
                Id = 1,
                ClientId = client.Id,
                Date = new DateTime(2013 - 1 - 02)
            };

            var chatProperty = new ContactProperty()
            {
                Id = 1,
                Type = "Description",
                Value = "this is the description"
            };
            var chatProperty2 = new ContactProperty()
            {
                Id = 1,
                Type = "Email",
                Value = "test@email.com"
            };
            var chatProperty3 = new ContactProperty()
            {
                Id = 1,
                Type = "Phone",
                Value = "08123456"
            };

            chat.Property.Add(chatProperty);
            chat.Property.Add(chatProperty2);
            chat.Property.Add(chatProperty3);
            chatRepository.Add(chat);

            var chatViewModel = new ChatViewModel()
            {
                Id = 1,
                ClientId = client.Id,
                ClientName = client.Name,
                Date = new DateTime(2013 - 1 - 02),
                Description = "Description is Edited",
                Email = "test2@email.com",
                Phone = "08654321",
                LiveChatId = "LC123456"
            };

            var controller = new ChatController(
                chatRepository, 
                clientRepository,
                contactService.Object,
                new ServerTime(),
                restClient.Object, userManager.Object, contactAutoRating.Object);

            var result = controller.Edit(chatViewModel) as ViewResult;
            
            Assert.AreEqual(1,chatRepository.All().Count());
            Assert.AreEqual("Description is Edited", chatRepository.Where(c => c.Id == chat.Id).First().Property.Single(lp => lp.Type == "Description").Value);
            Assert.AreEqual("test2@email.com", chatRepository.Where(c => c.Id == chat.Id).First().Property.Single(lp => lp.Type == "Email").Value);
            Assert.AreEqual("08654321", chatRepository.Where(c => c.Id == chat.Id).First().Property.Single(lp => lp.Type == "Phone").Value);
            Assert.AreEqual("LC123456", chatRepository.Where(c => c.Id == chat.Id).First().Property.Single(lp => lp.Type == "LiveChatId").Value);

        }

        [TestMethod]
        public void EditChatPostModifiesDataInRepositoryAndIgnoresNullValues()
        {
            var clientRepository = RepositoryHelper.RepositoryLocalWithOneClient;
            var chatRepository = new LocalRepository<Contact>();
            var client = clientRepository.All().First();
            var contactService = new Mock<IContactService>();
            var restClient = new Mock<IRestClient>();
            var userManager = new Mock<IIdentityMembershipProvider>();
            var contactAutoRating = new Mock<IContactAutoRating>();

            var chat = new Contact()
            {
                Id = 1,
                ClientId = client.Id,
                Date = new DateTime(2013 - 1 - 02)
            };

            var chatProperty = new ContactProperty()
            {
                Id = 1,
                Type = "Description",
                Value = "this is the description"
            };
            var chatProperty2 = new ContactProperty()
            {
                Id = 1,
                Type = "Email",
                Value = "test@email.com"
            };
            var chatProperty3 = new ContactProperty()
            {
                Id = 1,
                Type = "Phone",
                Value = "08123456"
            };

            var chatProperty4 = new ContactProperty()
            {
                Id = 1,
                Type = "LiveChatId",
                Value = "LC123456"
            };

            chat.Property.Add(chatProperty);
            chat.Property.Add(chatProperty2);
            chat.Property.Add(chatProperty3);
            chat.Property.Add(chatProperty4);
            chatRepository.Add(chat);

            var chatViewModel = new ChatViewModel()
            {
                Id = 1,
                ClientId = client.Id,
                ClientName = client.Name,
                Date = new DateTime(2013 - 1 - 02),
                Description = "Description is Edited",
                Email = "test2@email.com",
                Phone = "08654321",
                LiveChatId=""
            };

            var controller = new ChatController(
                chatRepository,
                clientRepository,
                contactService.Object,
                new ServerTime(),
                restClient.Object, userManager.Object, contactAutoRating.Object);

            var result = controller.Edit(chatViewModel) as ViewResult;

            Assert.AreEqual(1, chatRepository.All().Count());
            Assert.AreEqual("Description is Edited", chatRepository.Where(c => c.Id == chat.Id).First().Property.Single(lp => lp.Type == "Description").Value);
            Assert.AreEqual("test2@email.com", chatRepository.Where(c => c.Id == chat.Id).First().Property.Single(lp => lp.Type == "Email").Value);
            Assert.AreEqual("08654321", chatRepository.Where(c => c.Id == chat.Id).First().Property.Single(lp => lp.Type == "Phone").Value);
            Assert.AreEqual(1, chatRepository.Where(c => c.Id == chat.Id).First().Property.Count(lp => lp.Type == "LiveChatId"));
            Assert.AreEqual("LC123456", chatRepository.Where(c => c.Id == chat.Id).First().Property.Single(lp => lp.Type == "LiveChatId").Value);
            

        }

        [TestMethod]
        public void EditChatPostWithInvalidModelStateNotModifiesRepository()
        {
            var clientRepository = RepositoryHelper.RepositoryLocalWithOneClient;
            var chatRepository = new Mock<IRepository<Contact>>();
            var client = clientRepository.All().First();
            var contactService = new Mock<IContactService>();
            var restClient = new Mock<IRestClient>();
            var userManager = new Mock<IIdentityMembershipProvider>();
            var contactAutoRating = new Mock<IContactAutoRating>();


            var chat = new ChatViewModel()
            {
                Id = 1,
                ClientId = client.Id,
                ClientName = client.Name,
                Date = new DateTime(2013 - 1 - 02),
                Description = "this is the description"
            };

            var controller = new ChatController(
                chatRepository.Object,
                clientRepository,
                contactService.Object,
                new ServerTime(),
                restClient.Object, userManager.Object, contactAutoRating.Object);
            controller.ModelState.AddModelError("error", "SomeError");
            var result = controller.Edit(chat) as ViewResult;

            chatRepository.Verify(cr=>cr.SaveChanges(),Times.Never);
        }

        [TestMethod]
        public void DeleteChatsGetShowsCorrectData()
        {
            var clientRepository = RepositoryHelper.RepositoryLocalWithOneClient;
            var chatRepository = new LocalRepository<Contact>();
            var client = clientRepository.All().First();
            var contactService = new Mock<IContactService>();
            var restClient = new Mock<IRestClient>();
            var userManager = new Mock<IIdentityMembershipProvider>();
            var contactAutoRating = new Mock<IContactAutoRating>();


            var chat = new Contact
            {
                Id = 1,
                ClientId = client.Id,
                Date = new DateTime(2013 - 1 - 02)
            };
            var chatProperty = new ContactProperty()
            {
                Id = 1,
                Type = "Description",
                Value = "this is the description"
            };
            var chatProperty2 = new ContactProperty()
            {
                Id = 1,
                Type = "Email",
                Value = "test@email.com"
            };
            var chatProperty3 = new ContactProperty()
            {
                Id = 1,
                Type = "Phone",
                Value = "08123456"
            };

            chat.Property.Add(chatProperty);
            chat.Property.Add(chatProperty2);
            chat.Property.Add(chatProperty3);
            chatRepository.Add(chat);

            var claim = new Claim("test", "AnyId");
            
            userManager.Setup(um => um.GetRoles(It.IsAny<string>())).Returns(new List<string> { "mockRole" });

            var controller = new ChatController(
                chatRepository,
                clientRepository,
                contactService.Object,
                new ServerTime(),
                restClient.Object, userManager.Object, contactAutoRating.Object);

            ModelHelper.SetClaimAsCurrentUserForController(claim, controller);

            var result = controller.Delete(chat.Id) as ViewResult;
            var model = result.Model as ChatViewModel;

            Assert.AreEqual(client.Name, model.ClientName);
            Assert.AreEqual(chat.Id, model.Id);
            Assert.AreEqual("test@email.com", model.Email);
            Assert.AreEqual("08123456", model.Phone);
        }

        [TestMethod]
        public void DeleteConfirmedChatsPostRemovesFromRepository()
        {
            var clientRepository = RepositoryHelper.RepositoryLocalWithOneClient;
            var chatRepository = new LocalRepository<Contact>();
            var client = clientRepository.All().First();
            var contactService = new Mock<IContactService>();
            var restClient = new Mock<IRestClient>();
            var userManager = new Mock<IIdentityMembershipProvider>();
            var contactAutoRating = new Mock<IContactAutoRating>();

            var chat = new Contact
            {
                Id = 1,
                ClientId = client.Id,
                Date = new DateTime(2013 - 1 - 02)
            };
            var chatProperty = new ContactProperty()
            {
                Id = 1,
                Type = "Description",
                Value = "this is the description"
            };

            chat.Property.Add(chatProperty);

            chatRepository.Add(chat);
            var controller = new ChatController(
                chatRepository,
                clientRepository,
                contactService.Object,
                new ServerTime(),
                restClient.Object, userManager.Object, contactAutoRating.Object);

            var result = controller.DeleteConfirmed(chat.Id) as RedirectResult;
            

            Assert.AreEqual(0, chatRepository.All().Count());
           
        }

        [TestMethod]
        public void CanGetChatMessagesFromExternalChatProviderApi()
        {
            var clientRepository = new Mock<IRepository<Client>>();
            var chatRepository = new Mock<IRepository<Contact>>();
            var contactService = new Mock<IContactService>();
            var restClient = new Mock<IRestClient>();
            var userManager = new Mock<IIdentityMembershipProvider>();
            var contactAutoRating = new Mock<IContactAutoRating>();

            var pageNumber = 1;
            var dateFrom = new DateTime(2014 - 01 - 01);
            var controller = new ChatController(
                chatRepository.Object,
                clientRepository.Object,
                contactService.Object,
                new ServerTime(),
                restClient.Object, userManager.Object, contactAutoRating.Object);

            var responseMessage = new RestResponse();
            responseMessage.Content = "{\"chats\":[{\"text\":\"some text\"}]}";
            restClient.Setup(rc => rc.Execute(It.IsAny<RestRequest>())).Returns(responseMessage);

            var result = controller.ChatMessages(pageNumber,dateFrom) as ContentResult;

            restClient.Verify(rc=>rc.Execute(It.Is<RestRequest>(rq => rq.Parameters.Any())),Times.Once);
            Assert.AreEqual("{\"chats\":[{\"text\":\"some text\"}]}",result.Content);
        }

        


    }
}
