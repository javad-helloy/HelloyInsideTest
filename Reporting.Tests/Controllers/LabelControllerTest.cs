using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Helpers.test;
using InsideModel.Models;
using InsideReporting.Controllers.Enity;
using InsideReporting.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InsideReporting.Tests.Controllers
{
    [TestClass]
    public class LabelControllerTest
    {
        [TestMethod]
        public void CanCreate()
        {
            var labelRepository = new LocalRepository<Label>();
            var clientRepository = new LocalRepository<Client>();

            var controller = new LabelController(labelRepository, clientRepository);
        }

        [TestMethod]
        public void CanGetEmptyListOfLabels()
        {
            var labelRepository = new LocalRepository<Label>();
            var clientRepository = new LocalRepository<Client>();
            var controller = new LabelController(labelRepository, clientRepository);
            var result = controller.Index() as ViewResult;
            var resultModel = result.Model as LabelsViewModel;

            Assert.AreEqual(0, resultModel.Labels.Count);
        }

        [TestMethod]
        public void CanGetListOfLabels()
        {
            var labelRepository = new LocalRepository<Label>();
            labelRepository.Add(new Label(){Id = 1, Name = "Label 1"});
            labelRepository.Add(new Label() { Id = 2, Name = "Label 2" });
            labelRepository.Add(new Label() { Id = 3, Name = "Label 3" });

            var clientRepository = new LocalRepository<Client>();
            var controller = new LabelController(labelRepository, clientRepository);
            var result = controller.Index() as ViewResult;
            var resultModel = result.Model as LabelsViewModel;

            Assert.AreEqual(3, resultModel.Labels.Count);
            Assert.IsTrue(resultModel.Labels.Any(l => l.Id == 1 && l.Name == "Label 1"));
            Assert.IsTrue(resultModel.Labels.Any(l => l.Id == 2 && l.Name == "Label 2"));
            Assert.IsTrue(resultModel.Labels.Any(l => l.Id == 3 && l.Name == "Label 3"));
        }

        [TestMethod]
        public void CanCreateLabel()
        {
            var labelRepository = new LocalRepository<Label>();
            var clientRepository = new LocalRepository<Client>();
            var controller = new LabelController(labelRepository, clientRepository);

            Assert.AreEqual(0, labelRepository.All().Count());

            controller.Create("New Label");
            
            Assert.AreEqual(1, labelRepository.All().Count());
            Assert.IsTrue(labelRepository.All().Any(l => l.Name == "New Label"));
        }

        [TestMethod]
        public void CanDeleteLabel()
        {
            var labelRepository = new LocalRepository<Label>();
            var clientRepository = new LocalRepository<Client>();
            var controller = new LabelController(labelRepository, clientRepository);

            labelRepository.Add(new Label(){Id = 1, Name = "Label 1"});
            labelRepository.Add(new Label() { Id = 2, Name = "Label 2" });
            labelRepository.Add(new Label() { Id = 3, Name = "Label 3" });

            Assert.AreEqual(3, labelRepository.All().Count());
            
            controller.Delete(2);

            Assert.AreEqual(2, labelRepository.All().Count());
            Assert.IsTrue(labelRepository.All().Any(l => l.Id == 1 && l.Name == "Label 1"));
            Assert.IsTrue(labelRepository.All().Any(l => l.Id == 3 && l.Name == "Label 3"));
        }

        [TestMethod]
        public void CanAddLabelToClients()
        {
            var labelRepository = new LocalRepository<Label>();
            labelRepository.Add(new Label() { Id = 1, Name = "Label 1" });
            Label label2 = new Label() { Id = 2, Name = "Label 2" };
            labelRepository.Add(label2);
            labelRepository.Add(new Label() { Id = 3, Name = "Label 3" });

            var clientRepository = new LocalRepository<Client>();

            Client client1 = ModelHelper.TestClient1AllDataNoReferences;
            client1.Labels.Add(label2);
            Client client2 = ModelHelper.TestClient2AllDataNoReferences;
            clientRepository.Add(client1);
            clientRepository.Add(client2);

            var controller = new LabelController(labelRepository, clientRepository);

            LeadSetPostValues leadSetPostValues = new LeadSetPostValues();

            leadSetPostValues.clientIds = new List<int>() { client1.Id, client2.Id};
            leadSetPostValues.labelId = 2;

            controller.Set(leadSetPostValues);

            Assert.IsTrue(client1.Labels.Any(l => l.Name == "Label 2"));
            Assert.IsTrue(client2.Labels.Any(l => l.Name == "Label 2"));
        }

        [TestMethod]
        public void CanRemoveLabelFromClients()
        {
            var labelRepository = new LocalRepository<Label>();
            labelRepository.Add(new Label() { Id = 1, Name = "Label 1" });
            Label label2 = new Label() { Id = 2, Name = "Label 2" };
            labelRepository.Add(label2);
            labelRepository.Add(new Label() { Id = 3, Name = "Label 3" });

            var clientRepository = new LocalRepository<Client>();

            Client client1 = ModelHelper.TestClient1AllDataNoReferences;
            client1.Labels.Add(label2);

            Client client2 = ModelHelper.TestClient2AllDataNoReferences;
            client2.Labels.Add(label2);

            clientRepository.Add(client1);
            clientRepository.Add(client2);

            var controller = new LabelController(labelRepository, clientRepository);

            LeadSetPostValues leadSetPostValues = new LeadSetPostValues();

            leadSetPostValues.clientIds = new List<int>() { client1.Id, client2.Id };
            leadSetPostValues.labelId = 2;

            controller.Set(leadSetPostValues);

            Assert.IsFalse(client1.Labels.Any(l => l.Name == "Label 2"));
            Assert.IsFalse(client2.Labels.Any(l => l.Name == "Label 2"));
        }

    }
}
