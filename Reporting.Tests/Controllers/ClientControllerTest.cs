using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Claims;
using System.Web.Mvc;
using Helpers.test;
using Inside.membership;
using InsideModel.Models;
using InsideModel.Models.Identity;
using InsideModel.repositories;
using InsideReporting.Controllers.Enity;
using InsideReporting.Models;
using InsideReporting.Models.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace InsideReporting.Tests.Controllers
{
    /// <summary>
    /// Summary description for PhoneStatisticsTest
    /// </summary>
    [TestClass]
    public class ClientControllerTest
    {
        [TestMethod]
        public void CanCreate()
        {
            var clientRepositoryMoq = new Mock<IRepository<Client>>();
            var consultantRepositoryMoq = new Mock<IRepository<InsideUser>>();
            var userManager = new Mock<IIdentityMembershipProvider>();
            var labelRepositoryMoq = new Mock<IRepository<Label>>();

            var controllerUnderTest = new ClientController(clientRepositoryMoq.Object, labelRepositoryMoq.Object, consultantRepositoryMoq.Object, userManager.Object);
        }

        [TestMethod]
        public void ShowOnlyActiveClients()
        {
            var clientRepository = new LocalRepository<Client>();
            var clientInRepository = ModelHelper.TestClient1AllDataNoReferences;
            var activeClientInDb = ModelHelper.TestClient1AllDataNoReferences;

            var consultantMembershipProviderId = Guid.NewGuid().ToString();
            var userManager = new Mock<IIdentityMembershipProvider>();
            var consultantRepository = new LocalRepository<InsideUser>();
            var labelRepository = new LocalRepository<Label>();
            var consultant = new InsideUser()
            {
                Id = consultantMembershipProviderId,
                Role = new Collection<InsideRole>{ModelHelper.TestConsultantRole}
            };
            consultantRepository.Add(consultant);

            clientInRepository.IsActive = false;
            clientInRepository.Consultant = consultant;
            activeClientInDb.Consultant = consultant;
            clientRepository.Add(clientInRepository);
            clientRepository.Add(activeClientInDb);
            
            
            userManager.Setup(um => um.GetRoles(consultant.Id)).Returns(new List<string> { "mockRole" });

            var clientController = new ClientController(clientRepository, labelRepository, consultantRepository, userManager.Object);
            var claim = new Claim("test", consultant.Id);
            ModelHelper.SetClaimAsCurrentUserForController(claim, clientController);

            var result = clientController.Index(true, null) as ViewResult;
            var clients = result.Model as ClientsViewModel;

            Assert.AreEqual(1, clients.Clients.Count());
            Assert.IsTrue(clients.Clients.First().IsActive);
        }

        [TestMethod]
        public void ShowOnlyInActiveClients()
        {
            var clientRepository = new LocalRepository<Client>();
            var clientInRepository = ModelHelper.TestClient1AllDataNoReferences;
            var activeClientInDb = ModelHelper.TestClient1AllDataNoReferences;

            var consultantMembershipProviderId = Guid.NewGuid().ToString();
            var userManager = new Mock<IIdentityMembershipProvider>();
            var consultantRepository = new LocalRepository<InsideUser>();
            var labelRepository = new LocalRepository<Label>();
            var consultant = new InsideUser()
            {
                Id = consultantMembershipProviderId,
                Role = new Collection<InsideRole> { ModelHelper.TestConsultantRole }
            };
            consultantRepository.Add(consultant);

            clientInRepository.IsActive = false;
            clientInRepository.Consultant = consultant;

            activeClientInDb.Consultant = consultant;
            clientRepository.Add(clientInRepository);
            clientRepository.Add(activeClientInDb);


            userManager.Setup(um => um.GetRoles(consultant.Id)).Returns(new List<string> { "mockRole" });

            var clientController = new ClientController(clientRepository, labelRepository, consultantRepository, userManager.Object);
            var claim = new Claim("test", consultant.Id);
            ModelHelper.SetClaimAsCurrentUserForController(claim, clientController);

            var result = clientController.Index(false, true) as ViewResult;
            var clients = result.Model as ClientsViewModel;

            Assert.AreEqual(1, clients.Clients.Count());
        }

        [TestMethod]
        public void IsAddingLabelsToModel()
        {
            
            var labelRepository = new LocalRepository<Label>();
            labelRepository.Add(new Label() { Id = 1, Name = "Label 1" });
            labelRepository.Add(new Label() { Id = 2, Name = "Label 2" });

            var clientRepository = new LocalRepository<Client>();
            var clientInRepository = ModelHelper.TestClient1AllDataNoReferences;
            var consultantMembershipProviderId = Guid.NewGuid().ToString();
            var userManager = new Mock<IIdentityMembershipProvider>();
            var consultantRepository = new LocalRepository<InsideUser>();
            var consultant = new InsideUser()
            {
                Id = consultantMembershipProviderId,
                Role = new Collection<InsideRole>{ModelHelper.TestConsultantRole}
            };
            consultantRepository.Add(consultant);

            clientInRepository.IsActive = false;
            clientInRepository.Consultant = consultant;
            clientRepository.Add(clientInRepository);

            
           
            userManager.Setup(um => um.GetRoles(consultant.Id)).Returns(new List<string> { "mockRole" });

            var clientController = new ClientController(clientRepository, labelRepository, consultantRepository, userManager.Object);
            var claim = new Claim("test", consultant.Id);
            ModelHelper.SetClaimAsCurrentUserForController(claim, clientController);
            
            var result = clientController.Index(true, null) as ViewResult;
            var resultModel = result.Model as ClientsViewModel;

            Assert.AreEqual(2, resultModel.Labels.Count());
            Assert.IsTrue(resultModel.Labels.Any(l => l.Name == "Label 1" && l.Id == 1));
            Assert.IsTrue(resultModel.Labels.Any(l => l.Name == "Label 2" && l.Id == 2));
        }


        [TestMethod]
        public void ShowOnlyActiveClientsOfCertainConsultant()
        {
            var clientRepository = new LocalRepository<Client>();
            var clientInRepository1 = ModelHelper.TestClient1AllDataNoReferences;
            var clientInRepository2 = ModelHelper.TestClient2AllDataNoReferences;
            var consultantMembershipProviderId1 = Guid.NewGuid().ToString();
            var consultantMembershipProviderId2 = Guid.NewGuid().ToString();
            var userManager = new Mock<IIdentityMembershipProvider>();
            var consultantRepository = new LocalRepository<InsideUser>();
            var labelRepository = new LocalRepository<Label>();
            var consultant1 = new InsideUser()
                {
                    Id = consultantMembershipProviderId1,
                    Role = new Collection<InsideRole>{ModelHelper.TestConsultantRole}
                };

            clientInRepository1.IsActive = true;
            clientInRepository1.ConsultantId = consultant1.Id;
            clientInRepository1.Consultant = consultant1;

            consultantRepository.Add(consultant1);

            clientRepository.Add(clientInRepository1);

            var consultant2 = new InsideUser()
                {
                     Id = consultantMembershipProviderId2
                };
            consultantRepository.Add(consultant2);
            clientInRepository2.IsActive = true;
            clientInRepository2.ConsultantId = consultant2.Id;
            clientInRepository2.Consultant = consultant2;
            clientRepository.Add(clientInRepository2);

           
            
            userManager.Setup(um => um.GetRoles(consultant1.Id)).Returns(new List<string> { "consultant" });

            var clientController = new ClientController(clientRepository, labelRepository, consultantRepository, userManager.Object);
            var claim = new Claim("test", consultant1.Id);
            ModelHelper.SetClaimAsCurrentUserForController(claim, clientController);

            var result = clientController.Index(false, null) as ViewResult;
            var clients = result.Model as ClientsViewModel;

            Assert.AreEqual(1, clients.Clients.Count());

        }

        [TestMethod]
        public void InactiveClientNotShownAsDefault()
        {
            var clientRepository = new LocalRepository<Client>();
            var clientInRepository = ModelHelper.TestClient1AllDataNoReferences;
            var consultantMembershipProviderId = Guid.NewGuid().ToString();
            var userManager = new Mock<IIdentityMembershipProvider>();
            var consultantRepository = new LocalRepository<InsideUser>();
            var labelRepository = new LocalRepository<Label>();
            var consultant = new InsideUser()
                {
                    Id = consultantMembershipProviderId,
                    Role = new Collection<InsideRole>{ModelHelper.TestConsultantRole}
                };
            consultantRepository.Add(consultant);

            clientInRepository.IsActive = false;
            clientInRepository.Consultant = consultant;
            clientRepository.Add(clientInRepository);

            
            
            userManager.Setup(um => um.GetRoles(consultant.Id)).Returns(new List<string> { "mockRole" });
            
            var clientController = new ClientController(clientRepository, labelRepository, consultantRepository, userManager.Object);
            var claim = new Claim("test", consultant.Id);
            ModelHelper.SetClaimAsCurrentUserForController(claim, clientController);

            var result = clientController.Index(null, null) as ViewResult;
            var clients = result.Model as ClientsViewModel;

            Assert.AreEqual(0, clients.Clients.Count());

        }
        
        [TestMethod]
        public void CreateReturnsEmptyView()
        {
            var clientRepositoryMoq = new Mock<IRepository<Client>>();

            var consultantRepositoryMoq = new Mock<IRepository<InsideUser>>();
            var userManager = new Mock<IIdentityMembershipProvider>();
            var labelRepository = new LocalRepository<Label>();

            
            
            userManager.Setup(um => um.GetRoles("UserId")).Returns(new List<string> { "mockRole" });

            var clientController = new ClientController(clientRepositoryMoq.Object, labelRepository, consultantRepositoryMoq.Object, userManager.Object);
            var claim = new Claim("test", "UserId");
            ModelHelper.SetClaimAsCurrentUserForController(claim, clientController);
           
            var result = clientController.Create() as ViewResult;
            var resultModel = (ClientPageLoggedViewModel)result.Model;
            Assert.IsTrue(resultModel.ClientViewModel.Id == 0, "Expected empty result");
        }


        [TestMethod]
        public void CanCreateNewClientWithRespectedConsultant()
        {
            var clientRepositoryMoq = new Mock<IRepository<Client>>();
            var consultantRepository = new LocalRepository<InsideUser>();
            var userManager = new Mock<IIdentityMembershipProvider>();
            var labelRepository = new LocalRepository<Label>();
            var consultant = new InsideUser() { Id = "Id1" };
            consultantRepository.Add(consultant);

            var clientController = new ClientController(clientRepositoryMoq.Object, labelRepository, consultantRepository, userManager.Object);
            var clientToCreate = ModelHelper.TestClient1AllDataNoReferences;
            
            clientToCreate.ConsultantId = "Id1";
            clientToCreate.Consultant = consultant;

            var clientViewModel = new ClientViewModel(clientToCreate);
            var clilientPageViewModel = new ClientPageLoggedViewModel(new List<string>(), clientViewModel);
            var result = clientController.Create(clilientPageViewModel);

            Assert.IsTrue(clientController.ViewData.ModelState.Count == 0, "Excpected no validation errors.");
           
            clientRepositoryMoq.Verify(r => r.Add(It.IsAny<Client>()),"Expected client add");
            clientRepositoryMoq.Verify(r => r.SaveChanges(), "Expected call to save changes");

            Assert.IsInstanceOfType(result, typeof (RedirectToRouteResult),"Expected redirection reslut");
        }

        [TestMethod]
        public void InvalidModelStateWhenCreatingClientDoesntCreateClient()
        {
            var clientRepositoryMoq = new Mock<IRepository<Client>>();
            var consultantRepository = new LocalRepository<InsideUser>();
            var userManager = new Mock<IIdentityMembershipProvider>();
            var labelRepository = new LocalRepository<Label>();

            var consultant = new InsideUser() { Id = "Id1" };
            //consultantRepositoryMoq.Setup(m => m.All()).Returns(consultants.AsQueryable);
            consultantRepository.Add(consultant);
            var clientController = new ClientController(clientRepositoryMoq.Object, labelRepository, consultantRepository, userManager.Object);
            
            var clientToCreate = ModelHelper.TestClient1AllDataNoReferences;
            clientToCreate.ConsultantId = "Id1";
            clientToCreate.Consultant = consultant;
            clientToCreate.Consultant.Role = new Collection<InsideRole>{ModelHelper.TestConsultantRole};
            var clientViewModel = new ClientViewModel(clientToCreate);
            var clilientPageViewModel = new ClientPageLoggedViewModel(new List<string>(), clientViewModel);
            clientController.ModelState.AddModelError("key", "error message");
            var result = clientController.Create(clilientPageViewModel) as ViewResult;

           // var consultants = result.Consultant as SelectList;

            Assert.AreEqual(1 + 1, clilientPageViewModel.Consultant.Count(), "+1 is for not to count Null consultant"); 
            clientRepositoryMoq.Verify(r => r.Add(It.IsAny<Client>()),Times.Never());
        }

        [TestMethod]
        public void InvalidModelStateWhenEditingClientDoesntEditClients()
        {
            var clientRepositoryMoq = new Mock<IRepository<Client>>();
            var consultantRepository = new LocalRepository<InsideUser>();
            var userManager = new Mock<IIdentityMembershipProvider>();
            var labelRepository = new LocalRepository<Label>();

            var consultant = new InsideUser() { Id = "Id1" };
            //consultantRepositoryMoq.Setup(m => m.All()).Returns(consultants.AsQueryable);
            consultantRepository.Add(consultant);
            var clientController = new ClientController(clientRepositoryMoq.Object, labelRepository, consultantRepository, userManager.Object);

            var clientToEdit = ModelHelper.TestClient1AllDataNoReferences;
            clientToEdit.ConsultantId = "Id1";
            clientToEdit.Consultant = consultant;
            clientToEdit.Consultant.Role = new Collection<InsideRole>{ModelHelper.TestConsultantRole};
            var clientViewModel = new ClientViewModel(clientToEdit);
            var clilientPageViewModel = new ClientPageViewModel(new List<string>(), clientViewModel);
            clientController.ModelState.AddModelError("key", "error message");
            var result = clientController.Edit(clilientPageViewModel) as ViewResult;

            //var consultants = result.ViewBag.Consultant as SelectList;

            Assert.AreEqual(1 + 1, clilientPageViewModel.Consultant.Count(), "+1 is for not to count Null consultant"); 
            clientRepositoryMoq.Verify(r => r.Attach(It.IsAny<Client>()), Times.Never());
        }

        [TestMethod]
        public void EditClientGetReturnsView()
        {
            var clientRepository = new LocalRepository<Client>();
            var clientInRepository = ModelHelper.TestClient1AllDataNoReferences;
            var userManager = new Mock<IIdentityMembershipProvider>();
            var labelRepository = new LocalRepository<Label>();

            var clientID = 1;
            clientInRepository.Id = clientID;

            clientRepository.Add(clientInRepository);

            var consultantRepositoryMoq = new Mock<IRepository<InsideUser>>();

            
            userManager.Setup(um => um.GetRoles(It.IsAny<string>())).Returns(new List<string> { "mockRole" });

            var clientController = new ClientController(clientRepository, labelRepository, consultantRepositoryMoq.Object, userManager.Object);
            var claim = new Claim("test", "AnyId");
            ModelHelper.SetClaimAsCurrentUserForController(claim, clientController);

            var result = clientController.Edit(clientID) as ViewResult;
            var resultModel = result.Model as ClientPageViewModel;

            Assert.AreEqual(clientID, resultModel.ClientViewModel.Id, "Expected edit view of model.");
        }

        [TestMethod]
        public void EditClientSavesForOkData()
        {
            var clientRepositoryMoq = new Mock<IRepository<Client>>();
            var consultantRepositoryMoq = new Mock<IRepository<InsideUser>>();
            var userManager = new Mock<IIdentityMembershipProvider>();
            var labelRepository = new LocalRepository<Label>();

            var clientController = new ClientController(clientRepositoryMoq.Object, labelRepository, consultantRepositoryMoq.Object, userManager.Object);

            var clientToSave = ModelHelper.TestClient1AllDataNoReferences;
            var clientViewModel = new ClientViewModel(clientToSave);
            var clilientPageViewModel = new ClientPageViewModel(new List<string>(), clientViewModel);
            var result = clientController.Edit(clilientPageViewModel);


            Assert.IsTrue(clientController.ViewData.ModelState.Count == 0, "Expected no validation errors.");
            clientRepositoryMoq.Verify(r => r.Attach(It.Is<Client>(c=>c.Id==clientToSave.Id)), "Expected client attach");
            clientRepositoryMoq.Verify(r => r.SaveChanges(), "Expected call to save changes");

            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult), "Expected redirect after save.");
        }
    }
}
