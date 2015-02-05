using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web.Mvc;
using Helpers.test;
using Inside.membership;
using InsideModel.Models;
using InsideModel.repositories;
using InsideReporting.Controllers.Enity;
using InsideReporting.Models;
using InsideReporting.Models.Budget;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace InsideReporting.Tests.Controllers.Entity
{
    [TestClass]
    public class budgetControllerTest
    {
        [TestMethod]
        public void CanCreate()
        {
            IRepository<Client> clientRepository = new LocalRepository<Client>();
            IRepository<Budget> budgetRepository = new LocalRepository<Budget>();
            var userManager = new Mock<IIdentityMembershipProvider>();
            var controller = new BudgetController(clientRepository, budgetRepository, userManager.Object);
        }

        [TestMethod]
        public void GetsAListForFilteredOnLoggedInUser()
        {
            IRepository<Client> clientRepository = new LocalRepository<Client>();
            IRepository<Budget> budgetRepository = new LocalRepository<Budget>();

            var userManager = new Mock<IIdentityMembershipProvider>();

            var consultantGuid = Guid.NewGuid();

            Client client1 = new Client();
            client1.IsActive = true;
            client1.Consultant = new InsideUser() { Id = consultantGuid.ToString() };
            client1.Budgets.Add( new Budget(){Value = 1000, Month = new DateTime(2014,1,1)});
            client1.Budgets.Add(new Budget() { Value = 2000, Month = new DateTime(2014, 2, 1) });

            Client client2 = new Client();
            client2.IsActive = true;
            client2.Consultant = new InsideUser() { Id = consultantGuid.ToString() };
            
            Client client3 = new Client();
            client3.IsActive = true;
            client3.Consultant = new InsideUser() { Id = Guid.NewGuid().ToString() };

            clientRepository.Add(client1);
            clientRepository.Add(client2);
            clientRepository.Add(client3);

            var claim = new Claim("test", consultantGuid.ToString());
            
            userManager.Setup(um => um.GetRoles(consultantGuid.ToString())).Returns(new List<string> { "mockRole" });
            
            var controller = new BudgetController(clientRepository, budgetRepository, userManager.Object);
            ModelHelper.SetClaimAsCurrentUserForController(claim, controller);
            var result = controller.Index("") as ViewResult;
            var resultModel = result.Model as ClientsWithBudgetsViewModel;
            Assert.AreEqual(2, resultModel.Clients.Count);

            var result2 = controller.Index("", showAll:true) as ViewResult;
            var result2Model = result2.Model as ClientsWithBudgetsViewModel;
            Assert.AreEqual(3, result2Model.Clients.Count);
        }

        [TestMethod]
        public void CanGetAListOfBudgetsForClientOrderedByHandleStatusIgnoreInactiveClients()
        {
            IRepository<Client> clientRepository = new LocalRepository<Client>();
            IRepository<Budget> budgetRepository = new LocalRepository<Budget>();

            var userManager = new Mock<IIdentityMembershipProvider>();
            var budget1 = new Budget() { Value = 1000, Month = new DateTime(2014, 1, 1), ClientId = 1, IsHandled = false };
            var budget2 = new Budget() { Value = 2000, Month = new DateTime(2014, 2, 1), ClientId = 1 };
            var budget3 = new Budget() { Value = 4000, Month = new DateTime(2014, 3, 1), ClientId = 2, IsHandled = false };
            var budget4 = new Budget() { Value = 4000, Month = new DateTime(2014, 3, 1), ClientId = 3};

            budgetRepository.Add(budget1);
            budgetRepository.Add(budget2);
            budgetRepository.Add(budget3);
            budgetRepository.Add(budget4);

            Client client1 = new Client();
            client1.Id = 1;
            client1.IsActive = true;
            client1.FeeFixedMonthly = 250;
            client1.FeePercent = 0.1m;
            
            client1.Budgets.Add(budget1);
            
            client1.Budgets.Add(budget2);
            budget1.Client = client1;
            budget2.Client = client1;

            Client client2 = new Client();
            client2.IsActive = true;
            client2.Id = 2;
            client2.Budgets.Add(budget3);

            Client client3 = new Client();
            client2.IsActive = false;
            client2.Id = 3;
            client2.Budgets.Add(budget4);

            clientRepository.Add(client1);
            clientRepository.Add(client2);
            clientRepository.Add(client3);

            var claim = new Claim("test", "AnyId");
            
            userManager.Setup(um => um.GetRoles(It.IsAny<string>())).Returns(new List<string> { "mockRole" });

            var controller = new BudgetController(clientRepository, budgetRepository, userManager.Object);
            ModelHelper.SetClaimAsCurrentUserForController(claim, controller);

            var result = controller.List(1, "Potential error message") as ViewResult;
            var resultModel = result.Model as ClientBudgetListViewModel;
            
            Assert.AreEqual(2, resultModel.Budgets.Count());
            Assert.AreEqual(2000, resultModel.Budgets[1].Value);
            Assert.AreEqual(1800, resultModel.Budgets[1].AdSpent);

            Assert.AreEqual(1000, resultModel.Budgets[0].Value);
            Assert.AreEqual(900, resultModel.Budgets[0].AdSpent);

            Assert.AreEqual(new DateTime(2014, 2, 1), resultModel.Budgets[1].Month);
            Assert.AreEqual(new DateTime(2014, 1, 1), resultModel.Budgets[0].Month);

            Assert.AreEqual(1, resultModel.ClientId);
            Assert.AreEqual("Potential error message", resultModel.Error);


        }

        [TestMethod]
        public void CanCalculateAdSpendValueForAllBudgetsForOneClient()
        {
            IRepository<Client> clientRepository = new LocalRepository<Client>();
            IRepository<Budget> budgetRepository = new LocalRepository<Budget>();

            var userManager = new Mock<IIdentityMembershipProvider>();
            var budget1 = new Budget() { Value = 1000, Month = new DateTime(2014, 1, 1), ClientId = 1, IsHandled = false };
            var budget2 = new Budget() { Value = 2000, Month = new DateTime(2014, 2, 1), ClientId = 1 };
            var budget3 = new Budget() { Value = 4000, Month = new DateTime(2014, 3, 1), ClientId = 2, IsHandled = false };
            var budget4 = new Budget() { Value = 6543, Month = new DateTime(2014, 4, 1), ClientId = 2, IsHandled = false };
            budgetRepository.Add(budget1);
            budgetRepository.Add(budget2);
            budgetRepository.Add(budget3);
            budgetRepository.Add(budget4);

            Client client1 = new Client();
            client1.Id = 1;
            client1.IsActive = true;
            client1.FeeFixedMonthly = 250;
            client1.FeePercent = 0.1m;

            client1.Budgets.Add(budget1);

            client1.Budgets.Add(budget2);
            budget1.Client = client1;
            budget2.Client = client1;

            Client client2 = new Client();
            client2.IsActive = true;
            client2.Id = 2;
            client2.FeeFixedMonthly = 500;
            client2.FeePercent = 0.15m;

            client2.Budgets.Add(budget3);
            budget3.Client = client2;
            client2.Budgets.Add(budget4);
            budget4.Client = client2;

            clientRepository.Add(client1);
            clientRepository.Add(client2);

            var claim = new Claim("test", "AnyId");

            userManager.Setup(um => um.GetRoles(It.IsAny<string>())).Returns(new List<string> { "mockRole" });

            var controller = new BudgetController(clientRepository, budgetRepository, userManager.Object);
            ModelHelper.SetClaimAsCurrentUserForController(claim, controller);

            var result = controller.List(1, "") as ViewResult;
            var resultModel = result.Model as ClientBudgetListViewModel;

            Assert.AreEqual(2, resultModel.Budgets.Count());
            Assert.AreEqual(1, resultModel.Budgets[1].ClientId);
            Assert.AreEqual(2000.0m, resultModel.Budgets[1].Value);
            Assert.AreEqual(1800.0m, resultModel.Budgets[1].AdSpent);

            Assert.AreEqual(1, resultModel.Budgets[0].ClientId);
            Assert.AreEqual(1000.0m, resultModel.Budgets[0].Value);
            Assert.AreEqual(900.0m, resultModel.Budgets[0].AdSpent);
        }


        [TestMethod]
        public void CanCalculateAdSpendValueForAllBudgetsForAllClientsToShow()
        {
            IRepository<Client> clientRepository = new LocalRepository<Client>();
            IRepository<Budget> budgetRepository = new LocalRepository<Budget>();

            var userManager = new Mock<IIdentityMembershipProvider>();
            var budget1 = new Budget() { Value = 1000, Month = new DateTime(2014, 1, 1), ClientId = 1, IsHandled = false };
            var budget2 = new Budget() { Value = 2000, Month = new DateTime(2014, 2, 1), ClientId = 1 };
            var budget3 = new Budget() { Value = 4000, Month = new DateTime(2014, 3, 1), ClientId = 2, IsHandled = false };
            var budget4 = new Budget() { Value = 6543, Month = new DateTime(2014, 4, 1), ClientId = 2, IsHandled = false };
            budgetRepository.Add(budget1);
            budgetRepository.Add(budget2);
            budgetRepository.Add(budget3);
            budgetRepository.Add(budget4);

            Client client1 = new Client();
            client1.Id = 1;
            client1.IsActive = true;
            client1.FeeFixedMonthly = 250;
            client1.FeePercent = 0.1m;

            client1.Budgets.Add(budget1);

            client1.Budgets.Add(budget2);
            budget1.Client = client1;
            budget2.Client = client1;

            Client client2 = new Client();
            client2.IsActive = true;
            client2.Id = 2;
            client2.FeeFixedMonthly = 500;
            client2.FeePercent = 0.15m;

            client2.Budgets.Add(budget3);
            budget3.Client = client2;
            client2.Budgets.Add(budget4);
            budget4.Client = client2;

            clientRepository.Add(client1);
            clientRepository.Add(client2);

            var claim = new Claim("test", "AnyId");

            userManager.Setup(um => um.GetRoles(It.IsAny<string>())).Returns(new List<string> { "mockRole" });

            var controller = new BudgetController(clientRepository, budgetRepository, userManager.Object);
            ModelHelper.SetClaimAsCurrentUserForController(claim, controller);

            var result2 = controller.ListAll(null, true) as ViewResult;
            var resultModel2 = result2.Model as BudgetListViewModel;

            Assert.AreEqual(4, resultModel2.Budgets.Count());

            Assert.AreEqual(4000.0m, resultModel2.Budgets[2].Value);
            Assert.AreEqual(3400.0m, resultModel2.Budgets[2].AdSpent);

            Assert.AreEqual(6543m, resultModel2.Budgets[3].Value);
            Assert.AreEqual(5561.55m, resultModel2.Budgets[3].AdSpent);


        }

        [TestMethod]
        public void CanDeleteABudget()
        {
            IRepository<Client> clientRepository = new LocalRepository<Client>();
            IRepository<Budget> budgetRepository = new LocalRepository<Budget>();
            var userManager = new Mock<IIdentityMembershipProvider>();

            Budget budget1 = new Budget();
            budget1.Id = 1;
            budget1.Value = 1000;
            budget1.Month = new DateTime(2014, 1, 1);
            budget1.ClientId = 1;

            Budget budget2 = new Budget();
            budget2.Id = 2;
            budget2.Value = 2000;
            budget2.Month = new DateTime(2014, 2, 1);
            budget2.ClientId = 2;

            budgetRepository.Add(budget1);
            budgetRepository.Add(budget2);

            Assert.AreEqual(2, budgetRepository.All().Count());
            
            var controller = new BudgetController(clientRepository, budgetRepository, userManager.Object);
            controller.Delete(1);
            
            Assert.AreEqual(1, budgetRepository.All().Count());
        }

        [TestMethod]
        public void CanCreateABudgetAndDoesNotCreateOverlapping()
        {
            IRepository<Client> clientRepository = new LocalRepository<Client>();
            IRepository<Budget> budgetRepository = new LocalRepository<Budget>();
            var userManager = new Mock<IIdentityMembershipProvider>();

            Budget budget1 = new Budget();
            budget1.Id = 1;
            budget1.Value = 1000;
            budget1.Month = new DateTime(2014, 1, 1);
            budget1.ClientId = 1;

            Budget budget2 = new Budget();
            budget2.Id = 2;
            budget2.Value = 2000;
            budget2.Month = new DateTime(2014, 2, 1);
            budget2.ClientId = 2;

            budgetRepository.Add(budget1);
            budgetRepository.Add(budget2);

            Assert.AreEqual(2, budgetRepository.All().Count());

            var controller = new BudgetController(clientRepository, budgetRepository, userManager.Object);
            
            EditBudgetViewModel createBudgetPostMessage = new EditBudgetViewModel();
            createBudgetPostMessage.clientId = 1;
            createBudgetPostMessage.month = "2014-03";
            createBudgetPostMessage.value = 3000;

            controller.Create(createBudgetPostMessage);
            Assert.AreEqual(3, budgetRepository.All().Count());

            controller.Create(createBudgetPostMessage);
            Assert.AreEqual(3, budgetRepository.All().Count());
        }

        [TestMethod]
        public void EditGetReturnsCorrectData()
        {
            IRepository<Client> clientRepository = new LocalRepository<Client>();
            IRepository<Budget> budgetRepository = new LocalRepository<Budget>();
            var userManager = new Mock<IIdentityMembershipProvider>();

            Budget budget1 = new Budget();
            budget1.Id = 1;
            budget1.Value = 1000;
            budget1.Month = new DateTime(2014, 1, 1);
            budget1.ClientId = 1;
            budget1.Comment = "Comment";
            budget1.Client = new Client {Name = "Client Name"};

            budgetRepository.Add(budget1);

            Assert.AreEqual(1, budgetRepository.All().Count());

            userManager.Setup(um => um.GetRoles(It.IsAny<string>())).Returns(new List<string> { "mockRole" });
            var controller = new BudgetController(clientRepository, budgetRepository, userManager.Object);
            var claim = new Claim("test", "AnyId");
            ModelHelper.SetClaimAsCurrentUserForController(claim, controller);

            var expectedEditBudgetInRepository = new CreateEditClientBudgetViewModel
            {
                BudgetId = 1,
                ClientId = 1,
                Month = "2014-01",
                Value = 1000,
                Comment = "Comment",
                ClientName = "Client Name"
            };

            var result = controller.Edit(1) as ViewResult;
            var resultModel = result.Model as CreateEditClientBudgetViewModel;

            Assert.AreEqual(expectedEditBudgetInRepository.BudgetId,resultModel.BudgetId);
            Assert.AreEqual(expectedEditBudgetInRepository.ClientId, resultModel.ClientId);
            Assert.AreEqual(expectedEditBudgetInRepository.Month, resultModel.Month);
            Assert.AreEqual(expectedEditBudgetInRepository.Value, resultModel.Value);
            Assert.AreEqual(expectedEditBudgetInRepository.Comment, resultModel.Comment);
            Assert.AreEqual(expectedEditBudgetInRepository.ClientName, resultModel.ClientName);


        }

        [TestMethod]
        public void CanHandleNewBudgets()
        {
            IRepository<Client> clientRepository = new LocalRepository<Client>();
            IRepository<Budget> budgetRepository = new LocalRepository<Budget>();
            var userManager = new Mock<IIdentityMembershipProvider>();

            Budget budget1 = new Budget
            {
                Id = 1, 
                ClientId = 1, 
                IsHandled = false
            };

            Budget budget2 = new Budget
            {
                Id = 2,
                ClientId = 1,
                IsHandled = false
            };

            budgetRepository.Add(budget1);
            budgetRepository.Add(budget2);

            Assert.AreEqual(2, budgetRepository.All().Count());

            var controller = new BudgetController(clientRepository, budgetRepository, userManager.Object);
           
            var result = controller.Handle(1);
            
            Assert.AreEqual(true, budgetRepository.Where(b=>b.Id==budget1.Id).Single().IsHandled);
            Assert.AreEqual(false, budgetRepository.Where(b => b.Id == budget2.Id).Single().IsHandled);
        }

        [TestMethod]
        public void CanEditValueAndCommentOfABudget()
        {
            IRepository<Client> clientRepository = new LocalRepository<Client>();
            IRepository<Budget> budgetRepository = new LocalRepository<Budget>();
            var userManager = new Mock<IIdentityMembershipProvider>();

            Budget budget1 = new Budget();
            budget1.Id = 1;
            budget1.Value = 1000;
            budget1.Month = new DateTime(2014, 1, 1);
            budget1.ClientId = 1;

            budgetRepository.Add(budget1);

            Assert.AreEqual(1, budgetRepository.All().Count());
            var controller = new BudgetController(clientRepository, budgetRepository, userManager.Object);

            var createBudgetPostMessage = new EditBudgetViewModel
            {
                id = 1,
                clientId = 1,
                month = "2014-01-01",
                value = 3000,
                comment = "New Comment"
            };

            controller.Edit(createBudgetPostMessage);
            Assert.AreEqual(1, budgetRepository.All().Count());
            Assert.AreEqual(3000, budgetRepository.All().First().Value);
            Assert.AreEqual("New Comment", budgetRepository.All().First().Comment);
        }
    }
}
