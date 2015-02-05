using System;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Policy;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Helpers.test;
using Inside.AutoRating;
using Inside.Time;
using InsideModel.Models;
using InsideModel.repositories;
using InsideReporting.Controllers.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace InsideReporting.Tests.Controllers.Data
{
    [TestClass]
    public class TrackControllerTest
    {
        [TestMethod]
        public void CanConstruct()
        {
            var contactRepository = new Mock<IRepository<Contact>>();
            var serverTime = new Mock<IServerTime>();
            var contactAutoRating = new Mock<IContactAutoRating>();

            var controller = new TrackController(
                contactRepository.Object,
                serverTime.Object,
                contactAutoRating.Object);
        }

        [TestMethod]
        public void CanCreateContactForOkData()
        {
            var contactRepository = new LocalRepository<Contact>();
            var serverTime = new Mock<IServerTime>();
            var contactAutoRating = new Mock<IContactAutoRating>();

            var controller = new TrackController(
                contactRepository,
                serverTime.Object,
                contactAutoRating.Object);
            var now = new DateTime(2014, 01, 01);
            serverTime.Setup(st => st.Now).Returns(now);
            var request = new Mock<HttpRequestBase>();

            var requestParams = new NameValueCollection
            {
                { "id", "1-123456-0987"},
                { "data","{\"firstName\":\"javad\",\"Telefon\":\"0123456\",\"e-post\":\"javad@a.b\",\"description\":\"information\"}"}
            };
            request.Setup(r => r.Params).Returns(requestParams);

            var server = new Mock<HttpServerUtilityBase>();
            server.Setup(su => su.MapPath("/Content/collect.gif")).Returns("~/Content/collect.gif");
            var context = new Mock<HttpContextBase>();

            context.SetupGet(x => x.Request).Returns(request.Object);
            context.SetupGet(x => x.Server).Returns(server.Object);

            controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);


            var result = controller.Index() ;


            Assert.AreEqual(1,contactRepository.All().Count());
            Assert.AreEqual(now, contactRepository.All().First().Date);
            Assert.AreEqual("Email", contactRepository.All().First().LeadType);
            Assert.AreEqual(1, contactRepository.All().First().ClientId);
            Assert.AreEqual("{\"firstName\":\"javad\",\"Telefon\":\"0123456\",\"e-post\":\"javad@a.b\",\"description\":\"information\"}",
                contactRepository.All().First().Property.Single(p => p.Type == "FormData").Value);
            Assert.AreEqual("1-123456-0987", contactRepository.All().First().Property.Single(p => p.Type == "FormPostId").Value);
            Assert.AreEqual("0123456", contactRepository.All().First().Property.Single(p => p.Type == "FromPhone").Value);
            Assert.AreEqual("javad@a.b", contactRepository.All().First().Property.Single(p => p.Type == "FromEmail").Value);
        }


        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void UnRetrievableFormIdThrowsException()
        {
            var contactRepository = new Mock<IRepository<Contact>>();
            var serverTime = new Mock<IServerTime>();
            var contactAutoRating = new Mock<IContactAutoRating>();

            var controller = new TrackController(
                contactRepository.Object,
                serverTime.Object,
                contactAutoRating.Object);
            var now = new DateTime(2014, 01, 01);
            serverTime.Setup(st => st.Now).Returns(now);
            
            var request = new Mock<HttpRequestBase>();
            var requestParams = new NameValueCollection
            {
                { "invalidId", "1-123456-0987"},
                { "data","{\"firstName\":\"javad\",\"Telefon\":\"0123456\",\"e-post\":\"javad@a.b\",\"description\":\"information\"}"}
            };
            request.Setup(r => r.Params).Returns(requestParams);

            var server = new Mock<HttpServerUtilityBase>();
            server.Setup(su => su.MapPath("/Content/collect.gif")).Returns("~/Content/collect.gif");
            var context = new Mock<HttpContextBase>();

            context.SetupGet(x => x.Request).Returns(request.Object);
            context.SetupGet(x => x.Server).Returns(server.Object);

            controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);


            var result = controller.Index();

            contactRepository.Verify(cr=>cr.Add(It.IsAny<Contact>()), Times.Never);
            contactRepository.Verify(cr => cr.SaveChanges(), Times.Never);
        }

        [TestMethod]
        public void ContactInDbDoesNotCreateDuplicate()
        {
            var contactRepository = new LocalRepository<Contact>();
            var serverTime = new Mock<IServerTime>();
            var contactAutoRating = new Mock<IContactAutoRating>();

            var controller = new TrackController(
                contactRepository,
                serverTime.Object,
                contactAutoRating.Object);
            var now = new DateTime(2014, 01, 01);
            serverTime.Setup(st => st.Now).Returns(now);
            var request = new Mock<HttpRequestBase>();

            var contact = new Contact
            {
                LeadType = "Email",
                ClientId = 1,
                Date = now, 
            };
            contact.Property.Add(new ContactProperty
            {
                Type = "FormData",
                Value = "{\"firstName\":\"javad\",\"Telefon\":\"0123456\",\"e-post\":\"javad@a.b\",\"description\":\"information\"}"
            });

            contact.Property.Add(new ContactProperty
            {
                Type = "FormPostId",
                Value = "1-123456-0987"
            });
            contactRepository.Add(contact);

            var requestParams = new NameValueCollection
            {
                { "id", "1-123456-0987"},
                { "data","{\"firstName\":\"javad\",\"Telefon\":\"0123456\",\"e-post\":\"javad@a.b\",\"description\":\"information\"}"}
            };
            request.Setup(r => r.Params).Returns(requestParams);

            var server = new Mock<HttpServerUtilityBase>();
            server.Setup(su => su.MapPath("/Content/collect.gif")).Returns("~/Content/collect.gif");
            var context = new Mock<HttpContextBase>();

            context.SetupGet(x => x.Request).Returns(request.Object);
            context.SetupGet(x => x.Server).Returns(server.Object);

            controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);


            var result = controller.Index();


            Assert.AreEqual(1, contactRepository.All().Count());
        }


    }
}
