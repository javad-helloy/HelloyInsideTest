using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Helpers.test;
using Inside.AutoRating;
using Inside.ContactService;
using Inside.ExternalData;
using Inside.Time;
using InsideModel.Models;
using InsideModel.repositories;
using InsideReporting.Controllers.External;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;

namespace InsideReporting.Tests.Controllers.External
{
    [TestClass]
    public class WebhookControllerTest
    {
        [TestMethod]
        public void CanConstruct()
        {
            var leadsRepository = new Mock<IRepository<Contact>>();;
            var clientRepository = new Mock<IRepository<Client>>();
            var contactService = new Mock<IContactService>();
            var serverTime = new Mock<IServerTime>();
            var xDataProvider = new Mock<IExternalDataProvider>();
            var contactAutoRating = new Mock<IContactAutoRating>();

            var controller = new WebhookController(leadsRepository.Object, clientRepository.Object, contactService.Object, serverTime.Object, xDataProvider.Object, contactAutoRating.Object);
        }


        [TestMethod]
        public void CallTrackingMetricWebhookCreatesNewLead()
        {
            var request = new Mock<HttpRequestBase>();
            var serverTime = new Mock<IServerTime>();
            var xDataProvider = new Mock<IExternalDataProvider>();
            var contactAutoRating = new Mock<IContactAutoRating>();

            serverTime.Setup(st => st.RequestStarted).Returns(new DateTime(2013, 01, 01, 8, 20, 0));
            
            string sampleJsonData =
                "{\"id\":1,\"account_id\":1,\"caller_number\":\"08123456\", \"called_at\":\"2013-01-01 10:17 AM +02:00\"}";

            var serializeObject =
                new MemoryStream(
                    Encoding.UTF8.GetBytes(
                        sampleJsonData));

            request.SetupGet(x => x.InputStream).Returns(serializeObject);

            var context = new Mock<HttpContextBase>();
            context.SetupGet(x => x.Request).Returns(request.Object);
            
            var leadsRepository = new Mock<IRepository<Contact>>();
            
            var clientRepository = new LocalRepository<Client>();
            var client = ModelHelper.TestClient1AllDataNoReferences;
            clientRepository.Add(client);

            var contactService = new Mock<IContactService>();

            var returnContact = new Contact()
            {
                Id = 1,
                Date = new DateTime(2013, 01, 01, 08, 17, 0)
            };
            xDataProvider.Setup(x => x.MapPhoneDataToContact(It.Is<CallTrackingMetricsWebhookData>(ctm => ctm.id == 1)))
                .Returns(returnContact);
            var controller = new WebhookController(leadsRepository.Object, clientRepository, contactService.Object, serverTime.Object, xDataProvider.Object, contactAutoRating.Object);

            controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);
            controller.CallTrackingMetricWebhook();

            leadsRepository.Verify(pc => pc.Add(It.IsAny<Contact>()), Times.Once());
            leadsRepository.Verify(pc => pc.SaveChanges(), Times.Once());
            contactService.Verify(cs => cs.NotifyClientsForNewContactWithEmail(It.IsAny<int>()), Times.Once());
            contactService.Verify(cs => cs.NotifyClientsForNewContactWithPhoneNotification(It.IsAny<int>()), Times.Once());
            contactService.Verify(cs => cs.NotifyClientsForNewContactWithSmsNotification(It.IsAny<int>()), Times.Once());
            contactAutoRating.Verify(ar => ar.SetAutoRating(It.Is<Contact>(c => c.Id == returnContact.Id)), Times.Once());
        }

