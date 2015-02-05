using System;
using System.Linq;
using Helpers.test;
using Inside.GoogleService;
using Inside.Time;
using InsideModel.Models;
using InsideModel.repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RestSharp;

namespace Inside.Test.GoogleService
{
    [TestClass]
    public class GoogleAuthenticationApiTest
    {
        [TestMethod]
        public void CanConstruct()
        {
            var restClient = new Mock<IRestClient>();
            var externalTokenRepository = new Mock<IRepository<ExternalToken>>() ;
            var serverTime = new Mock<IServerTime>() ;
            var jsonConverter = new Mock<IJsonConverter>();

            var api = new GoogleAuthentication(restClient.Object, 
                                                externalTokenRepository.Object, 
                                                serverTime.Object,
                                                jsonConverter.Object);
        }

        [TestMethod]
        public void CanGetAuthenticationUrl()
        {
            var restClient = new Mock<IRestClient>();
            var externalTokenRepository = new Mock<IRepository<ExternalToken>>();
            var serverTime = new Mock<IServerTime>();
            var jsonConverter = new Mock<IJsonConverter>();

            var api = new GoogleAuthentication(restClient.Object,
                                                externalTokenRepository.Object,
                                                serverTime.Object,
                                                jsonConverter.Object);

            var url = api.GetAuthorizationCodeUrl("redirectUrl");

            Assert.AreEqual("https://accounts.google.com/o/oauth2/auth?scope=https%3A%2F%2Fwww.googleapis.com%2Fauth%2Fanalytics.readonly&redirect_uri=redirectUrl&response_type=code&client_id=&access_type=offline", url);
        }

        [TestMethod]
        public void CanGetAccessTokenValidInDb()
        {
            var restClient = new Mock<IRestClient>();
            var externalTokenRepository = new LocalRepository<ExternalToken>();
            var serverTime = new Mock<IServerTime>();
            var jsonConverter = new Mock<IJsonConverter>();

            var api = new GoogleAuthentication(restClient.Object,
                                                externalTokenRepository,
                                                serverTime.Object,
                                                jsonConverter.Object);

            var now = new DateTime(2014, 12, 17, 10, 0,0);

            serverTime.Setup(st => st.RequestStarted).Returns(now);
            var validAccessToken = new ExternalToken
            {
                AccessToken = "AccessToken",
                ExpirationDate = now.AddHours(1),
                Type = "Google_Access_Token"
            };
            externalTokenRepository.Add(validAccessToken);
            var token = api.GetAccessToken();

            Assert.AreEqual("AccessToken", token.AccessToken);
            Assert.AreEqual("Google_Access_Token", token.Type);
            Assert.AreEqual(11, token.ExpirationDate.Hour);
           
        }

        [TestMethod]
        public void CanGetNewAccessTokenIfNotValidInDbOrForcedForNew()
        {
            var restClient = new Mock<IRestClient>();
            var externalTokenRepository = new LocalRepository<ExternalToken>();
            var serverTime = new Mock<IServerTime>();
            var jsonConverter = new Mock<IJsonConverter>();

            var api = new GoogleAuthentication(restClient.Object,
                                                externalTokenRepository,
                                                serverTime.Object,
                                                jsonConverter.Object);

            var now = new DateTime(2014, 12, 17, 10, 0, 0);

            serverTime.Setup(st => st.RequestStarted).Returns(now);
            var validRefreshToken = new ExternalToken
            {
                AccessToken = "RefreshToken",
                ExpirationDate = now.AddYears(100),
                Type = "Google_Refresh_Token"
            };

            var inValidAccessToken = new ExternalToken
            {
                AccessToken = "AccessToken",
                ExpirationDate = now.AddHours(-2),
                Type = "Google_Access_Token"
            };
            externalTokenRepository.Add(validRefreshToken);
            externalTokenRepository.Add(inValidAccessToken);

            var restResponse = new RestResponse();
            restResponse.Content = "{ \"access_token\" : \"NewAccessToken\", \"token_type\" : \"Bearer\", \"expires_in\" : 3600}";
            restClient.Setup(rc => rc.Execute(It.IsAny<RestRequest>()))
                .Returns(restResponse);

            var googleApiTokenResponse = new GoogleApiTokenResponse
            {
                access_token = "NewAccessToken",
                expires_in= 3600,
                token_type = "Bearer"
            };
            jsonConverter.Setup(
                jc =>
                    jc.Deserilize<GoogleApiTokenResponse>(
                        "{ \"access_token\" : \"NewAccessToken\", \"token_type\" : \"Bearer\", \"expires_in\" : 3600}"))
                .Returns(googleApiTokenResponse);

            var hasValidTokenInDb = externalTokenRepository.Any(t => t.Type == "Google_Access_Token" && t.ExpirationDate > now);
            Assert.AreEqual(false, hasValidTokenInDb);

            var token = api.GetAccessToken();

            var gotNewValidToken = externalTokenRepository.Any(t => t.Type == "Google_Access_Token" && t.ExpirationDate > now);
            Assert.AreEqual(true, gotNewValidToken);

            Assert.AreEqual("NewAccessToken", token.AccessToken);
            Assert.AreEqual("Google_Access_Token", token.Type);
            Assert.AreEqual(11, token.ExpirationDate.Hour);

        }

