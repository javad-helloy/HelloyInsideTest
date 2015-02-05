using System;
using Inside.ContactProductService;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Inside.Test
{
    [TestClass]
    public class ProductServiceTest
    {
        [TestMethod]
        public void CanCreate()
        {
            var service = new ProductService();
        }

        [TestMethod]
        public void CanComputeSearchProductCorrectlly()
        {
            var service = new ProductService();

            Assert.IsFalse(service.IsSearch("", ""));
            Assert.IsTrue(service.IsSearch("google", "cpc"));
            Assert.IsFalse(service.IsSearch("Retargeting", "Cpc"));
            Assert.IsFalse(service.IsSearch("display in campaign", "cpc"));
            Assert.IsFalse(service.IsSearch("yahoo", "organic"));
        }

        [TestMethod]
        public void CanComputeOrganicProductCorrectlly()
        {
            var service = new ProductService();

            Assert.IsFalse(service.IsOrganic( ""));
            Assert.IsTrue(service.IsOrganic( "organic"));
            Assert.IsTrue(service.IsOrganic("Organic"));
            Assert.IsTrue(service.IsOrganic("organic"));
            Assert.IsFalse(service.IsOrganic("cpc"));
        }

        [TestMethod]
        public void CanComputeRetargettingProductCorrectlly()
        {
            var service = new ProductService();

            Assert.IsFalse(service.IsRetargeting("", ""));
            Assert.IsFalse(service.IsRetargeting("yahoo", "cpc"));
            Assert.IsTrue(service.IsRetargeting("display in campaign with Remarketing", "cpc"));
            Assert.IsTrue(service.IsRetargeting("Remarketing", "retargeting"));
            Assert.IsTrue(service.IsRetargeting("Some campaign", "Remarketing"));
        }

        [TestMethod]
        public void CanComputeDisplayProductCorrectlly()
        {
            var service = new ProductService();

            Assert.IsFalse(service.IsDisplay("", ""));
            Assert.IsFalse(service.IsDisplay("yahoo", "cpc"));
            Assert.IsFalse(service.IsDisplay("display in campaign with Remarketing", "cpc"));
            Assert.IsTrue(service.IsDisplay("Some campaign", "Display"));
            Assert.IsTrue(service.IsDisplay("display in campaign", "cpc"));
        }

        [TestMethod]
        public void CanGetProductCorrectlly()
        {
            var service = new ProductService();

            Assert.AreEqual("Display", service.GetProduct("Some campaign", "Display"));
            Assert.AreEqual("Retargeting", service.GetProduct("display in campaign with Remarketing", "cpc"));
            Assert.AreEqual("Search", service.GetProduct("yahoo", "cpc"));
            Assert.AreEqual("Organic", service.GetProduct("no campaign", "organic"));
           
        }

        [TestMethod]
        public void CanValidateProductCorrectlly()
        {
            var service = new ProductService();

            Assert.IsTrue(service.IsValidProduct("Some campaign", "Display"));
            Assert.IsTrue(service.IsValidProduct("display in campaign with Remarketing", "cpc"));
            Assert.IsTrue(service.IsValidProduct("yahoo", "cpc"));
            Assert.IsTrue(service.IsValidProduct("no campaign", "organic"));

            Assert.IsFalse(service.IsValidProduct("campaing", "Medium"));
            Assert.IsFalse(service.IsValidProduct("yahoo", "Notcpc"));
            Assert.IsFalse(service.IsValidProduct("no campaign", "Notorganic"));

        }
    }
}
