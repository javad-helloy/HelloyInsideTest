using System;
using System.Linq;
using Helpers.test;
using Inside.AutoRating;
using Inside.ExternalData;
using InsideModel.Models;
using InsideModel.repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Task.UpdatePhonecalls.UpdatePhoneCalls;

namespace BackgroundWorker.Test.UpdatePhoneCalls
{
    [TestClass]
    public class UpdatePhoneCallsTest
    {
        [TestMethod]
        public void CanConstruct()
        {
            var clientRepository = new LocalRepository<Client>();
            var externalDataProvider = new Mock<IExternalDataProvider>();
            var contactRepository = new LocalRepository<Contact>();
            var contactAutoRating = new Mock<IContactAutoRating>();

            var task = new UpdatePhonecalls(
                 clientRepository,
                 contactRepository,
                 externalDataProvider.Object,
                 contactAutoRating.Object);
        }

        [TestMethod]
        public void CanPerfomTask()
        {
            var clientRepository = new LocalRepository<Client>();
            var externalDataProvider = new Mock<IExternalDataProvider>();
            var contactRepository = new LocalRepository<Contact>();
            var contactAutoRating = new Mock<IContactAutoRating>();

            var task = new UpdatePhonecalls(
                 clientRepository,
                 contactRepository,
                 externalDataProvider.Object,
                 contactAutoRating.Object);

            var result = task.CanPerformTask("UpdatePhonecalls");
            Assert.IsTrue(result);

            var result2 = task.CanPerformTask("Not Update Phonecall Task");
            Assert.IsFalse(result2);
        }

        [TestMethod]
        public void CanPerfomTaskForClientsTranslateAllCallsInData()
        {
            var clientRepository = new LocalRepository<Client>();
            var externalDataProvider = new Mock<IExternalDataProvider>();
            var contactRepository = new Mock<IRepository<Contact>>();
            var contactAutoRating = new Mock<IContactAutoRating>();

            clientRepository.Add(ModelHelper.TestClient1AllDataNoReferences);

            var startDate = new DateTime(2014, 01, 01);
            var endDate = new DateTime(2014, 02, 01);

            externalDataProvider.Setup(x => x.GetPhoneData(1, startDate, endDate)).Returns("{\"calls\":[{\"id\": 1,\"account_id\":1}, {\"id\": 2,\"account_id\":1}],\"next_page\":null,\"previous_page\":null,\"total_pages\":1,\"page\":1}");

            var task = new UpdatePhonecalls(
                 clientRepository,
                 contactRepository.Object,
                 externalDataProvider.Object,
                 contactAutoRating.Object);

            var taskMessage = "{\"ClientId\":1,\"StartDate\":\"2014-01-01T00:00:00\",\"EndDate\":\"2014-02-01T00:00:00\"}";
            var message = new InsideModel.Models.Task() { Message = taskMessage };

            task.PerformTask(message);
            
            externalDataProvider.Verify(x => x.MapPhoneDataToContact(It.Is<CallTrackingMetricsWebhookData>(ctm=>ctm.id==1)), Times.Once);
            externalDataProvider.Verify(x => x.MapPhoneDataToContact(It.Is<CallTrackingMetricsWebhookData>(ctm => ctm.id == 2)), Times.Once);

            contactRepository.Verify(cr=>cr.Add(It.IsAny<Contact>()), Times.Exactly(2));
        }

        [TestMethod]
        public void CanPerfomTaskForClientsTranslateAllCallsInDataWithMoreThanOnePage()
        {
            var clientRepository = new LocalRepository<Client>();
            var externalDataProvider = new Mock<IExternalDataProvider>();
            var contactRepository = new Mock<IRepository<Contact>>();
            var contactAutoRating = new Mock<IContactAutoRating>();

            clientRepository.Add(ModelHelper.TestClient1AllDataNoReferences);

            var startDate = new DateTime(2014, 01, 01);
            var endDate = new DateTime(2014, 02, 01);
            externalDataProvider.Setup(x => x.GetPhoneData(1, startDate, endDate)).Returns("{\"calls\":[{\"id\": 1,\"account_id\":1}, {\"id\": 2,\"account_id\":1}],\"next_page\":\"www.url.com/toNextPage\",\"previous_page\":null,\"total_pages\":2,\"page\":1}");

            externalDataProvider.Setup(x => x.GetPhoneData("www.url.com/toNextPage")).Returns("{\"calls\":[{\"id\": 3,\"account_id\":1}, {\"id\": 4,\"account_id\":1}],\"next_page\":null,\"previous_page\":null,\"total_pages\":2,\"page\":2}");

            var task = new UpdatePhonecalls(
                 clientRepository,
                 contactRepository.Object,
                 externalDataProvider.Object,
                 contactAutoRating.Object);

            var taskMessage = "{\"ClientId\":1,\"StartDate\":\"2014-01-01T00:00:00\",\"EndDate\":\"2014-02-01T00:00:00\"}";
            var message = new InsideModel.Models.Task() { Message = taskMessage };

            task.PerformTask(message);

            externalDataProvider.Verify(x => x.MapPhoneDataToContact(It.IsAny<CallTrackingMetricsWebhookData>()), Times.Exactly(4));
            externalDataProvider.Verify(x => x.MapPhoneDataToContact(It.Is<CallTrackingMetricsWebhookData>(ctm => ctm.id == 1)), Times.Once);
            externalDataProvider.Verify(x => x.MapPhoneDataToContact(It.Is<CallTrackingMetricsWebhookData>(ctm => ctm.id == 2)), Times.Once);
            externalDataProvider.Verify(x => x.MapPhoneDataToContact(It.Is<CallTrackingMetricsWebhookData>(ctm => ctm.id == 3)), Times.Once);
            externalDataProvider.Verify(x => x.MapPhoneDataToContact(It.Is<CallTrackingMetricsWebhookData>(ctm => ctm.id == 4)), Times.Once);

            contactRepository.Verify(cr=>cr.Add(It.IsAny<Contact>()), Times.Exactly(4));
        }

