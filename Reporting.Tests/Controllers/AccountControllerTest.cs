using Inside.membership;
using InsideModel.Models;
using InsideModel.repositories;
using InsideReporting.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace InsideReporting.Tests.Controllers
{
    [TestClass]
    public class AccountControllerTest
    {
        [TestMethod]
        public void CanCreate()
        {
            var tokenRepository =new Mock<IRepository<Token>>();
            var userRepository = new Mock<IRepository<InsideUser>>();
            var userManager = new Mock<IIdentityMembershipProvider>();

            var controller = new AccountController(tokenRepository.Object, userRepository.Object,
                 userManager.Object);
        }
    }
}