        [TestMethod]
        public void CallTrackingMetricWebhookUpdatesOldContact()
        {
            var request = new Mock<HttpRequestBase>();
            var serverTime = new Mock<IServerTime>();
            var xDataProvider = new Mock<IExternalDataProvider>();
            var contactAutoRating = new Mock<IContactAutoRating>();

            serverTime.Setup(st => st.RequestStarted).Returns(new DateTime(2013, 01, 01, 8, 20, 0));

            string sampleJsonData =
                "{\"id\":1,\"account_id\":1,\"caller_number\":\"08123456\", \"called_at\":\"2013-01-01 10:17 AM +02:00\"}";

            var serializeObject =
                new MemoryStream(
                    Encoding.UTF8.GetBytes(
                        sampleJsonData));

            request.SetupGet(x => x.InputStream).Returns(serializeObject);

            var context = new Mock<HttpContextBase>();
            context.SetupGet(x => x.Request).Returns(request.Object);

            var leadsRepository = new LocalRepository<Contact>();
            var contact = new Contact()
            {
                Id = 1,
                ClientId = 1
            };
            var contactProperty1 = new ContactProperty("CTMId", "1");
            contact.Property.Add(contactProperty1);
            leadsRepository.Add(contact);

            var clientRepository = new LocalRepository<Client>();
            var client = ModelHelper.TestClient1AllDataNoReferences;
            clientRepository.Add(client);

            var contactService = new Mock<IContactService>();


            var controller = new WebhookController(leadsRepository, clientRepository, contactService.Object, serverTime.Object, xDataProvider.Object, contactAutoRating.Object);

            controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);
            controller.CallTrackingMetricWebhook();

