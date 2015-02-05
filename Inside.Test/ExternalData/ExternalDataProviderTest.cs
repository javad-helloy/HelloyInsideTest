using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Routing;
using Helpers.test;
using Inside.ContactService;
using Inside.ExternalData;
using Inside.Time;
using InsideModel.Models;
using InsideModel.repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using RestSharp;

namespace Inside.Test.ExternalData
{
    [TestClass]
    public class ExternalDataProviderTest
    {
        [TestMethod]
        public void CanConstruct()
        {
            var restClient = new Mock<IRestClient>();
            var clientRepository = new Mock<IRepository<Client>>();
            var serverTime = new Mock<IServerTime>();

            var xDataProvider = new ExternalDataProvider(restClient.Object, 
                                                         clientRepository.Object, 
                                                         serverTime.Object);
        }

        [TestMethod]
        public void CanGetPhoneData()
        {
            var restClient = new Mock<IRestClient>();
            var clientRepository = new Mock<IRepository<Client>>();
            var serverTime = new Mock<IServerTime>();

            var xDataProvider = new ExternalDataProvider(restClient.Object,
                                                         clientRepository.Object,
                                                         serverTime.Object);

            var restResponse = new Mock<IRestResponse>();
            restResponse.Setup(rr => rr.StatusCode).Returns(HttpStatusCode.OK);
            restResponse.Setup(rr => rr.Content).Returns("Some Content");
            restClient.Setup(rc =>rc.Execute(It.Is<RestRequest>(r =>
                                r.Resource ==
                                "api/v1/accounts/1/calls.json?start_date=2014-01-01&end_date=2014-02-01")))
                            .Returns(restResponse.Object);
            var startDate = new DateTime(2014, 01, 01);
            var endDate = new DateTime(2014, 02, 01);
            var result = xDataProvider.GetPhoneData(1, startDate, endDate);

            Assert.AreEqual("Some Content", result);
        }

        [TestMethod]
        public void CallTrackingMetricDataCreatesNewContactWithAllValues()
        {

            var restClient = new Mock<IRestClient>();
            var clientRepository = new LocalRepository<Client>();
            var serverTime = new Mock<IServerTime>();

            serverTime.Setup(st => st.RequestStarted).Returns(new DateTime(2013, 01, 01, 8, 20, 0));
            serverTime.Setup(s => s.ParseToServerTimeZoneFromStandardUser("2013-01-01 10:17 AM", "yyyy-MM-dd hh:mm tt")).Returns(new DateTime(2013, 01, 01, 8, 17, 0));

            var client = ModelHelper.TestClient1AllDataNoReferences;
            client.CallTrackingMetricId = 9724;
            clientRepository.Add(client);



            var xDataProvider = new ExternalDataProvider(restClient.Object,
                                                         clientRepository,
                                                         serverTime.Object);

            string sampleJsonData =
               "{\"id\":3158334,\"account_id\":9724,\"name\":\"Restricted\",\"search\":null,\"referrer\":null,\"location\":null,\"source\":\"Website\",\"source_id\":14932,\"tgid\":28622,\"likelihood\":null,\"duration\":107,\"talk_time\":101,\"ring_time\":6,\"parent_id\":null,\"email\":null,\"street\":null,\"city\":\"\",\"state\":\"Stockholm\",\"country\":\"SE\",\"postal_code\":\"\",\"called_at\":\"2013-01-01 10:17 AM +02:00\",\"tracking_number_id\":28938,\"tracking_number\":\"+46844680390\",\"tracking_label\":null,\"business_number\":\"+4687541040\",\"business_label\":\"Winassist Växel\",\"receiving_number_id\":14464,\"dial_status\":\"completed\",\"billed_amount\":1080.0,\"billed_at\":\"2013-10-01T08:19:09\",\"caller_number_split\":[\"46\",\"8\",\"890443\"],\"excluded\":false,\"tracking_number_format\":\"+46-8-44680390\",\"business_number_format\":\"+46-8-7541040\",\"caller_number_format\":\"(+46) 889-0443\",\"alternative_number\":\"+468890443\",\"caller_number_complete\":\"+468890443\",\"caller_number\":\"+468890443\",\"visitor\":false,\"audio\":\"https://ct4.s3.amazonaws.com/accounts/AC31fe87364641848e5d08c041cac16c84/recordings/REef08b869be099e8c7aeb0d85b04a99d2\",\"tag_list\":[],\"notes\":null,\"latitude\":59.3323,\"longitude\":18.0629,\"extended_lookup_on\":false}";

            var ctmDataDeSerialized = JsonConvert.DeserializeObject<CallTrackingMetricsWebhookData>(sampleJsonData);

            var contact = xDataProvider.MapPhoneDataToContact(ctmDataDeSerialized);

            Assert.AreEqual("Phone", contact.LeadType);
            Assert.AreEqual("3158334", contact.Property.First(lp => lp.Type == "CTMId").Value);
            Assert.AreEqual("https://ct4.s3.amazonaws.com/accounts/AC31fe87364641848e5d08c041cac16c84/recordings/REef08b869be099e8c7aeb0d85b04a99d2", contact.Property.First(lp => lp.Type == "Audio").Value);
            Assert.AreEqual("+4687541040", contact.Property.First(lp => lp.Type == "BusinessNumber").Value);
            Assert.AreEqual("+468890443", contact.Property.First(lp => lp.Type == "CallerNumber").Value);
            Assert.AreEqual("SE", contact.Property.First(lp => lp.Type == "Country").Value);
            Assert.AreEqual("Website", contact.Property.First(lp => lp.Type == "TrackingNumberName").Value);
            Assert.AreEqual("Stockholm", contact.Property.First(lp => lp.Type == "State").Value);
            Assert.AreEqual("completed", contact.Property.First(lp => lp.Type == "Status").Value);
            Assert.AreEqual("+46844680390", contact.Property.First(lp => lp.Type == "TrackingNumber").Value);
            Assert.AreEqual("9724", contact.Property.First(lp => lp.Type == "CTMAccoutId").Value);
            Assert.AreEqual("107", contact.Property.First(lp => lp.Type == "Duration").Value);

            Assert.IsFalse(contact.Property.Any(lp => lp.Type == "LocationUrl"));
            Assert.IsFalse(contact.Property.Any(lp => lp.Type == "ReferalUrl"));
            Assert.IsFalse(contact.Property.Any(lp => lp.Type == "SearchPhrase"));
            Assert.IsFalse(contact.Property.Any(lp => lp.Type == "City"));
            Assert.IsFalse(contact.Property.Any(lp => lp.Type == "PostalCode"));
        }
    }
}
