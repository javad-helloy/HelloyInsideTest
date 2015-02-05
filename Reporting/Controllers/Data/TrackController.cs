using System;
using System.Dynamic;
using System.Linq;
using System.Web.Mvc;
using Inside.AutoRating;
using Inside.Time;
using InsideModel.Models;
using InsideModel.repositories;
using Newtonsoft.Json;

namespace InsideReporting.Controllers.Data
{
    public class TrackController : Controller
    {
        
        private readonly IRepository<Contact> contactRepository;
        private readonly IServerTime serverTime;
        private readonly IContactAutoRating contactAutoRating;

        public TrackController(
                IRepository<Contact> contactRepository,
                IServerTime serverTime,
                IContactAutoRating contactAutoRating)
        {
            this.contactRepository = contactRepository;
            this.serverTime = serverTime;
            this.contactAutoRating = contactAutoRating;
        }

        public ActionResult Index()
        {
            if (Request.Params.GetValues("id")==null)
            {
                throw new Exception("Could not retrieve FormId");
            }

            var queryData = Request.Params.GetValues("data").FirstOrDefault();
            var id = Request.Params.GetValues("id").FirstOrDefault();
            
            var clientId = int.Parse(id.Split('-')[0]);
            AddFormContact(queryData, id, clientId);
            var dir = Server.MapPath("/Content/collect.gif");
            return base.File(dir, "image/gif"); 
        }

        private void AddFormContact(string formData, string dateId, int clientId)
        {
            var serverRequestStartDate =serverTime.Now;
            var hasContactInDb =
                contactRepository.Where(
                    l =>
                        l.LeadType == "Email" && l.ClientId == clientId &&
                        l.Date == serverRequestStartDate &&
                        l.Property.Any(lp => (lp.Type == "FormData" && lp.Value == formData)) &&
                        l.Property.Any(lp => (lp.Type == "FormPostId" && lp.Value == dateId))).Any();
            if (hasContactInDb)
            {
                return;
            }

            var eventContact = new Contact();
            eventContact.LeadType = "Email";
            eventContact.ClientId = clientId;
            eventContact.Date = serverRequestStartDate;
            
            contactAutoRating.SetAutoRating(eventContact);
            eventContact.AddProperty("FormData", formData);
            eventContact.AddProperty("FormPostId", dateId);

            MapFormDataToContact(eventContact, formData);
            contactRepository.Add(eventContact);
            contactRepository.SaveChanges();
        }

        private void MapFormDataToContact(Contact eventContact, string formData)
        {
            var extractedFormData = JsonConvert.DeserializeObject<ExpandoObject>(formData);
            
            foreach (var kvp in extractedFormData)
            {
                if (string.IsNullOrEmpty(kvp.Key) || string.IsNullOrEmpty(kvp.Value.ToString()))
                {
                    continue;
                }
                if (ContactFromType.Phone.Contains(kvp.Key.ToLower()))
                {
                    eventContact.AddProperty("FromPhone", kvp.Value.ToString());
                }
                else if (ContactFromType.Email.Contains(kvp.Key.ToLower()))
                {
                    eventContact.AddProperty("FromEmail", kvp.Value.ToString());
                }
            }
        }
    }
}