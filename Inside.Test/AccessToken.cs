using System;
using System.Linq;
using Helpers.test;
using Inside.AcceptToken;
using InsideModel.Models;
using InsideModel.repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Inside.Test
{
    [TestClass]
    public class AccessTokenTest
    {
        [TestMethod]
        public void CanCreate()
        {
            var tokenRepository = new Mock<IRepository<Token>>();
            var userRepository = new Mock<IRepository<InsideUser>>();
            var provider = new AccessTokenProvider(tokenRepository.Object, userRepository.Object);
        }

        [TestMethod]
        public void CreateNewTokenAndReturnForUser()
        {
            var tokenRepository = new Mock<IRepository<Token>>();
            var userRepository = new LocalRepository<InsideUser>();
            var provider = new AccessTokenProvider(tokenRepository.Object, userRepository);
            var userId = "NewGuidString";

            userRepository.Add(new InsideUser() {Id = userId });
            var result = provider.GetToken(userId);

            tokenRepository.Verify(tr => tr.Add(It.IsAny<Token>()), Times.Once);

            Assert.AreEqual(DateTime.Today.AddDays(30).Date, result.ExpirationDate.Date);
            Assert.AreEqual("NewGuidString", result.UserId);
        }

        [TestMethod]
        public void CreateDifferentTokenEachTime()
        {
            var tokenRepository = new Mock<IRepository<Token>>();
            var userRepository = new LocalRepository<InsideUser>();
            var provider = new AccessTokenProvider(tokenRepository.Object, userRepository);
            var userId = "NewGuidString";
            userRepository.Add(new InsideUser() { Id = userId });
            var result = provider.GetToken(userId);

            tokenRepository.Verify(tr => tr.Add(It.IsAny<Token>()), Times.Once);

            Assert.AreEqual(DateTime.Today.AddDays(30).Date, result.ExpirationDate.Date);
            Assert.AreEqual("NewGuidString", result.UserId);

            var result2 = provider.GetToken(userId);

            tokenRepository.Verify(tr => tr.Add(It.IsAny<Token>()), Times.Exactly(2));

            Assert.AreEqual(DateTime.Today.AddDays(30).Date, result2.ExpirationDate.Date);
            Assert.AreEqual("NewGuidString", result2.UserId);

            Assert.AreNotEqual(result.AccessToken, result2.AccessToken);
        }

        [TestMethod]
        public void RemoveExpiredTokens()
        {
            var tokenRepository = new LocalRepository<Token>();
            var userRepository = new Mock<IRepository<InsideUser>>();
            var provider = new AccessTokenProvider(tokenRepository, userRepository.Object);
            var expiredToken = new Token()
            {
                Id = 1,
                UserId = "NewGuidString",
                AccessToken = "Some Random Generated String 1",
                ExpirationDate = DateTime.Today.AddDays(-1)
            };
            var validToken = new Token()
            {
                Id = 2,
                UserId = "NewGuidString2",
                AccessToken = "Some Random Generated String 2",
                ExpirationDate = DateTime.Today.AddDays(1)
            };
            tokenRepository.Add(expiredToken);
            tokenRepository.Add(validToken);

            provider.RemoveExpired();

            Assert.AreEqual(1, tokenRepository.All().Count());
            Assert.AreEqual(validToken.ExpirationDate, tokenRepository.All().Single().ExpirationDate);

        }

        [TestMethod]
        public void ValidateExistingAndNotExistingToken()
        {
            var tokenRepository = new LocalRepository<Token>();
            var userRepository = new Mock<IRepository<InsideUser>>();
            var provider = new AccessTokenProvider(tokenRepository, userRepository.Object);
            var client = new Client()
            {
                Id = 1
            };
            var user = new InsideUser()
            {
                Id = "Id2",
                ClientId = client.Id,
                Client = client

            };

            var validToken = new Token()
            {
                Id = 2,
                UserId =  user.Id,
                AccessToken = "Some Random Generated String 2",
                ExpirationDate = DateTime.Today.AddDays(1),
                InsideUser = user
            };

            
            tokenRepository.Add(validToken);

            var validResult = provider.Validate((int) validToken.InsideUser.ClientId, validToken.AccessToken);
            var inValidResult = provider.Validate((int) validToken.InsideUser.ClientId, "Some Invalid Random String");
            Assert.IsTrue(validResult);
            Assert.IsFalse(inValidResult);
        }

        [TestMethod]
        public void ValidateUrlGenerator()
        {
            var tokenRepository = new Mock<IRepository<Token>>();
            var userRepository = new LocalRepository<InsideUser>();
            var provider = new AccessTokenProvider(tokenRepository.Object, userRepository);
            var userId = "NewGuidString";
            userRepository.Add(new InsideUser() { Id = userId });
            var urlResult = provider.GenerateAccessUrl(userId, "www.return.url/Action");

            tokenRepository.Verify(tr => tr.Add(It.IsAny<Token>()), Times.Once);
            tokenRepository.Verify(tr => tr.SaveChanges(), Times.Once);

            Assert.IsTrue(urlResult.Contains("&returnUrl=www.return.url%2fAction"));


        }
    }
}
