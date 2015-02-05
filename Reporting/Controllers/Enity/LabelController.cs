using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.SessionState;
using InsideModel.Models;
using InsideModel.repositories;
using InsideReporting.Models;

namespace InsideReporting.Controllers.Enity
{
    [Authorize(Roles = "consultant, sales")]
    [SessionState(SessionStateBehavior.ReadOnly)]
    public class LabelController : Controller{
        private readonly IRepository<Label> labelRepository;
        private readonly IRepository<Client> clientRepository;

        public LabelController(
            IRepository<Label> labelRepository, 
            IRepository<Client> clientRepository)
        {
            this.labelRepository = labelRepository;
            this.clientRepository = clientRepository;
        }

        //
        // GET: /Label/
        public ActionResult Index()
        {
            var model = new LabelsViewModel();
            model.Labels = labelRepository.All().ToList().Select(l => new LabelViewModel(l)).ToList();
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(string name)
        {
            var newLabel = new Label();
            newLabel.Name = name;
            labelRepository.Add(newLabel);
            labelRepository.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult Delete(int labelId)
        {
            var labelToDelete = labelRepository.Where(l => l.Id == labelId).First();
            labelToDelete.Clients.Clear();
            labelRepository.Delete(labelToDelete);
            labelRepository.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Set(LeadSetPostValues postvalue)
        {
            if (postvalue.clientIds == null || postvalue.clientIds.Count == 0)
            {
                return Json(new {addedLabels = false});
            }

            var label = labelRepository.Where(l => l.Id == postvalue.labelId).First();
            var clients = clientRepository.Where(c => postvalue.clientIds.Contains(c.Id));

            var clientsWithLabel = clients.Where(c => c.Labels.Any(l => l.Id == postvalue.labelId));

            var allClientsHaveLabel = clients.Count() == clientsWithLabel.Count();

            if (allClientsHaveLabel)
            {
                foreach (var client in clients)
                {
                    client.Labels.Remove(label);
                }
            }
            else
            {
                foreach (var client in clients)
                {
                    if (!client.Labels.Contains(label))
                    {
                        client.Labels.Add(label);    
                    }
                }
            }
            clientRepository.SaveChanges();

            return Json(new {addedLabels = !allClientsHaveLabel});
        }
	}

    public class LeadSetPostValues
    {
        public List<int> clientIds { get; set; }
        public int labelId { get; set; }
    }
}