        [TestMethod]
        public void GetExistingValidAccessTokenIfForcedForNew()
        {
            var restClient = new Mock<IRestClient>();
            var externalTokenRepository = new LocalRepository<ExternalToken>();
            var serverTime = new Mock<IServerTime>();
            var jsonConverter = new Mock<IJsonConverter>();

            var api = new GoogleAuthentication(restClient.Object,
                                                externalTokenRepository,
                                                serverTime.Object,
                                                jsonConverter.Object);

            var now = new DateTime(2014, 12, 17, 10, 0, 0);

            serverTime.Setup(st => st.RequestStarted).Returns(now);
            var validRefreshToken = new ExternalToken
            {
                AccessToken = "RefreshToken",
                ExpirationDate = now.AddYears(100),
                Type = "Google_Refresh_Token"
            };

            var validAccessToken = new ExternalToken
            {
                AccessToken = "AccessToken",
                ExpirationDate = now.AddHours(1),
                Type = "Google_Access_Token"
            };
            externalTokenRepository.Add(validRefreshToken);
            externalTokenRepository.Add(validAccessToken);

            var restResponse = new RestResponse();
            restResponse.Content = "{ \"access_token\" : \"AccessToken\", \"token_type\" : \"Bearer\", \"expires_in\" : 3600}";
            restClient.Setup(rc => rc.Execute(It.IsAny<RestRequest>()))
                .Returns(restResponse);

            var googleApiTokenResponse = new GoogleApiTokenResponse
            {
                access_token = "AccessToken",
                expires_in = 3600,
                token_type = "Bearer"
            };
            jsonConverter.Setup(
                jc =>
                    jc.Deserilize<GoogleApiTokenResponse>(
                        "{ \"access_token\" : \"AccessToken\", \"token_type\" : \"Bearer\", \"expires_in\" : 3600}"))
                .Returns(googleApiTokenResponse);

            var hasValidTokenInDb = externalTokenRepository.Any(t => t.Type == "Google_Access_Token" && t.ExpirationDate > now);
            Assert.AreEqual(true, hasValidTokenInDb);

            var token = api.GetAccessToken();

            Assert.AreEqual(2, externalTokenRepository.All().Count());
            Assert.AreEqual(1, externalTokenRepository.Where(t => t.Type == "Google_Access_Token").Count());
            Assert.AreEqual("AccessToken", token.AccessToken);
            Assert.AreEqual("Google_Access_Token", token.Type);
            Assert.AreEqual(11, token.ExpirationDate.Hour);

        }

        [TestMethod]
        public void CanGetRefreshToken()
        {
            var restClient = new Mock<IRestClient>();
            var externalTokenRepository = new LocalRepository<ExternalToken>();
            var serverTime = new Mock<IServerTime>();
            var jsonConverter = new Mock<IJsonConverter>();

            var api = new GoogleAuthentication(restClient.Object,
                                                externalTokenRepository,
                                                serverTime.Object,
                                                jsonConverter.Object);

            var now = new DateTime(2014, 12, 17, 10, 0, 0);

            serverTime.Setup(st => st.RequestStarted).Returns(now);
           
            var restResponse = new RestResponse();
            restResponse.Content = "{\"access_token\" : \"AccessToken\", \"token_type\" : \"Bearer\", \"expires_in\" : 3600, \"refresh_token\" : \"RefreshToken\"}";
            restClient.Setup(rc => rc.Execute(It.IsAny<RestRequest>()))
                .Returns(restResponse);

            var googleApiTokenResponse = new GoogleApiTokenResponse
            {
                access_token = "AccessToken",
                expires_in = 3600,
                token_type = "Bearer",
                refresh_token = "RefreshToken",
            };
            jsonConverter.Setup(
                jc =>
                    jc.Deserilize<GoogleApiTokenResponse>(
                        "{\"access_token\" : \"AccessToken\", \"token_type\" : \"Bearer\", \"expires_in\" : 3600, \"refresh_token\" : \"RefreshToken\"}"))
                .Returns(googleApiTokenResponse);

            var token = api.GetRefreshToken("code", "redirectUrl");

            Assert.AreEqual(2, externalTokenRepository.All().Count());

            Assert.AreEqual("RefreshToken", token.AccessToken);
            Assert.AreEqual("Google_Refresh_Token", token.Type);
            Assert.AreEqual(2114, token.ExpirationDate.Year);

            var hasValidTokenInDb = externalTokenRepository.Any(t => t.Type == "Google_Access_Token" && t.ExpirationDate > now);
            Assert.AreEqual(true, hasValidTokenInDb);
            Assert.AreEqual("AccessToken", externalTokenRepository.Single(t => t.Type == "Google_Access_Token" && t.ExpirationDate > now).AccessToken);
            Assert.AreEqual("Google_Access_Token", externalTokenRepository.Single(t => t.Type == "Google_Access_Token" && t.ExpirationDate > now).Type);
            Assert.AreEqual(11, externalTokenRepository.Single(t => t.Type == "Google_Access_Token" && t.ExpirationDate > now).ExpirationDate.Hour);

        }
    }
}
