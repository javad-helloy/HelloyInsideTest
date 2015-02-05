using System;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Security.Principal;
using System.Web.Http.Results;
using Helpers.test;
using Inside.Time;
using InsideModel.Models;
using InsideModel.repositories;
using InsideReporting.Controllers.External;
using Mandrill;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Task.Email.Sender;
using Moq;

namespace InsideReporting.Tests.Controllers
{
    [TestClass]
    public class SocialControllerTest
    {
        [TestMethod]
        public void CanConstruct()
        {
            var contactRepository = new Mock<IRepository<Contact>>();
            var userRepository = new  Mock<IRepository<InsideUser>>();
            var emailSender = new Mock<IEmailSender>();
            var serverTime = new Mock<IServerTime>();

            var controller = new SocialController(
                contactRepository.Object, 
                userRepository.Object, 
                emailSender.Object, 
                serverTime.Object);
        }

        [TestMethod]
        public void GeneratesAndSendsEmail()
        {
            var contactRepository = new LocalRepository<Contact>();
            var userRepository = new LocalRepository<InsideUser>();
            var emailSender = new Mock<IEmailSender>(MockBehavior.Strict);
            var serverTime = new ServerTime();

            var client = new Client();
            client.Name = "Kund";
            client.Id = 8;

            var contact = new Contact();
            contact.LeadType = "Chat";
            contact.Id = 14;
            contact.Date = new DateTime(2014,6,13,14,22,9);
            contact.Client = client;
            contact.ClientId = client.Id;

            var currentUser = new InsideUser();
            currentUser.Email = "linus@helloy.se";
            currentUser.Name = "Linus Lind";
            currentUser.Id = "currentUserId";
            currentUser.Client = client;
            currentUser.ClientId = client.Id;

            contactRepository.Add(contact);
            userRepository.Add(currentUser);

            var controller = new SocialController(
                contactRepository,
                userRepository,
                emailSender.Object,
                serverTime);

            GenericIdentity genericIdentity = new GenericIdentity("user");
            genericIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, "currentUserId"));
            controller.User = new ClaimsPrincipal(
                new GenericPrincipal(genericIdentity, null));


            Expression<Action<IEmailSender>> emailMatch = es => es.Send(
                It.Is<EmailMessage>(
                    em => 
                        em.html.Contains(
                            "Linus Lind har delat en kundkontakt från Helloy med dig"
                            ) && em.html.Contains(
                                "08 123 456 den 13 juni 2014 16:22"
                                ) && em.html.Contains(
                                    "Message<br/>on two rows"
                                    ) && em.to.Any(t => t.email == "linus@helloy.se")
                        && em.from_email == "linus@helloy.se"

                    ));
            emailSender.Setup(emailMatch);

            var sendEmail = controller.SendContact(14, "linus@helloy.se", "08 123 456", "Message\non two rows");

            emailSender.Verify(emailMatch, Times.Once);
        }

        [TestMethod]
        public void ReturnsNotFoundForMissingContact()
        {
            var contactRepository = new LocalRepository<Contact>();
            var userRepository = new LocalRepository<InsideUser>();
            var emailSender = new Mock<IEmailSender>();
            var serverTime = new ServerTime();

            var contact = new Contact();
            contact.LeadType = "Chat";
            contact.Id = 14;
            contact.Date = new DateTime(2014, 6, 13, 14, 22, 9);

            var currentUser = new InsideUser();
            currentUser.Email = "linus@helloy.se";
            currentUser.Name = "Linus Lind";
            currentUser.Id = "currentUserId";

            contactRepository.Add(contact);
            userRepository.Add(currentUser);

            var controller = new SocialController(
                contactRepository,
                userRepository,
                emailSender.Object,
                serverTime);

            GenericIdentity genericIdentity = new GenericIdentity("user");
            genericIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, "currentUserId"));
            controller.User = new ClaimsPrincipal(
                new GenericPrincipal(genericIdentity, null));

            var results = controller.SendContact(15, "linus@helloy.se", "08 123 456", "Message\non two rows");

            Assert.IsInstanceOfType(results, typeof(NotFoundResult));
        }

        [TestMethod]
        public void ThrowsExceptionIfCurrentUserDoesNotHaveAccess()
        {
            var contactRepository = new LocalRepository<Contact>();
            var userRepository = new LocalRepository<InsideUser>();
            var emailSender = new Mock<IEmailSender>();
            var serverTime = new ServerTime();

            var client = new Client();
            client.Name = "Kund";
            client.Id = 8;

            var client2 = new Client();
            client2.Name = "Kund 2";
            client2.Id = 18;


            var contact = new Contact();
            contact.LeadType = "Chat";
            contact.Id = 14;
            contact.Date = new DateTime(2014, 6, 13, 14, 22, 9);
            contact.Client = client;
            contact.ClientId = 8;

            var currentUser = new InsideUser();
            currentUser.Email = "linus@helloy.se";
            currentUser.Name = "Linus Lind";
            currentUser.Id = "currentUserId";
            currentUser.Client = client2;
            currentUser.ClientId = 18;

            contactRepository.Add(contact);
            userRepository.Add(currentUser);

            var controller = new SocialController(
                contactRepository,
                userRepository,
                emailSender.Object,
                serverTime);

            GenericIdentity genericIdentity = new GenericIdentity("user");
            genericIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, "currentUserId"));
            controller.User = new ClaimsPrincipal(
                new GenericPrincipal(genericIdentity, null));

            var results = controller.SendContact(14, "linus@helloy.se", "08 123 456", "Message\non two rows") as UnauthorizedResult;

            Assert.IsInstanceOfType(results, typeof(UnauthorizedResult));
        }
    }
}
