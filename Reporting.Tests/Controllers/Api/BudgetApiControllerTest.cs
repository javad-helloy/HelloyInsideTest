using System;
using System.Linq;
using System.Web.Http.Results;
using Helpers.test;
using Inside.membership;
using InsideModel.Models;
using InsideModel.repositories;
using InsideReporting.Controllers.Api;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace InsideReporting.Tests.Controllers.Api
{
    [TestClass]
    public class BudgetApiControllerTest
    {
       [TestMethod]
        public void CanCreate()
        {
            var budgetRepository = new Mock<IRepository<Budget>>();
            var userManager = new Mock<IIdentityMembershipProvider>();

            var controller = new BudgetController(budgetRepository.Object, userManager.Object);
        }

       [TestMethod]
       public void CanGetBudgetWithId()
       {
           var budgetRepository = new LocalRepository<Budget>();
           var userManager = new Mock<IIdentityMembershipProvider>();

           var budgetInRepository = new Budget()
           {
               ClientId = 1,
               Id=1,
               Value = 1000,
               Month = new DateTime(2013, 2, 2)
           };

           budgetRepository.Add(budgetInRepository);
           var controller = new BudgetController(budgetRepository, userManager.Object);

           var budget = controller.GetBudget(budgetInRepository.Id) as OkNegotiatedContentResult<Budget>;

           Assert.AreEqual(1,budget.Content.Id);
           Assert.AreEqual(1000, budget.Content.Value);          
       }

       [TestMethod]
       public void GetBudgetWithIdNoResult()
       {
           var budgetRepository = new Mock<IRepository<Budget>>();
           var userManager = new Mock<IIdentityMembershipProvider>();

           var controller = new BudgetController(budgetRepository.Object, userManager.Object);
           var someId = 1;
           var budget = controller.GetBudget(someId);

           Assert.IsInstanceOfType(budget, typeof(NotFoundResult));
       }

       [TestMethod]
       public void GetBudgetList()
       {
           var budgetRepository = new LocalRepository<Budget>();
           var userManager = new Mock<IIdentityMembershipProvider>();
           var clientId = 1;
          
           var startDate = new DateTime(2014, 1, 1);
           var endDate = new DateTime(2014, 1, 31);
           var controller = new BudgetController(budgetRepository, userManager.Object);
           budgetRepository.Add(new Budget() { ClientId = clientId, Month = startDate.AddDays(5), Value = 10});
           //budgetRepository.Setup(br=>br.Where(It.IsAny<Expression<Func<Budget, bool>>>())).Returns(
           //     matchingBudget.AsQueryable());
           var budget = controller.GetBudgets(clientId,startDate,endDate) as OkNegotiatedContentResult<IQueryable<Budget>>;

           Assert.IsTrue(budget.Content.Any(b=>b.ClientId==clientId && b.Value==10));
           Assert.AreEqual(budget.Content.Count(), 1);
           
       }
 
       [TestMethod]
       public void BadrequestOnBadDate()
       {
           var budgetRepository = new LocalRepository<Budget>();
           var userManager = new Mock<IIdentityMembershipProvider>();
           var controller = new BudgetController(budgetRepository,userManager.Object);

           DateTime startDate = new DateTime(2013, 1, 1);
           DateTime endDate = startDate.AddMonths(-1);

           var budget = controller.GetBudgets(1, startDate, endDate);
           Assert.IsInstanceOfType(budget, typeof(BadRequestErrorMessageResult));
       }
    }
}
