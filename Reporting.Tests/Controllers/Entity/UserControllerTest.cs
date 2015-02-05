using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web.Mvc;
using Helpers.test;
using Inside.membership;
using InsideModel.Models;
using InsideModel.repositories;
using InsideReporting.Controllers.Enity;
using InsideReporting.Models.User;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace InsideReporting.Tests.Controllers.Entity
{
    [TestClass]
    public class UserControllerTest
    {
        [TestMethod]
        public void CanCreate()
        {
            var clientRepository = new Mock<IRepository<Client>>();
            var userRepository = new Mock<IRepository<InsideUser>>();
            var userManager = new Mock<IIdentityMembershipProvider>();

            var controller = new UserController(clientRepository.Object, userRepository.Object, userManager.Object);
        }

        [TestMethod]
        public void CreateWithNoPasswordAddsErrorAndReturnsView()
        {
            var clientRepository = RepositoryHelper.RepositoryLocalWithOneClient;
            var client = clientRepository.All().First();
            var userRepository = new LocalRepository<InsideUser>();
            var userManager = new Mock<IIdentityMembershipProvider>();

            var controller = new UserController(clientRepository, userRepository, userManager.Object);
            ModelHelper.SetCurrentUserToRole("consultant", controller);

            var postModel = new UserViewModel();
            postModel.ClientId = client.Id;
            var result = controller.Create(postModel) as ViewResult;

            Assert.IsTrue(controller.ModelState.Count > 0);
        }

        [TestMethod]
        public void CanCreateUserAndAddToRepository()
        {
            var clientRepository = RepositoryHelper.RepositoryLocalWithOneClient;
            var client = clientRepository.All().First();

            var userRepositoryMock = new Mock<LocalRepository<InsideUser>> { CallBase = true };

            var userManager = new Mock<IIdentityMembershipProvider>();

            var controller = new UserController(clientRepository, userRepositoryMock.Object, userManager.Object);
            ModelHelper.SetCurrentUserToRole("consultant", controller);

            var userPostData = new UserViewModel();
            userPostData.Name = "user name";
            userPostData.Password = "Password1!";
            userPostData.ClientId = client.Id;
            userPostData.ClientName = client.Name;

            userManager.Setup(u => u.GetUsers()).Returns(userRepositoryMock.Object.All);
            userManager.Setup(u => u.Create(It.IsAny<InsideUser>(), "Password1!")).Returns(true).Verifiable();
            var result = controller.Create(userPostData) as ViewResult;

            userManager.Verify(um => um.Create(It.IsAny<InsideUser>(), "Password1!"), Times.Once);
            userManager.Verify(um => um.AddToRole(It.IsAny<string>(), "client"), Times.Once);
        }

        [TestMethod]
        public void CanNotCreateUserIfUserNameAlreadyExsists()
        {
            var clientRepository = RepositoryHelper.RepositoryLocalWithOneClient;
            var client = clientRepository.All().First();

            var userRepositoryMock = new Mock<LocalRepository<InsideUser>> { CallBase = true };

            var userManager = new Mock<IIdentityMembershipProvider>();

            var controller = new UserController(clientRepository, userRepositoryMock.Object, userManager.Object);
            ModelHelper.SetCurrentUserToRole("consultant", controller);

            var userPostData = new UserViewModel();

            userPostData.Name = "user name";
            userPostData.Password = "Password1!";
            userPostData.ClientId = client.Id;
            userPostData.ClientName = client.Name;

            userManager.Setup(u => u.Create(It.IsAny<InsideUser>(), "Password1!")).Returns(false).Verifiable();
            
            var result = controller.Create(userPostData) as ViewResult;

            Assert.IsTrue(controller.ModelState.Count > 0);
        }

        [TestMethod]
        public void EditForIdReturnViewWithModel()
        {
            var clientRepository = RepositoryHelper.RepositoryLocalWithOneClient;
            var client = clientRepository.All().First();

            var userRepository = new LocalRepository<InsideUser>();
            userRepository.Add(ModelHelper.TestUser1AllDataNoReferences);
            var user = userRepository.All().First();
            user.UserName = "User name";
            user.Client = client;
            var userManager = new Mock<IIdentityMembershipProvider>();

            userManager.Setup(u => u.FindById(user.Id)).Returns(user);
            var claim = new Claim("test", "AnyId");

            userManager.Setup(um => um.GetRoles(It.IsAny<string>())).Returns(new List<string> { "mockRole" });

            var controller = new UserController(clientRepository, userRepository, userManager.Object);
            ModelHelper.SetCurrentUserToRole("consultant", controller);
            ModelHelper.SetClaimAsCurrentUserForController(claim, controller);

            var result = controller.Edit(user.Id) as ViewResult;

            var resultmodel = result.Model as UserEditViewModel;

            Assert.AreEqual(user.ClientId, resultmodel.User.ClientId);
        }

        [TestMethod]
        public void EditPostSavesData()
        {
            var clientRepository = RepositoryHelper.RepositoryLocalWithOneClient;
            var client = clientRepository.All().First();

            var userRepository = new LocalRepository<InsideUser>();
            userRepository.Add(ModelHelper.TestUser1AllDataNoReferences);
            var user = userRepository.All().First();
            user.PasswordHash = "HashedOldPassword";
            var userManager = new Mock<IIdentityMembershipProvider>();

            userManager.Setup(u => u.FindById(user.Id)).Returns(user);
            userManager.Setup(u => u.UpdatePassword(user.Id, "Password1!")).Returns(true).Verifiable();
            var controller = new UserController(clientRepository, userRepository, userManager.Object);
            ModelHelper.SetCurrentUserToRole("consultant", controller);

            var userPostData = new UserViewModel();
            userPostData.Name = "new";
            userPostData.Password = "Password1!";
            userPostData.ClientId = client.Id;
            userPostData.Id = user.Id;
            userPostData.ClientName = client.Name;

            var result = controller.Edit(userPostData) as ViewResult;

            /*userManager.Verify(u => u.RemovePassword(user.Id), Times.Once);*/
            userManager.Verify(u => u.UpdatePassword(user.Id, "Password1!"), Times.Once);
        }


        [TestMethod]
        public void DeletPostForValidDataRemovesUserFromRepository()
        {
            var clientRepository = RepositoryHelper.RepositoryLocalWithOneClient;
            var client = clientRepository.All().First();

            var userRepository = new LocalRepository<InsideUser>();
            userRepository.Add(ModelHelper.TestUser1AllDataNoReferences);
            var user = userRepository.All().First();

            var userManager = new Mock<IIdentityMembershipProvider>();
            userManager.Setup(u => u.FindById(user.Id)).Returns(user);

            var controller = new UserController(clientRepository, userRepository, userManager.Object);
            ModelHelper.SetCurrentUserToRole("consultant", controller);

            var result = controller.DeleteClientUser(user.Id,client.Id) as RedirectResult;

            userManager.Verify(um => um.Delete(user), Times.Once);
        }
    }
}
