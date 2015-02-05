using System;
using Inside;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InsideReporting.Tests.Service.Json
{
    [TestClass]
    public class JsonNetConverterTest
    {
        [TestMethod]
        public void CanCreate()
        {
            var jsonCoverter = new JsonUtcConverter();
        }

        [TestMethod]
        public void CanSerilize()
        {
            var jsonCoverter = new JsonUtcConverter();

            var testClass = new SimpleTestClassForJsonNetConverterTest();

            testClass.Date = new DateTime(2013,1,2,3,4,5);
            testClass.String = "test string";

            var result = jsonCoverter.Serilize(testClass);

            Assert.AreEqual("{\"Date\":\"2013-01-02T03:04:05Z\",\"String\":\"test string\"}", result);
        }

        [TestMethod]
        public void CanDeSerilize()
        {
            var jsonCoverter = new JsonUtcConverter();

            var testJson = "{\"Date\":\"2013-01-02 03:04:05 GMT\",\"String\":\"test string\"}";
            var result = jsonCoverter.Deserilize<SimpleTestClassForJsonNetConverterTest>(testJson);

            Assert.AreEqual(new DateTime(2013,1,2,3,4,5), result.Date);
            Assert.AreEqual("test string", result.String);
        }
    }

    internal class SimpleTestClassForJsonNetConverterTest
    {
        public DateTime Date { get; set; }
        public string String { get; set; }
    }
}
