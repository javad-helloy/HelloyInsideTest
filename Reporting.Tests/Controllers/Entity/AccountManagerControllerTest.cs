using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Claims;
using System.Collections.Generic;
using System.Web.Mvc;
using Helpers.test;
using Inside.membership;
using InsideModel.Models;
using InsideModel.Models.Identity;
using InsideModel.repositories;
using InsideReporting.Controllers.Enity;
using InsideReporting.Models;
using InsideReporting.Models.AccountManager;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace InsideReporting.Tests.Controllers.Entity
{
  
    [TestClass]
    public class AccountManagerControllerTest
    {

        [TestMethod]
        public void CanCreate()
        {
            var adminRepository = new Mock<IRepository<InsideUser>>();
            var userManager = new Mock<IIdentityMembershipProvider>();
            var controller = new AccountManagerController(adminRepository.Object, userManager.Object);
        }

        [TestMethod]
        public void IndexReturnsZeroItems()
        {
            var adminRepository = new Mock<IRepository<InsideUser>>();
            var userManager = new Mock<IIdentityMembershipProvider>();
            var claim = new Claim("test", "AnyId");
            
            userManager.Setup(um => um.GetRoles(It.IsAny<string>())).Returns(new List<string> { "mockRole" });

            var controller = new AccountManagerController(adminRepository.Object, userManager.Object);
            ModelHelper.SetClaimAsCurrentUserForController(claim, controller);

            var result = controller.Index() as ViewResult;

            var model = result.Model as AccountManagerListViewModel;

            Assert.AreEqual(0, model.Collection.Count);
        }

        [TestMethod]
        public void IndexReturnsmultipleItems()
        {
            var adminRepository = new LocalRepository<InsideUser>();
            var userManager = new Mock<IIdentityMembershipProvider>();

            var claim = new Claim("test", "AnyId");
            
            userManager.Setup(um => um.GetRoles(It.IsAny<string>())).Returns(new List<string> { "mockRole" });


            var controller = new AccountManagerController(adminRepository, userManager.Object);
            ModelHelper.SetClaimAsCurrentUserForController(claim, controller);
            var adminInRepository1 = new InsideUser()
            {
                Role = new Collection<InsideRole>{ModelHelper.TestSalesRole},
                Id = "Id1",
                Name = "Test Admin 1"
            };

            var adminInRepository2 = new InsideUser()
            {
                Role = new Collection<InsideRole>{ModelHelper.TestSalesRole},
                Id = "Id2",
                Name = "Test Admin 2"
            };
            adminRepository.Add(adminInRepository1);
            adminRepository.Add(adminInRepository2);

            userManager.Setup(um => um.GetUsers()).Returns(adminRepository.All());
            var result = controller.Index() as ViewResult;

            var model = result.Model as AccountManagerListViewModel;

            Assert.AreEqual(2, model.Collection.Count);
            Assert.AreEqual("Test Admin 1", model.Collection.Single(a => a.Id == "Id1").Name);
            Assert.AreEqual("Test Admin 2", model.Collection.Single(a => a.Id == "Id2").Name);
        }

        [TestMethod]
        public void IndexShowsOnlyAccountManagers()
        {
            var adminRepository = new LocalRepository<InsideUser>();
            var userManager = new Mock<IIdentityMembershipProvider>();

            var claim = new Claim("test", "AnyId");
            
            userManager.Setup(um => um.GetRoles(It.IsAny<string>())).Returns(new List<string> { "mockRole" });

            var controller = new AccountManagerController(adminRepository, userManager.Object);
            ModelHelper.SetClaimAsCurrentUserForController(claim, controller);

            var adminInRepository1 = new InsideUser()
            {
                Role = new Collection<InsideRole>{ModelHelper.TestSalesRole},
                Id = "Id1",
                Name = "Test Admin 1"
            };

            var adminInRepository2 = new InsideUser()
            {
                Role = new Collection<InsideRole>{ModelHelper.TestConsultantRole},
                Id = "Id2",
                Name = "Test Admin 2"
            };
            adminRepository.Add(adminInRepository1);
            adminRepository.Add(adminInRepository2);
            userManager.Setup(um => um.GetUsers()).Returns(adminRepository.All());
            var result = controller.Index() as ViewResult;

            var model = result.Model as AccountManagerListViewModel;

            Assert.AreEqual(1, model.Collection.Count);
            Assert.AreEqual("Test Admin 1", model.Collection.Single().Name);
            
        }

        [TestMethod]
        public void CreateGetReturnsView()
        {
            var adminRepository = new Mock<IRepository<InsideUser>>();
            var userManager = new Mock<IIdentityMembershipProvider>();

            var claim = new Claim("test", "AnyId");
            
            userManager.Setup(um => um.GetRoles(It.IsAny<string>())).Returns(new List<string> { "mockRole" });

            var controller = new AccountManagerController(adminRepository.Object, userManager.Object);
            ModelHelper.SetClaimAsCurrentUserForController(claim, controller);

            var result = controller.Create() as ViewResult;

            Assert.IsTrue(controller.ModelState.Count == 0);
            
        }

        [TestMethod]
        public void CreatePostReturnsErrorFroEmptyPassword()
        {
            var adminRepository = new Mock<IRepository<InsideUser>>();
            var userManager = new Mock<IIdentityMembershipProvider>();
            var controller = new AccountManagerController(adminRepository.Object, userManager.Object);
            var userPostData = new AccountManagerViewModel();
            var result = controller.Create(userPostData) as ViewResult;

            Assert.IsTrue(controller.ModelState.Values.First().Errors.First().ErrorMessage == "Ange lösenord");
           

        }
        
        [TestMethod]
        public void CreatePostCanCreateNewAccountManager()
        {
            var adminRepository = new Mock<LocalRepository<InsideUser>>{ CallBase = true };
            var userManager = new Mock<IIdentityMembershipProvider>();

            var controller = new AccountManagerController(adminRepository.Object, userManager.Object);
            var accountManager1 = new AccountManagerViewModel()
            {
                Id = "Id1",
                Name = "Test AM",
                Email = "Test@test.com",
                Password = "Test"
            };


            userManager.Setup(u => u.GetUsers()).Returns(adminRepository.Object.All);
            userManager.Setup(u => u.Create(It.IsAny<InsideUser>(),"Test")).Returns(true).Verifiable();

            var result = controller.Create(accountManager1) as ViewResult;

            //Assert.IsTrue(adminRepository.Object.All().Any(u => u.Id == guid));
            userManager.Verify(um => um.Create(It.IsAny<InsideUser>(), "Test"), Times.Once);
            userManager.Verify(um => um.AddToRole(It.IsAny<string>(), "sales"), Times.Once);
           

        }

        [TestMethod]
        public void CreatePostReturnsToViewIfUserAlreadyExists()
        {
            var adminRepository = new Mock<LocalRepository<InsideUser>> { CallBase = true };
            var userManager = new Mock<IIdentityMembershipProvider>();

            var controller = new AccountManagerController(adminRepository.Object, userManager.Object);
            
            var accountManager1 = new AccountManagerViewModel()
            {
                Id = "Id1",
                Name = "Test AM",
                Email = "Test@test.com",
                Password = "Test"
            };
            

            userManager.Setup(u => u.GetUsers()).Returns(adminRepository.Object.All);
            userManager.Setup(u => u.Create(It.IsAny<InsideUser>(), "Test")).Returns(false).Verifiable();

            var result = controller.Create(accountManager1) as ViewResult;

            Assert.IsTrue(controller.ModelState.Count > 0);


        }

        [TestMethod]
        public void EditGetReturnsErrorIfIdIsNull()
        {
            var adminRepository = new LocalRepository<InsideUser>();
            var userManager = new Mock<IIdentityMembershipProvider>();
            adminRepository.Add(ModelHelper.TestAccountManager);
            var accountManagerInRepository = adminRepository.All().First();
            var controller = new AccountManagerController(adminRepository, userManager.Object);
            var result = controller.Edit((string)null) as HttpStatusCodeResult;

            Assert.AreEqual(400,result.StatusCode);


        }

        [TestMethod]
        public void EditGetReturnsErrorIfAdminNotInRepository()
        {
            var adminRepository = new LocalRepository<InsideUser>();
            var userManager = new Mock<IIdentityMembershipProvider>();
            
            var controller = new AccountManagerController(adminRepository, userManager.Object);

            var result = controller.Edit("Id1") as HttpNotFoundResult;

            Assert.AreEqual(404, result.StatusCode);


        }

        [TestMethod]
        public void EditGetReturnsView()
        {
            var adminRepository = new LocalRepository<InsideUser>();
            var userManager = new Mock<IIdentityMembershipProvider>();
            var accountManagerInRepository = ModelHelper.TestAccountManager;

            adminRepository.Add(accountManagerInRepository);


            userManager.Setup(u => u.FindById(accountManagerInRepository.Id)).Returns(accountManagerInRepository);

            var claim = new Claim("test", "AnyId");
            
            userManager.Setup(um => um.GetRoles(It.IsAny<string>())).Returns(new List<string> { "mockRole" });

            var controller = new AccountManagerController(adminRepository, userManager.Object);
            ModelHelper.SetClaimAsCurrentUserForController(claim, controller);
            var result = controller.Edit(accountManagerInRepository.Id) as ViewResult;

            Assert.IsTrue(controller.ModelState.Count == 0);
        }

        [TestMethod]
        public void EditPostSavesDataForOkData()
        {
            var adminRepository = new LocalRepository<InsideUser>();
            var userManager = new Mock<IIdentityMembershipProvider>();
            
            var accountManagerInRepository = ModelHelper.TestAccountManager;
            accountManagerInRepository.PasswordHash = "HashedOldPassword";
            adminRepository.Add(accountManagerInRepository);

            var controller = new AccountManagerController(adminRepository, userManager.Object);

            var accountManager1 = new AccountManagerViewModel()
            {
                Id = accountManagerInRepository.Id,
                Password = "Test Edited",
                Email = "EditTest@Test.com"
            };


            userManager.Setup(u => u.GetUsers()).Returns(adminRepository.All);
            userManager.Setup(u => u.FindById(accountManagerInRepository.Id)).Returns(accountManagerInRepository);
            userManager.Setup(u => u.UpdatePassword(accountManagerInRepository.Id, "Test Edited")).Returns(true).Verifiable();

            var result = controller.Edit(accountManager1) as ViewResult;

            /*userManager.Verify(u => u.UpdatePassword(accountManagerInRepository.Id), Times.Once);*/
            userManager.Verify(u => u.UpdatePassword(accountManagerInRepository.Id, "Test Edited"), Times.Once);
           
            Assert.AreEqual("EditTest@Test.com", adminRepository.All().First().Email);
            
        }

        [TestMethod]
        public void EditPostSavesDataForOkDataReturnsViewForException()
        {
            var adminRepository = new LocalRepository<InsideUser>();
            var userManager = new Mock<IIdentityMembershipProvider>();

            var accountManagerInRepository = ModelHelper.TestAccountManager;
            adminRepository.Add(accountManagerInRepository);

            var controller = new AccountManagerController(adminRepository, userManager.Object);

            var accountManager1 = new AccountManagerViewModel()
            {
                Id = accountManagerInRepository.Id,
                Password = "Test Edited",
                Email = "EditTest@Test.com"
            };

            userManager.Setup(u => u.GetUsers()).Returns(adminRepository.All);
            userManager.Setup(u => u.FindById(accountManagerInRepository.Id)).Returns(accountManagerInRepository);
            userManager.Setup(u => u.UpdatePassword(accountManagerInRepository.Id, "Test Edited")).Returns(false).Verifiable();

            var result = controller.Edit(accountManager1) as ViewResult;

            Assert.IsTrue(controller.ModelState.Count > 0);


        }

        [TestMethod]
        public void EditPostSavesDataForOkDataWithEmptyMembershipProviderId()
        {
            var adminRepository = new LocalRepository<InsideUser>();
            var userManager = new Mock<IIdentityMembershipProvider>();
            var accountManagerInRepository = ModelHelper.TestAccountManager;
            accountManagerInRepository.Id = null;
            accountManagerInRepository.PasswordHash = "HashedOldPassword";
            adminRepository.Add(accountManagerInRepository);

            var controller = new AccountManagerController(adminRepository, userManager.Object);
            
            var accountManager1 = new AccountManagerViewModel()
            {
                Name= accountManagerInRepository.Name,
                Id = accountManagerInRepository.Id,
                Password = "Test Edited",
                Email = "EditTest@Test.com"
            };


            userManager.Setup(u => u.GetUsers()).Returns(adminRepository.All);
            userManager.Setup(u => u.FindById(accountManagerInRepository.Id)).Returns(accountManagerInRepository);
            userManager.Setup(u => u.UpdatePassword(accountManagerInRepository.Id, "Test Edited")).Returns(true).Verifiable();
            userManager.Setup(u => u.Create(accountManagerInRepository, "Test Edited")).Returns(true).Verifiable();

            var result = controller.Edit(accountManager1) as ViewResult;

           /* userManager.Verify(u => u.RemovePassword(accountManagerInRepository.Id), Times.Once);*/
            userManager.Verify(u => u.UpdatePassword(accountManagerInRepository.Id, "Test Edited"), Times.Never);
            userManager.Verify(u => u.Create(It.IsAny<InsideUser>(), "Test Edited"), Times.Once);
            Assert.AreEqual("EditTest@Test.com", adminRepository.All().First().Email);

        }
        
        [TestMethod]
        public void EditPostEmptyMembershipProviderIdReturnsViewOnException()
        {
            var adminRepository = new LocalRepository<InsideUser>();
            var userManager = new Mock<IIdentityMembershipProvider>();
            var accountManagerInRepository = ModelHelper.TestAccountManager;
            accountManagerInRepository.Id = null;
            adminRepository.Add(accountManagerInRepository);

            var controller = new AccountManagerController(adminRepository, userManager.Object);

            var accountManager1 = new AccountManagerViewModel()
            {
                Name= accountManagerInRepository.Name,
                Id = accountManagerInRepository.Id,
                Password = "Test Edited",
                Email = "EditTest@Test.com"
            };
            userManager.Setup(u => u.GetUsers()).Returns(adminRepository.All);
            userManager.Setup(u => u.FindById(accountManagerInRepository.Id)).Returns(accountManagerInRepository);
            userManager.Setup(u => u.Create(It.IsAny<InsideUser>(), "Test Edited")).Returns(false).Verifiable();

            var result = controller.Edit(accountManager1) as ViewResult;

            Assert.IsTrue(controller.ModelState.Count > 0);

        }

        [TestMethod]
        public void DeleteForValidIdReturnsViewWithModel()
        {
            var adminRepository = new LocalRepository<InsideUser>();
            var userManager = new Mock<IIdentityMembershipProvider>();

            var accountManagerInRepository = ModelHelper.TestAccountManager;
            adminRepository.Add(accountManagerInRepository);

            userManager.Setup(u => u.FindById(accountManagerInRepository.Id)).Returns(accountManagerInRepository);

            var claim = new Claim("test", "AnyId");
            
            userManager.Setup(um => um.GetRoles(It.IsAny<string>())).Returns(new List<string> { "mockRole" });

            var controller = new AccountManagerController(adminRepository, userManager.Object);
            ModelHelper.SetClaimAsCurrentUserForController(claim, controller);
            var result = controller.Delete(accountManagerInRepository.Id) as ViewResult;

            var resultModel = result.Model as AccountManagerViewModel;

            Assert.AreEqual("Test AM", resultModel.Name);
        }

        [TestMethod]
        public void DeletePostForValidDataREmovesUserFromRepository()
        {
            var adminRepository = new LocalRepository<InsideUser>();
            var userManager = new Mock<IIdentityMembershipProvider>();

            var accountManagerInRepository = ModelHelper.TestAccountManager;
            adminRepository.Add(accountManagerInRepository);

            userManager.Setup(u => u.FindById(accountManagerInRepository.Id)).Returns(accountManagerInRepository);
            userManager.Setup(u => u.GetUsers()).Returns(adminRepository.All);

            var controller = new AccountManagerController(adminRepository, userManager.Object);

            var result = controller.DeleteConfirmed(accountManagerInRepository.Id) as RedirectResult;

            userManager.Verify(um=>um.Delete(It.IsAny<InsideUser>()),Times.Once);
        }
    }
}