        [TestMethod]
        public void NoPhoneCallsForDatePeriodDoesNotTranslateToAnyContact()
        {
            var clientRepository = new LocalRepository<Client>();
            var externalDataProvider = new Mock<IExternalDataProvider>();
            var contactRepository = new Mock<IRepository<Contact>>();
            var contactAutoRating = new Mock<IContactAutoRating>();

            clientRepository.Add(ModelHelper.TestClient1AllDataNoReferences);

            var startDate = new DateTime(2014, 01, 01);
            var endDate = new DateTime(2014, 02, 01);
            externalDataProvider.Setup(x => x.GetPhoneData(1, startDate, endDate)).Returns("");

            var task = new UpdatePhonecalls(
                 clientRepository,
                 contactRepository.Object,
                 externalDataProvider.Object,
                 contactAutoRating.Object);

            var taskMessage = "{\"ClientId\":1,\"StartDate\":\"2014-01-01T00:00:00\",\"EndDate\":\"2014-02-01T00:00:00\"}";
            var message = new InsideModel.Models.Task() { Message = taskMessage };

            task.PerformTask(message);

            externalDataProvider.Verify(x => x.MapPhoneDataToContact(It.IsAny<CallTrackingMetricsWebhookData>()), Times.Never);

            contactRepository.Verify(cr=>cr.Add(It.IsAny<Contact>()), Times.Never);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void NoCtmIdThrowsException()
        {
            var clientRepository = new LocalRepository<Client>();
            var externalDataProvider = new Mock<IExternalDataProvider>();
            var contactRepository = new Mock<IRepository<Contact>>();
            var contactAutoRating = new Mock<IContactAutoRating>();

            clientRepository.Add(new Client(){Id=1});

            var startDate = new DateTime(2014, 01, 01);
            var endDate = new DateTime(2014, 02, 01);
            externalDataProvider.Setup(x => x.GetPhoneData(1, startDate, endDate)).Returns("{\"calls\":[{\"id\": 1}, {\"id\": 2}],\"next_page\":null,\"previous_page\":null,\"total_pages\":1,\"page\":1}");

            var task = new UpdatePhonecalls(
                 clientRepository,
                 contactRepository.Object,
                 externalDataProvider.Object,
                 contactAutoRating.Object);

            var taskMessage = "{\"ClientId\":1,\"StartDate\":\"2014-01-01T00:00:00\",\"EndDate\":\"2014-02-01T00:00:00\"}";
            var message = new InsideModel.Models.Task() { Message = taskMessage };

            task.PerformTask(message);

            externalDataProvider.Verify(x => x.MapPhoneDataToContact(It.IsAny<CallTrackingMetricsWebhookData>()), Times.Never);
            contactRepository.Verify(cr => cr.Add(It.IsAny<Contact>()), Times.Never);
        }

        [TestMethod]
        public void ContactInDbDoesNotAddContactModifiesState()
        {
            var clientRepository = new LocalRepository<Client>();
            var externalDataProvider = new Mock<IExternalDataProvider>();
            var contactRepository = new LocalRepository<Contact>();
            var contactAutoRating = new Mock<IContactAutoRating>();

            clientRepository.Add(ModelHelper.TestClient1AllDataNoReferences);

            var contactProperty = new ContactProperty("CTMId", "1");
            var contact = new Contact()
            {
                Id = 1,
                ClientId = 1
            };
            contact.Property.Add(contactProperty);
            contactRepository.Add(contact);

            var startDate = new DateTime(2014, 01, 01);
            var endDate = new DateTime(2014, 02, 01);
            externalDataProvider.Setup(x => x.GetPhoneData(1, startDate, endDate)).Returns("{\"calls\":[{\"id\": 1,\"account_id\":1}, {\"id\": 2,\"account_id\":1}],\"next_page\":null,\"previous_page\":null,\"total_pages\":1,\"page\":1}");

            var task = new UpdatePhonecalls(
                 clientRepository,
                 contactRepository,
                 externalDataProvider.Object,
                 contactAutoRating.Object);

            var taskMessage = "{\"ClientId\":1,\"StartDate\":\"2014-01-01T00:00:00\",\"EndDate\":\"2014-02-01T00:00:00\"}";
            var message = new InsideModel.Models.Task() { Message = taskMessage };

            task.PerformTask(message);

            externalDataProvider.Verify(x => x.MapPhoneDataToContact(It.IsAny<CallTrackingMetricsWebhookData>()), Times.Once);
            externalDataProvider.Verify(x => x.MapPhoneDataToContact(It.IsAny<CallTrackingMetricsWebhookData>(), It.IsAny<Contact>()), Times.Once);

            Assert.AreEqual(2, contactRepository.All().Count());

        }
    }
}
