using System;
using System.Collections.ObjectModel;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using InsideModel.Models;
using InsideModel.Models.Identity;
using Moq;

namespace Helpers.test
{
    public class ModelHelper
    {
        static public void SetClaimAsCurrentUserForController(Claim claim, Controller controller)
        {
            var context = new Mock<HttpContextBase>();
            var fakeIdentity = Mock.Of<ClaimsIdentity>(ci => ci.FindFirst(It.IsAny<string>()) == claim);
            var controllerContext = new Mock<ControllerContext>();

            context.SetupGet(x => x.User.Identity).Returns(fakeIdentity);
            controllerContext.Setup(t => t.HttpContext).Returns(context.Object);
            controller.ControllerContext = controllerContext.Object;
        }
        
        static public Client TestClient1AllDataNoReferences
        {
            get
            {
                return new Client()
                {
                    Address = "Stor vägen 1",
                    AnalyticsTableId = "ga:123456",
                    CallTrackingMetricId = 1,
                    Id = 1,
                    Consultant = null,
                    ConsultantId = "UserIDGuid",
                    Domain = "www.foretaget.se",
                    EmailAddress = "info@foretaget.se",
                    InsideUserSets = null,
                    Latitude = 1.1m,
                    Longitude = 2.2m,
                    Name = "Företaget",
                    IsActive = true,
                };
            }
        }

        static public Client TestClient2AllDataNoReferences
        {
            get
            {
                return new Client()
                {
                    Address = "Lillstigen 1",
                    AnalyticsTableId = "ga:234567",
                    CallTrackingMetricId = 2,
                    Id = 3,
                    Consultant = null,
                    ConsultantId = "UserIDGuid",
                    Domain = "www.foretaget2.se",
                    EmailAddress = "info@foretaget2.se",
                    InsideUserSets = null,
                    Latitude = 2.2m,
                    Longitude = 3.3m,
                    Name = "Företag nummer 2",
                    IsActive = true,
                };
            }
        }

        static public InsideUser TestUser1AllDataNoReferences
        {
            get
            {
                return new InsideUser()
                {
                    ClientId = 1,
                    UserId = 1,
                    Id = Guid.NewGuid().ToString(),
                    ReceiveEmail = true
                };
            }
        }

        static public InsideUser TestUser2AllDataNoReferences
        {
            get
            {
                return new InsideUser()
                {
                    ClientId = 1,
                    UserId = 2,
                    Id = Guid.NewGuid().ToString(),
                    ReceiveEmail = true
                };
            }
        }

        static public Task Task1AllData
        {
            get
            {
                return new Task("", TaskType.ImportSeoData, new DateTime(2013,01,01))
                {
                    Id = 123456,
                    NumTries = 0,
                };
            }
        }

        static public Task NewContactTaskData
        {
            get
            {
                return new Task("", TaskType.CreateTaskForUsersToSendNewContactEmailNotification, new DateTime(2013, 01, 01))
                {
                    Id = 123456,
                    NumTries = 0,
                };
            }
        }

        static public Contact EmailLead
        {
            get
            {
                var emailLead = new Contact();
                emailLead.Date = new DateTime(2013,1,1);

                emailLead.Property.Add(new ContactProperty() { Type = "MandrillId", Value = "1" });
                emailLead.Property.Add(new ContactProperty() { Type = "FromEmail", Value = "testFrom@helloy.com" });
                emailLead.Property.Add(new ContactProperty() { Type = "ToEmail", Value = "info@foretaget.se" });
                emailLead.Property.Add(new ContactProperty() { Type = "Text", Value = "Sample Text" });
                emailLead.Property.Add(new ContactProperty() { Type = "Html", Value = "<p>Sample Text</p>" });

                return emailLead;
            }
        }

        static public InsideRole TestSalesRole
        {
            get
            {
                return new InsideRole()
                {
                    Id = "Guid1",
                    Name = "sales",
                };
                
            }
        }
        static public InsideRole TestConsultantRole
        {
            get
            {
                return new InsideRole()
                {
                    Id = "Guid2",
                    Name = "consultant",
                };
            }
        }

        static public InsideRole TestClientRole
        {
            get
            {
                return new InsideRole()
                {
                    Id = "Guid3",
                    Name = "client",
                };
            }
        }

        static public InsideUser TestAccountManager
        {
            get
            {
                var user =  new InsideUser()
                {
                    Id = "Guid1",
                    Name = "Test AM",
                    Email = "Test@test.com",
                    
                };
                user.Role.Add(TestSalesRole);
                return user;
            }
        }

        static public InsideUser TestConsultant
        {
            get
            {
                var user = new InsideUser()
                {
                    Id = "Guid2",
                    Name = "Test MC",
                    Email = "Test@test.com"
                };
                user.Role.Add(TestConsultantRole);
                return user;
            }
        }

        public static void SetCurrentUserToRole(string role, Controller controller)
        {
            Mock<ControllerContext> controllerContextMock = new Mock<ControllerContext>();
            controllerContextMock.Setup(
                x => x.HttpContext.User.IsInRole(It.Is<string>(s => s.Equals(role)))
                ).Returns(true);

            controller.ControllerContext = controllerContextMock.Object;
        }

    }
}