            xDataProvider.Verify(x=>x.MapPhoneDataToContact(It.Is<CallTrackingMetricsWebhookData>(ctm=>ctm.id==1), It.Is<Contact>(c=>c.Id==1)), Times.Once);
            contactService.Verify(cs => cs.NotifyClientsForNewContactWithEmail(It.IsAny<int>()), Times.Never);
            contactService.Verify(cs => cs.NotifyClientsForNewContactWithPhoneNotification(It.IsAny<int>()), Times.Never);
            contactService.Verify(cs => cs.NotifyClientsForNewContactWithSmsNotification(It.IsAny<int>()), Times.Never);
            contactAutoRating.Verify(ar => ar.SetAutoRating(It.Is<Contact>(c => c.Id == contact.Id)), Times.Once());
        }

        [TestMethod]
        public void CallTrackingMetricLeadsOlderThanTenMinutesSaveLeadInDataBaseAndDontSendNotifications()
        {
            var request = new Mock<HttpRequestBase>();
            var serverTime = new Mock<IServerTime>();
            var xDataProvider = new Mock<IExternalDataProvider>();
            var contactAutoRating = new Mock<IContactAutoRating>();

            serverTime.Setup(st => st.RequestStarted).Returns(new DateTime(2013, 01, 01, 8, 30, 0));
            string sampleJsonData =
                "{\"id\":1,\"account_id\":1,\"caller_number\":\"08123456\", \"called_at\":\"2013-01-01 10:00 AM +02:00\"}";

            var serializeObject =
                new MemoryStream(
                    Encoding.UTF8.GetBytes(
                        sampleJsonData));

            request.SetupGet(x => x.InputStream).Returns(serializeObject);

            var context = new Mock<HttpContextBase>();
            context.SetupGet(x => x.Request).Returns(request.Object);

            var leadsRepository = new Mock<IRepository<Contact>>();

            var clientRepository = new LocalRepository<Client>();
            var client = ModelHelper.TestClient1AllDataNoReferences;
            clientRepository.Add(client);

            var contactService = new Mock<IContactService>();


            var returnContact = new Contact()
            {
                Id = 1,
                Date = new DateTime(2013, 01, 01, 08, 0, 0)
            };
            xDataProvider.Setup(x => x.MapPhoneDataToContact(It.Is<CallTrackingMetricsWebhookData>(ctm => ctm.id == 1)))
                .Returns(returnContact);

            var controller = new WebhookController(leadsRepository.Object, clientRepository, contactService.Object, serverTime.Object, xDataProvider.Object, contactAutoRating.Object);

            controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);
            controller.CallTrackingMetricWebhook();

            leadsRepository.Verify(pc => pc.Add(It.IsAny<Contact>()), Times.Once());
            leadsRepository.Verify(pc => pc.SaveChanges(), Times.Once());
            contactAutoRating.Verify(ar => ar.SetAutoRating(It.Is<Contact>(c => c.Id == returnContact.Id)), Times.Once());

            contactService.Verify(cs => cs.NotifyClientsForNewContactWithEmail(It.IsAny<int>()), Times.Never);
            contactService.Verify(cs => cs.NotifyClientsForNewContactWithPhoneNotification(It.IsAny<int>()), Times.Never);
            contactService.Verify(cs => cs.NotifyClientsForNewContactWithSmsNotification(It.IsAny<int>()), Times.Never);
        }

        [TestMethod]
        public void CallTrackingMetricWebhookCreatesLeadWithValidData()
        {
            var serverTime = new Mock<IServerTime>();

            serverTime.Setup(st => st.RequestStarted).Returns(new DateTime(2013, 01, 01, 8, 20, 0));
            var xDataProvider = new Mock<IExternalDataProvider>();
            var contactAutoRating = new Mock<IContactAutoRating>();

            var request = new Mock<HttpRequestBase>();

            string sampleJsonData =
                "{\"id\":3158334,\"account_id\":9724,\"name\":\"Restricted\",\"search\":null,\"referrer\":null,\"location\":null,\"source\":\"Website\",\"source_id\":14932,\"tgid\":28622,\"likelihood\":null,\"duration\":107,\"talk_time\":101,\"ring_time\":6,\"parent_id\":null,\"email\":null,\"street\":null,\"city\":\"\",\"state\":\"Stockholm\",\"country\":\"SE\",\"postal_code\":\"\",\"called_at\":\"2013-10-01 10:17 AM +02:00\",\"tracking_number_id\":28938,\"tracking_number\":\"+46844680390\",\"tracking_label\":null,\"business_number\":\"+4687541040\",\"business_label\":\"Winassist Växel\",\"receiving_number_id\":14464,\"dial_status\":\"completed\",\"billed_amount\":1080.0,\"billed_at\":\"2013-10-01T08:19:09\",\"caller_number_split\":[\"46\",\"8\",\"890443\"],\"excluded\":false,\"tracking_number_format\":\"+46-8-44680390\",\"business_number_format\":\"+46-8-7541040\",\"caller_number_format\":\"(+46) 889-0443\",\"alternative_number\":\"+468890443\",\"caller_number_complete\":\"+468890443\",\"caller_number\":\"+468890443\",\"visitor\":false,\"audio\":\"https://ct4.s3.amazonaws.com/accounts/AC31fe87364641848e5d08c041cac16c84/recordings/REef08b869be099e8c7aeb0d85b04a99d2\",\"tag_list\":[],\"notes\":null,\"latitude\":59.3323,\"longitude\":18.0629,\"extended_lookup_on\":false}";

            var serializeObject =
                new MemoryStream(
                    Encoding.UTF8.GetBytes(
                        sampleJsonData));

            request.SetupGet(x => x.InputStream).Returns(serializeObject);

            var context = new Mock<HttpContextBase>();
            context.SetupGet(x => x.Request).Returns(request.Object);

            var leadsRepository = new LocalRepository<Contact>();
            
            var clientRepository = new LocalRepository<Client>();
            var client = ModelHelper.TestClient1AllDataNoReferences;
            client.CallTrackingMetricId = 9724;
            clientRepository.Add(client);

            var contactService = new Mock<IContactService>();

            var returnContact = new Contact()
            {
                Id = 1,
                Date = new DateTime(2013, 01, 01, 08, 17, 0)
            };
            xDataProvider.Setup(x => x.MapPhoneDataToContact(It.Is<CallTrackingMetricsWebhookData>(ctm => ctm.id == 3158334)))
                .Returns(returnContact);

            var controller = new WebhookController(leadsRepository, clientRepository, contactService.Object, serverTime.Object, xDataProvider.Object, contactAutoRating.Object);

            controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);
            controller.CallTrackingMetricWebhook();

            Assert.AreEqual(1, leadsRepository.All().Count());

            contactAutoRating.Verify(ar => ar.SetAutoRating(It.Is<Contact>(c => c.Id == returnContact.Id)), Times.Once());

            contactService.Verify(cs => cs.NotifyClientsForNewContactWithEmail(It.IsAny<int>()), Times.Once());
            contactService.Verify(cs => cs.NotifyClientsForNewContactWithPhoneNotification(It.IsAny<int>()), Times.Once());
            contactService.Verify(cs => cs.NotifyClientsForNewContactWithSmsNotification(It.IsAny<int>()), Times.Once());
        }

        [TestMethod]
        [ExpectedException(typeof(JsonReaderException))]
        public void CallTrackingMetricWebhookThrowsExceptionTest()
        {
            var request = new Mock<HttpRequestBase>();
            var serverTime = new Mock<IServerTime>();
            var xDataProvider = new Mock<IExternalDataProvider>();
            var contactAutoRating = new Mock<IContactAutoRating>();

            string sampleInvalidJsonData =
                "{\"invalid\":Data\"}";
            var serializeObject =
                new MemoryStream(
                    Encoding.UTF8.GetBytes(
                        sampleInvalidJsonData));
            request.SetupGet(x => x.InputStream).Returns(serializeObject);

            var context = new Mock<HttpContextBase>();
            context.SetupGet(x => x.Request).Returns(request.Object);
            
            var leadsRepository = new Mock<IRepository<Contact>>();
            var clientRepository = new Mock<IRepository<Client>>();
            var contactService = new Mock<IContactService>();

            var controller = new WebhookController(leadsRepository.Object, clientRepository.Object, contactService.Object, serverTime.Object, xDataProvider.Object, contactAutoRating.Object);

            controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);
            controller.CallTrackingMetricWebhook();

            contactAutoRating.Verify(ar => ar.SetAutoRating(It.IsAny<Contact>()), Times.Never);

            contactService.Verify(cs => cs.NotifyClientsForNewContactWithEmail(It.IsAny<int>()), Times.Never);
            contactService.Verify(cs => cs.NotifyClientsForNewContactWithPhoneNotification(It.IsAny<int>()), Times.Never);
            contactService.Verify(cs => cs.NotifyClientsForNewContactWithSmsNotification(It.IsAny<int>()), Times.Never);
        }

        [TestMethod]
        public void MandrillContentWebhookHasEmailInRepository()
        {
            var leadRepository = new LocalRepository<Contact>();
            var emailLead = ModelHelper.EmailLead;
            leadRepository.Add(emailLead);
            var serverTime = new Mock<IServerTime>();
            var clientRepository = new Mock<IRepository<Client>>();
            var contactService = new Mock<IContactService>();
            var xDataProvider = new Mock<IExternalDataProvider>();
            var contactAutoRating = new Mock<IContactAutoRating>();


            serverTime.Setup(st => st.RequestStarted).Returns(new DateTime(2013, 01, 01, 9, 0, 0));

            var emailController = new WebhookController(leadRepository, clientRepository.Object, contactService.Object, serverTime.Object, xDataProvider.Object, contactAutoRating.Object);

            emailController.MandrillContentWebhook(new MadrillContentPost{id = "1", html = "<p>New Sample Text</p>", text = "New Sample Text", to = "info@foretaget.se"});
            Assert.AreEqual(1, leadRepository.All().Count());
            Assert.AreEqual("New Sample Text", leadRepository.All().First().Property.First(lp => lp.Type == "Text").Value);
            Assert.AreEqual("<p>New Sample Text</p>", leadRepository.All().First().Property.First(lp => lp.Type == "Html").Value);
            contactAutoRating.Verify(ar => ar.SetAutoRating(It.Is<Contact>(c => c.Id == emailLead.Id)), Times.Once());

            contactService.Verify(cs => cs.NotifyClientsForNewContactWithEmail(It.IsAny<int>()), Times.Never);
            contactService.Verify(cs => cs.NotifyClientsForNewContactWithPhoneNotification(It.IsAny<int>()), Times.Never);
            contactService.Verify(cs => cs.NotifyClientsForNewContactWithSmsNotification(It.IsAny<int>()), Times.Never);
        }

        [TestMethod]
        public void MandrillContentWebhookDoesntHaveEmailInRepository()
        {
            var leadRepository = new LocalRepository<Contact>();

            var emailLead = ModelHelper.EmailLead;
            leadRepository.Add(emailLead);
            var serverTime = new Mock<IServerTime>();
            var clientRepository = new LocalRepository<Client>();
            clientRepository.Add(ModelHelper.TestClient1AllDataNoReferences);
            var xDataProvider = new Mock<IExternalDataProvider>();
            var contactAutoRating = new Mock<IContactAutoRating>();

            var contactService = new Mock<IContactService>();
            serverTime.Setup(st => st.RequestStarted).Returns(new DateTime(2013, 01, 01, 9, 0, 0));
            var emailController = new WebhookController(leadRepository, clientRepository, contactService.Object, serverTime.Object, xDataProvider.Object, contactAutoRating.Object);

            emailController.MandrillContentWebhook(new MadrillContentPost{id= "2", html = "<p>New Sample Text</p>", text = "New Sample Text", to = "info@foretaget.se"});
            Assert.AreEqual(2, leadRepository.All().Count());
            Assert.AreEqual("New Sample Text", leadRepository.Where(e => e.Property.Any(lp => lp.Type == "MandrillId" && lp.Value =="2")).First().Property.First(lp => lp.Type == "Text").Value);
            Assert.AreEqual("<p>New Sample Text</p>", leadRepository.Where(e => e.Property.Any(lp => lp.Type == "MandrillId" && lp.Value =="2")).First().Property.First(lp => lp.Type == "Html").Value);
            contactAutoRating.Verify(ar => ar.SetAutoRating(It.IsAny<Contact>()), Times.Once());

            contactService.Verify(cs => cs.NotifyClientsForNewContactWithEmail(It.IsAny<int>()), Times.Once());
            contactService.Verify(cs => cs.NotifyClientsForNewContactWithPhoneNotification(It.IsAny<int>()), Times.Once());
            contactService.Verify(cs => cs.NotifyClientsForNewContactWithSmsNotification(It.IsAny<int>()), Times.Once());
        }

        [TestMethod]
        public void ThrowsExceptionWhenAddingTheSameEmailUpdatesExistingDataWithNewData()
        {
            var leadRepository = new Mock<LocalRepository<Contact>> { CallBase = true };
            var xDataProvider = new Mock<IExternalDataProvider>();
            var emailLead = ModelHelper.EmailLead;
            var contactAutoRating = new Mock<IContactAutoRating>();

            var numCalls = 0;
            Action throwExceptionOnFirstCall = () =>
            {
                if (numCalls == 0)
                {
                    numCalls++;
                    throw new Exception();
                }
                else
                {
                    numCalls++;
                }
            };
            leadRepository.Setup(er => er.SaveChanges())
                           .Callback(throwExceptionOnFirstCall);
            leadRepository.Object.Add(emailLead);

            var clientRepository = new LocalRepository<Client>();
            var client = ModelHelper.TestClient1AllDataNoReferences;
            clientRepository.Add(client);
            var serverTime = new Mock<IServerTime>();
            var contactService = new Mock<IContactService>();

            serverTime.Setup(st => st.RequestStarted).Returns(new DateTime(2013, 01, 01, 9, 0, 0));

            var emailController = new WebhookController(leadRepository.Object, clientRepository, contactService.Object, serverTime.Object, xDataProvider.Object, contactAutoRating.Object);
            emailController.MandrillContentWebhook(new MadrillContentPost{id = "1", html = "<p>Very New Sample Text</p>", text = "Very New Sample Text", to = "info@foretaget.se"});


            Assert.AreEqual(1, leadRepository.Object.Where(er => er.Property.Any( lp => lp.Type =="Html" && lp.Value == "<p>Very New Sample Text</p>")).Count());
            Assert.AreEqual(1, leadRepository.Object.Where(er => er.Property.Any(lp => lp.Type == "Text" && lp.Value == "Very New Sample Text")).Count());
            Assert.AreEqual(0, leadRepository.Object.Where(er => er.Property.Any(lp => lp.Type == "Text" && lp.Value == "New Sample Text")).Count());
            contactAutoRating.Verify(ar => ar.SetAutoRating(It.IsAny<Contact>()), Times.Exactly(2));

            contactService.Verify(cs => cs.NotifyClientsForNewContactWithEmail(It.IsAny<int>()), Times.Never);
            contactService.Verify(cs => cs.NotifyClientsForNewContactWithPhoneNotification(It.IsAny<int>()), Times.Never);
            contactService.Verify(cs => cs.NotifyClientsForNewContactWithSmsNotification(It.IsAny<int>()), Times.Never);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void ThrowsExceptionWhenAddingTheSameEmailUpdatesExistingDataWithNewDataAndOnRetryFails()
        {
            var leadRepository = new Mock<LocalRepository<Contact>> { CallBase = true };
            var serverTime = new Mock<IServerTime>();
            var emailLead = ModelHelper.EmailLead;
            var xDataProvider = new Mock<IExternalDataProvider>();
            var contactAutoRating = new Mock<IContactAutoRating>();

            leadRepository.Setup(er => er.SaveChanges()).Throws(new Exception());
            leadRepository.Object.Add(emailLead);

            var clientRepository = new LocalRepository<Client>();
            var client = ModelHelper.TestClient1AllDataNoReferences;
            clientRepository.Add(client);

            var contactService = new Mock<IContactService>();

            var emailController = new WebhookController(leadRepository.Object, clientRepository, contactService.Object, serverTime.Object, xDataProvider.Object, contactAutoRating.Object);
            emailController.MandrillContentWebhook(new MadrillContentPost{id = "1",html =  "<p>Very New Sample Text</p>", text = "Very New Sample Text", to = "info@foretaget.se"});

            Assert.AreEqual(0, leadRepository.Object.Where(er => er.Property.Any(lp => lp.Type == "Html" && lp.Value == "<p>Very New Sample Text</p>")).Count());
            Assert.AreEqual(0, leadRepository.Object.Where(er => er.Property.Any(lp => lp.Type == "Text" && lp.Value == "Very New Sample Text")).Count());
            Assert.AreEqual(1, leadRepository.Object.Where(er => er.Property.Any(lp => lp.Type == "Text" && lp.Value == "New Sample Text")).Count());
            contactAutoRating.Verify(ar => ar.SetAutoRating(It.IsAny<Contact>()), Times.Exactly(2));

            contactService.Verify(cs => cs.NotifyClientsForNewContactWithEmail(It.IsAny<int>()), Times.Never);
            contactService.Verify(cs => cs.NotifyClientsForNewContactWithPhoneNotification(It.IsAny<int>()), Times.Never);
            contactService.Verify(cs => cs.NotifyClientsForNewContactWithSmsNotification(It.IsAny<int>()), Times.Never);
        }

        [TestMethod]
        public void MultiMandrilleventsWithSameIdIsNotDuplicatedInRepository()
        {
            var leadRepository = new LocalRepository<Contact>();
            var xDataProvider = new Mock<IExternalDataProvider>();
            var contactAutoRating = new Mock<IContactAutoRating>();
            var mandrill_event =
                "[{\"event\":\"send\",\"msg\":{\"ts\":1365109999,\"subject\":\"This an example webhook message\",\"email\":\"example.webhook@mandrillapp.com\"," +
                "\"sender\":\"example.sender@mandrillapp.com\",\"tags\":[\"webhook-example\"],\"opens\":[],\"clicks\":[],\"state\":\"sent\",\"metadata\":{\"user_id\":111}," +
                "\"_id\":\"1\",\"_version\":\"exampleaaaaaaaaaaaaaaa1\"},\"_id\":\"1\",\"ts\":1385020180}," +

                "{\"event\":\"send\",\"msg\":{\"ts\":1365109999,\"subject\":\"This an example webhook message\",\"email\":\"example.webhook@mandrillapp.com\"," +
                "\"sender\":\"example.sender@mandrillapp.com\",\"tags\":[\"webhook-example\"],\"opens\":[],\"clicks\":[],\"state\":\"sent\",\"metadata\":{\"user_id\":111}," +
                "\"_id\":\"1\",\"_version\":\"exampleaaaaaaaaaaaaaaa1\"},\"_id\":\"1\",\"ts\":1385020180}," +

                "{\"event\":\"send\",\"msg\":{\"ts\":1365109999,\"subject\":\"This an example webhook message2\",\"email\":\"example2.webhook@mandrillapp.com\"," +
                "\"sender\":\"example2.sender@mandrillapp.com\",\"tags\":[\"webhook-example\"],\"opens\":[],\"clicks\":[],\"state\":\"sent\",\"metadata\":{\"user_id\":222}," +
                "\"_id\":\"2\",\"_version\":\"exampleaaaaaaaaaaaaaaa2\"},\"_id\":\"2\",\"ts\":1385020180}]";

            var clientRepository = new LocalRepository<Client>();
            var client = ModelHelper.TestClient1AllDataNoReferences;
            client.EmailAddress = "example.webhook@mandrillapp.com";
            clientRepository.Add(client);

            var client2 = ModelHelper.TestClient1AllDataNoReferences;
            client2.EmailAddress = "example2.webhook@mandrillapp.com";
            client2.Id = 2;
            clientRepository.Add(client2);

            var serverTime = new Mock<IServerTime>();
            serverTime.Setup(st => st.RequestStarted).Returns(new DateTime(2013, 11, 21, 8, 50, 0)); //2013-11-21 08:49:40 on email date

            var contactService = new Mock<IContactService>();

            var emailController = new WebhookController(leadRepository, clientRepository, contactService.Object, serverTime.Object, xDataProvider.Object, contactAutoRating.Object);
            var result = emailController.ManrillWebhook(mandrill_event);

            Assert.AreEqual(2, leadRepository.All().Count());

            Assert.AreEqual("This an example webhook message", leadRepository.Where(l => l.Property.Any(lp => lp.Type == "MandrillId" && lp.Value == "1")).First().Property.First(lp => lp.Type == "Subject").Value);
            Assert.AreEqual("This an example webhook message2", leadRepository.Where(l => l.Property.Any(lp => lp.Type == "MandrillId" && lp.Value == "2")).First().Property.First(lp => lp.Type == "Subject").Value);
            contactAutoRating.Verify(ar => ar.SetAutoRating(It.IsAny<Contact>()), Times.Exactly(3));

            contactService.Verify(cs => cs.NotifyClientsForNewContactWithEmail(It.IsAny<int>()), Times.Exactly(2));
            contactService.Verify(cs => cs.NotifyClientsForNewContactWithPhoneNotification(It.IsAny<int>()), Times.Exactly(2));
            contactService.Verify(cs => cs.NotifyClientsForNewContactWithSmsNotification(It.IsAny<int>()), Times.Exactly(2));
        }

        [TestMethod]
        public void GetOneMandrillEventsAndThrowsExceptionAndRetriesSuccessfully()
        {
            var leadRepository = new Mock<LocalRepository<Contact>>() { CallBase = true };
            var xDataProvider = new Mock<IExternalDataProvider>();
            var contactAutoRating = new Mock<IContactAutoRating>();

            var mandrill_event =
                "[{\"event\":\"send\",\"msg\":{\"ts\":1365109999,\"subject\":\"This an example webhook message\",\"email\":\"example.webhook@mandrillapp.com\"," +
                "\"sender\":\"example.sender@mandrillapp.com\",\"tags\":[\"webhook-example\"],\"opens\":[],\"clicks\":[],\"state\":\"sent\",\"metadata\":{\"user_id\":111}," +
                "\"_id\":\"1\",\"_version\":\"exampleaaaaaaaaaaaaaaa1\"},\"_id\":\"1\",\"ts\":1385020180}]";


            var numCalls = 0;
            Action throwExceptionOnFirstCall = () =>
            {
                if (numCalls == 0)
                {
                    numCalls++;
                    throw new Exception();
                }
                else
                {
                    numCalls++;
                }
            };
            leadRepository.Setup(er => er.SaveChanges())
                           .Callback(throwExceptionOnFirstCall);

            var clientRepository = new LocalRepository<Client>();
            var client = ModelHelper.TestClient1AllDataNoReferences;
            client.EmailAddress = "example.webhook@mandrillapp.com";
            clientRepository.Add(client);

            var contactService = new Mock<IContactService>();
            var serverTime = new Mock<IServerTime>();
            serverTime.Setup(st => st.RequestStarted).Returns(new DateTime(2013, 11, 21, 8, 50, 0)); //2013-11-21 08:49:40 on email date

            var emailController = new WebhookController(leadRepository.Object, clientRepository, contactService.Object, serverTime.Object, xDataProvider.Object, contactAutoRating.Object);
            var result = emailController.ManrillWebhook(mandrill_event);

            Assert.AreEqual(1, leadRepository.Object.All().Count());
            string propertyValue = leadRepository.Object.Where(l => l.Property.Any(lp => lp.Type == "MandrillId" && lp.Value == "1")).Single().Property.First(lp => lp.Type == "Subject").Value;
            Assert.AreEqual("This an example webhook message",propertyValue);
            contactAutoRating.Verify(ar => ar.SetAutoRating(It.IsAny<Contact>()), Times.Exactly(2));

            contactService.Verify(cs => cs.NotifyClientsForNewContactWithEmail(It.IsAny<int>()), Times.Once);
            contactService.Verify(cs => cs.NotifyClientsForNewContactWithPhoneNotification(It.IsAny<int>()), Times.Once);
            contactService.Verify(cs => cs.NotifyClientsForNewContactWithSmsNotification(It.IsAny<int>()), Times.Once());
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void GetOneMandrillEventsAndThrowsExceptionFailsOnBothTries()
        {
            var leadRepository = new Mock<LocalRepository<Contact>>() { CallBase = true };
            var xDataProvider = new Mock<IExternalDataProvider>();
            var contactAutoRating = new Mock<IContactAutoRating>();

            var mandrill_event =
                "[{\"event\":\"send\",\"msg\":{\"ts\":1365109999,\"subject\":\"This an example webhook message\",\"email\":\"example.webhook@mandrillapp.com\"," +
                "\"sender\":\"example.sender@mandrillapp.com\",\"tags\":[\"webhook-example\"],\"opens\":[],\"clicks\":[],\"state\":\"sent\",\"metadata\":{\"user_id\":111}," +
                "\"_id\":\"1\",\"_version\":\"exampleaaaaaaaaaaaaaaa1\"},\"_id\":\"1\",\"ts\":1385020180}]";

            leadRepository.Setup(er => er.SaveChanges()).Throws(new Exception());

            var clientRepository = new LocalRepository<Client>();
            var client = ModelHelper.TestClient1AllDataNoReferences;
            client.EmailAddress = "example.webhook@mandrillapp.com";
            clientRepository.Add(client);
            var serverTime = new Mock<IServerTime>();
            var contactService = new Mock<IContactService>();

            var emailController = new WebhookController(leadRepository.Object, clientRepository, contactService.Object, serverTime.Object, xDataProvider.Object, contactAutoRating.Object);
            var result = emailController.ManrillWebhook(mandrill_event);

            Assert.AreEqual(0, leadRepository.Object.All().Count());
            contactAutoRating.Verify(ar => ar.SetAutoRating(It.IsAny<Contact>()), Times.Exactly(2));

            contactService.Verify(cs => cs.NotifyClientsForNewContactWithEmail(It.IsAny<int>()), Times.Never);
            contactService.Verify(cs => cs.NotifyClientsForNewContactWithPhoneNotification(It.IsAny<int>()), Times.Never);
            contactService.Verify(cs => cs.NotifyClientsForNewContactWithSmsNotification(It.IsAny<int>()), Times.Never);
        }
    }
}
