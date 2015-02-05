using System.Web.Mvc;
using InsideReporting.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RestSharp;

namespace InsideReporting.Tests.Controllers
{
    [TestClass]
    public class GeoControllerTest
    {
        [TestMethod]
        public void CanCreate()
        {
            var restClient = new Mock<IRestClient>();
            var controller = new GeoController(restClient.Object);
        }

        [TestMethod]
        public void DelegatesToGoogleMaps()
        {
            var restClient = new Mock<IRestClient>();

            var restResponse = new Mock<IRestResponse>();
            var responseFromGoogle = "response from google";
            restResponse.Setup(r => r.Content).Returns(responseFromGoogle);

            restClient.Setup(m => m.Execute(It.IsAny<IRestRequest>())).Returns(restResponse.Object);

            var controller = new GeoController(restClient.Object);
            var controllerResponse = controller.Code("address") as ContentResult;

            Assert.AreEqual(responseFromGoogle, controllerResponse.Content, "expected same result as from google");
        }
    }
}
