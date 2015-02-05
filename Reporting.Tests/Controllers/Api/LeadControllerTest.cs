using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Http.Routing;
using Helpers.test;
using Inside.membership;
using Inside.Time;
using InsideModel.Models;
using InsideModel.repositories;
using InsideReporting.Controllers;
using InsideReporting.Controllers.Api;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace InsideReporting.Tests.Controllers.Api
{
    [TestClass]
    public class LeadControllerTest
    {
        [TestMethod]
        public void CanCreate()
        {
            var leadRepository = new Mock<IRepository<Contact>>();
            var userManager = new Mock<IIdentityMembershipProvider>();
            var serverTime = new Mock<IServerTime>();
            var controller = new LeadController(leadRepository.Object,userManager.Object, serverTime.Object);
        }

       [TestMethod]
        public void CanGetLeadsTypeAggregateForClientFillInMissingDates()
        {
            var leadRepository = new LocalRepository<Contact>();
            var userManager = new Mock<IIdentityMembershipProvider>();
            var serverTime = new Mock<IServerTime>();
            var endDate = new DateTime(2013, 1, 31);

            serverTime.Setup(st => st.RequestStarted).Returns(endDate);

           leadRepository.Add(new Contact()
           {
               ClientId = 1,
               LeadType = "Phone",
               Date = endDate.AddMonths(-1),
               RatingScore = 3
           });
           leadRepository.Add(new Contact()
           {
               ClientId = 1,
               LeadType = "Email",
               Date = endDate.AddMonths(-1),
               RatingScore = 5
           });
           leadRepository.Add(new Contact() {ClientId = 1, LeadType = "Email", Date = endDate.AddMonths(-3)});
           leadRepository.Add(new Contact()
           {
               ClientId = 1,
               LeadType = "Phone",
               Date = endDate.AddMonths(-4),
               RatingScore = 4
           });

           leadRepository.Add(new Contact() {ClientId = 2, LeadType = "Phone", Date = endDate.AddMonths(-4)});

            var controller = new LeadController(leadRepository, userManager.Object, serverTime.Object);

            var leads = controller.GetLeadTypeAggregateMonthly(1) as OkNegotiatedContentResult<IOrderedEnumerable<LeadController.MonthlyAggregateByType>>;

            Assert.AreEqual(5, leads.Content.Count());

            Assert.AreEqual(1, leads.Content.Count(c => c.Year == 2013 && c.Month == 1));
            Assert.IsNull( leads.Content.Single(c => c.Year == 2013 && c.Month == 1).Scores.AverageScore);
            Assert.AreEqual(0, leads.Content.Single(c => c.Year == 2013 && c.Month == 1).Count.Total);
            Assert.AreEqual(0, leads.Content.Single(c => c.Year == 2013 && c.Month == 1).Count.Phone);
            Assert.AreEqual(0, leads.Content.Single(c => c.Year == 2013 && c.Month == 1).Count.Email);
            Assert.AreEqual(0, leads.Content.Single(c => c.Year == 2013 && c.Month == 1).Count.Chat);
            Assert.AreEqual(0, leads.Content.Single(c => c.Year == 2013 && c.Month == 1).Count.Event);

            Assert.AreEqual(1, leads.Content.Count(c=>c.Year==2012 && c.Month==12));
            Assert.AreEqual(4, leads.Content.Single(c => c.Year == 2012 && c.Month == 12).Scores.AverageScore);
            Assert.AreEqual(2, leads.Content.Single(c => c.Year == 2012 && c.Month == 12).Count.Total);
            Assert.AreEqual(1, leads.Content.Single(c => c.Year == 2012 && c.Month == 12).Count.Phone);
            Assert.AreEqual(1, leads.Content.Single(c => c.Year == 2012 && c.Month == 12).Count.Email);
            Assert.AreEqual(0, leads.Content.Single(c => c.Year == 2012 && c.Month == 12).Count.Chat);
            Assert.AreEqual(0, leads.Content.Single(c => c.Year == 2012 && c.Month == 12).Count.Event);

            Assert.AreEqual(1, leads.Content.Count(c => c.Year == 2012 && c.Month == 11));
            Assert.IsNull(leads.Content.Single(c => c.Year == 2012 && c.Month == 11).Scores.AverageScore);
            Assert.AreEqual(0, leads.Content.Single(c => c.Year == 2012 && c.Month == 11).Count.Total);
            Assert.AreEqual(0, leads.Content.Single(c => c.Year == 2012 && c.Month == 11).Count.Phone);
            Assert.AreEqual(0, leads.Content.Single(c => c.Year == 2012 && c.Month == 11).Count.Email);
            Assert.AreEqual(0, leads.Content.Single(c => c.Year == 2012 && c.Month == 11).Count.Chat);
            Assert.AreEqual(0, leads.Content.Single(c => c.Year == 2012 && c.Month == 11).Count.Event);

            Assert.AreEqual(1, leads.Content.Count(c => c.Year == 2012 && c.Month == 10));
            Assert.IsNull(leads.Content.Single(c => c.Year == 2012 && c.Month == 10).Scores.AverageScore);
            Assert.AreEqual(1, leads.Content.Single(c => c.Year == 2012 && c.Month == 10).Count.Total);
            Assert.AreEqual(0, leads.Content.Single(c => c.Year == 2012 && c.Month == 10).Count.Phone);
            Assert.AreEqual(1, leads.Content.Single(c => c.Year == 2012 && c.Month == 10).Count.Email);
            Assert.AreEqual(0, leads.Content.Single(c => c.Year == 2012 && c.Month == 10).Count.Chat);
            Assert.AreEqual(0, leads.Content.Single(c => c.Year == 2012 && c.Month == 10).Count.Event);

            Assert.AreEqual(1, leads.Content.Count(c => c.Year == 2012 && c.Month == 9));
            Assert.AreEqual(4, leads.Content.Single(c => c.Year == 2012 && c.Month == 9).Scores.AverageScore);
            Assert.AreEqual(1, leads.Content.Single(c => c.Year == 2012 && c.Month == 9).Count.Total);
            Assert.AreEqual(1, leads.Content.Single(c => c.Year == 2012 && c.Month == 9).Count.Phone);
            Assert.AreEqual(0, leads.Content.Single(c => c.Year == 2012 && c.Month == 9).Count.Email);
            Assert.AreEqual(0, leads.Content.Single(c => c.Year == 2012 && c.Month == 9).Count.Chat);
            Assert.AreEqual(0, leads.Content.Single(c => c.Year == 2012 && c.Month == 9).Count.Event);
        }

        [TestMethod]
        public void CanGetLeadsTypeAggregateForClientWithNoContactReturnsEmptyCurrentMonth()
        {
            var leadRepository = new LocalRepository<Contact>();
            var userManager = new Mock<IIdentityMembershipProvider>();
            var serverTime = new Mock<IServerTime>();
            var endDate = new DateTime(2013, 1, 31);

            serverTime.Setup(st => st.RequestStarted).Returns(endDate);

            var controller = new LeadController(leadRepository, userManager.Object, serverTime.Object);

            var leads = controller.GetLeadTypeAggregateMonthly(1) as OkNegotiatedContentResult<IOrderedEnumerable<LeadController.MonthlyAggregateByType>>;

            Assert.AreEqual(1, leads.Content.Count());

            Assert.AreEqual(1, leads.Content.Count(c => c.Year == 2013 && c.Month == 1));
            Assert.IsNull(leads.Content.Single(c => c.Year == 2013 && c.Month == 1).Scores.AverageScore);
            Assert.AreEqual(0, leads.Content.Single(c => c.Year == 2013 && c.Month == 1).Count.Total);
            Assert.AreEqual(0, leads.Content.Single(c => c.Year == 2013 && c.Month == 1).Count.Phone);
            Assert.AreEqual(0, leads.Content.Single(c => c.Year == 2013 && c.Month == 1).Count.Email);
            Assert.AreEqual(0, leads.Content.Single(c => c.Year == 2013 && c.Month == 1).Count.Chat);
            Assert.AreEqual(0, leads.Content.Single(c => c.Year == 2013 && c.Month == 1).Count.Event);

        }

        [TestMethod]
        public void CanGetLeadsOverAllAggregateForClientFillInMissingDates()
        {
            var leadRepository = new LocalRepository<Contact>();
            var userManager = new Mock<IIdentityMembershipProvider>();
            var serverTime = new Mock<IServerTime>();
            var endDate = new DateTime(2013, 1, 31);

            serverTime.Setup(st => st.RequestStarted).Returns(endDate);

            leadRepository.Add(new Contact() { ClientId = 1, LeadType = "Phone", Date = endDate.AddMonths(-1) });
            leadRepository.Add(new Contact() { ClientId = 1, LeadType = "Email", Date = endDate.AddMonths(-1) });
            leadRepository.Add(new Contact() { ClientId = 1, LeadType = "Email", Date = endDate.AddMonths(-3) });
            leadRepository.Add(new Contact() { ClientId = 1, LeadType = "Phone", Date = endDate.AddMonths(-4) });

            leadRepository.Add(new Contact() { ClientId = 2, LeadType = "Phone", Date = endDate.AddMonths(-4) });

            var controller = new LeadController(leadRepository, userManager.Object, serverTime.Object);

            var leads = controller.GetLeadAggregate(1) as OkNegotiatedContentResult<LeadController.OverAllAggregate>;

            
            Assert.AreEqual(4, leads.Content.Total);
            Assert.AreEqual(0, leads.Content.Aggregates.Single(c => c.Year == 2013 && c.Month == 1).ContactCount);

            Assert.AreEqual(2, leads.Content.Aggregates.Single(c => c.Year == 2012 && c.Month == 12).ContactCount);

            Assert.AreEqual(0, leads.Content.Aggregates.Single(c => c.Year == 2012 && c.Month == 11).ContactCount);

            Assert.AreEqual(1, leads.Content.Aggregates.Single(c => c.Year == 2012 && c.Month == 10).ContactCount);

            Assert.AreEqual(1, leads.Content.Aggregates.Single(c => c.Year == 2012 && c.Month == 9).ContactCount);
            
        }

        [TestMethod]
        public void CanGetLeadsAggregateForClientWithNoContactReturnsEmptyCurrentMonth()
        {
            var leadRepository = new LocalRepository<Contact>();
            var userManager = new Mock<IIdentityMembershipProvider>();
            var serverTime = new Mock<IServerTime>();
            var endDate = new DateTime(2013, 1, 31);

            serverTime.Setup(st => st.RequestStarted).Returns(endDate);

            var controller = new LeadController(leadRepository, userManager.Object, serverTime.Object);

            var leads = controller.GetLeadAggregate(1) as OkNegotiatedContentResult<LeadController.OverAllAggregate>;

            Assert.AreEqual(0, leads.Content.Total);
            Assert.AreEqual(1, leads.Content.Aggregates.Count());
            Assert.AreEqual(0, leads.Content.Aggregates.Single(c => c.Year == 2013 && c.Month == 1).ContactCount);

        }
        
        [TestMethod]
        public void CanGetPageContactList()
        {
            var leadRepository = new LocalRepository<Contact>();
            var userManager = new Mock<IIdentityMembershipProvider>();

            var date = new DateTime(2014, 10, 15, 15, 45, 33);

            leadRepository.Add(new Contact() { ClientId = 1, LeadType = "Phone", Date = date.AddDays(1) });
            leadRepository.Add(new Contact() { ClientId = 1, LeadType = "Email", Date = date.AddDays(2) });
            leadRepository.Add(new Contact() { ClientId = 1, LeadType = "Email", Date = date.AddDays(3) });
            leadRepository.Add(new Contact() { ClientId = 2, LeadType = "Phone", Date = date.AddDays(4) });

            var serverTime = new Mock<IServerTime>();
            serverTime.Setup(st => st.RequestStarted).Returns(new DateTime());

            var controller = new LeadController(leadRepository, userManager.Object, serverTime.Object);
            controller.Request = new HttpRequestMessage
            {
                RequestUri = new Uri("http://insidetest.helloy.se/api/client/1/contact/")
            };
            controller.Configuration = new HttpConfiguration();
            controller.Configuration.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional });

            controller.RequestContext.RouteData = new HttpRouteData(
                route: new HttpRoute(),
                values: new HttpRouteValueDictionary { { "ListContacts", "products" } });

            

            var response = controller.GetContacts(1) as PageResult<Contact>;
            var resultList = response.results;

            Assert.AreEqual(3, resultList.Count());

            Assert.AreEqual(new DateTime(2014, 10, 18, 15, 45, 33), resultList.ToArray()[0].Date);
            Assert.AreEqual(new DateTime(2014, 10, 16, 15, 45, 33), resultList.ToArray()[2].Date);
        }

        [TestMethod]
        public void CanGetPageContactListFilteredOnTypeEmail()
        {
            var leadRepository = new LocalRepository<Contact>();
            var userManager = new Mock<IIdentityMembershipProvider>();

            var date = new DateTime(2014, 10, 15, 15, 45, 33);

            leadRepository.Add(new Contact() { ClientId = 1, LeadType = "Phone", Date = date.AddDays(1) });
            leadRepository.Add(new Contact() { ClientId = 1, LeadType = "Email", Date = date.AddDays(2) });
            leadRepository.Add(new Contact() { ClientId = 1, LeadType = "Email", Date = date.AddDays(3) });
            leadRepository.Add(new Contact() { ClientId = 2, LeadType = "Phone", Date = date.AddDays(4) });

            var serverTime = new Mock<IServerTime>();
            serverTime.Setup(st => st.RequestStarted).Returns(new DateTime());

            var controller = new LeadController(leadRepository, userManager.Object, serverTime.Object);
            controller.Request = new HttpRequestMessage
            {
                RequestUri = new Uri("http://insidetest.helloy.se/api/client/1/contact/")
            };
            controller.Configuration = new HttpConfiguration();
            controller.Configuration.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional });

            controller.RequestContext.RouteData = new HttpRouteData(
                route: new HttpRoute(),
                values: new HttpRouteValueDictionary { { "ListContacts", "products" } });



            var response = controller.GetContacts(1, type:"Email") as PageResult<Contact>;
            var resultList = response.results;

            Assert.AreEqual(2, resultList.Count());

            Assert.AreEqual("Email", resultList.ToArray()[0].LeadType);
            Assert.AreEqual("Email", resultList.ToArray()[1].LeadType);
        }

        [TestMethod]
        public void CanGetLinkToNextPage()
        {
            var leadRepository = new LocalRepository<Contact>();
            var userManager = new Mock<IIdentityMembershipProvider>();

            var date = new DateTime(2014, 10, 15, 15, 45, 33);

            leadRepository.Add(new Contact() { ClientId = 1, LeadType = "Phone", Date = date.AddDays(1) });
            for (int i = 0; i < 35; i++)
            {
                leadRepository.Add(new Contact() { ClientId = 1, LeadType = "Phone", Date = date.AddDays(i) });    
            }

            leadRepository.Add(new Contact() { ClientId = 2, LeadType = "Phone", Date = date.AddDays(4) });

            var serverTime = new Mock<IServerTime>();
            serverTime.Setup(st => st.RequestStarted).Returns(new DateTime());

            var controller = new LeadController(leadRepository, userManager.Object, serverTime.Object);
            controller.Request = new HttpRequestMessage
            {
                RequestUri = new Uri("http://insidetest.helloy.se/api/client/1/contact/?clientId=1")
            };
            controller.Configuration = new HttpConfiguration();
            controller.Configuration.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional });
            controller.Configuration.Routes.Add("ListContacts", new HttpRoute("api/client/{clientId}/contact/"));
            HttpRouteValueDictionary httpRouteValueDictionary = new HttpRouteValueDictionary { { "ListContacts", "products" } };

            controller.RequestContext.RouteData = new HttpRouteData(
                route: new HttpRoute(),
                values: httpRouteValueDictionary);

            var response = controller.GetContacts(1) as PageResult<Contact>;
            var resultList = response.results;

            Assert.AreEqual(20, resultList.Count());
            Assert.IsTrue(String.IsNullOrEmpty(response.prevPageLink));
            Assert.IsFalse(String.IsNullOrEmpty(response.nextPageLink));
        }
    }
}
