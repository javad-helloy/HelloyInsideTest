using System.Security.Claims;
using System.Web.Mvc;
using Helpers.test;
using Inside.membership;
using InsideModel.Models;
using InsideModel.repositories;
using InsideReporting.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace InsideReporting.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {

        [TestMethod]
        public void CanCreate()
        {
            var insideUserRepositoryMock = new Mock<IRepository<InsideUser>>();
            var insideUserManagerMock= new Mock<IIdentityMembershipProvider>();

            var controller = new HomeController(insideUserRepositoryMock.Object, insideUserManagerMock.Object);    
        }

        [TestMethod]
        public void UserInDemoIsRedirectedToContactList()
        {
            var insideUserRepositoryMock = new Mock<IRepository<InsideUser>>();
            var insideUserManagerMock = new Mock<IIdentityMembershipProvider>();
            var controller = new HomeController(insideUserRepositoryMock.Object,insideUserManagerMock.Object);

            var claim = new Claim("test","AnyId");
            
            insideUserManagerMock.Setup(um => um.IsInRole(It.IsAny<string>(), "demo")).Returns(true);
            ModelHelper.SetClaimAsCurrentUserForController(claim, controller);
            var result = controller.Index() as RedirectToRouteResult;

            Assert.AreEqual("reportcontacts", result.RouteName);
        }

        [TestMethod]
        public void UserInConsultantIsRedirectedToDemo()
        {
            var insideUserRepositoryMock = new Mock<IRepository<InsideUser>>();
            var insideUserManagerMock = new Mock<IIdentityMembershipProvider>();
            var controller = new HomeController(insideUserRepositoryMock.Object, insideUserManagerMock.Object);
            
            var claim = new Claim("test", "AnyId");
            ModelHelper.SetClaimAsCurrentUserForController(claim, controller);
            insideUserManagerMock.Setup(um => um.IsInRole(It.IsAny<string>(), "consultant")).Returns(true);
            var result = controller.Index() as RedirectToRouteResult;

            Assert.IsTrue(result.RouteValues.ContainsValue("Client"));
            Assert.IsTrue(result.RouteValues.ContainsValue("Index"));
        }

        [TestMethod]
        public void UserClientRedirectsToReport()
        {
            var insideUserRepositoryMock = new LocalRepository<InsideUser>();
            var insideUserManagerMock = new Mock<IIdentityMembershipProvider>();
            var insideUser = ModelHelper.TestUser1AllDataNoReferences;
            var client = ModelHelper.TestClient1AllDataNoReferences;
            insideUser.Client = client;

            insideUserRepositoryMock.Add(insideUser);

            var claim = new Claim("test", insideUser.Id);
            

            insideUserManagerMock.Setup(um => um.IsInRole(insideUser.Id, "client")).Returns(true);
            insideUserManagerMock.Setup(um => um.IsInRole(insideUser.Id, "consultant")).Returns(false);

            var controller = new HomeController( insideUserRepositoryMock, insideUserManagerMock.Object);
            ModelHelper.SetClaimAsCurrentUserForController(claim, controller);

            var result = controller.Index() as RedirectToRouteResult;

            Assert.AreEqual(insideUser.ClientId, result.RouteValues["clientID"]);

        }
    }
}
    