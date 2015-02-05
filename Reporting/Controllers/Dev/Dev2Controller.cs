using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InsideModel.Models;
using InsideModel.repositories;
using InsideReporting.Models;
using Newtonsoft.Json;

namespace InsideReporting.Controllers.Dev
{
    public class Dev2Controller : Controller
    {
        private readonly IRepository<Label> labelRepository;
        public Dev2Controller(IRepository<Label> labelRepository)
        {
            this.labelRepository = labelRepository;
        }
        // GET: Dev2
        public ActionResult Index()
        {
            return Content(JsonConvert.SerializeObject(labelRepository.All().ToList().Select(l => new LabelViewModel(l)).ToList()), "application/json");
        }

    }
}