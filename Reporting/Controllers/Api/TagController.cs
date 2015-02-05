using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Inside.membership;
using InsideModel.Models;
using InsideModel.repositories;
using InsideReporting.Helpers;

namespace InsideReporting.Controllers.Api
{
    public class TagController : ApiAuthenticationController
    {
        private readonly IRepository<Tag> tagRepository;
        private readonly IRepository<Contact> contactRepository;

        public TagController(IIdentityMembershipProvider userManager, 
                             IRepository<Tag> tagRepository, 
                             IRepository<Contact> contactRepository)
            : base(userManager)
        {
            this.tagRepository = tagRepository;
            this.contactRepository = contactRepository;
        }

        [Authorize(Roles = "consultant, client, demo, sales")]
        [AuthorizeClientAPIAccess]
        [Route("api/client/{clientId}/tag")]
        public IHttpActionResult GetTags(int clientId)
        {
            if (User.IsInRole("demo"))
            {
                var demoTags = new List<Tag>();

                demoTags.Add(new Tag
                {
                    Id = 1,
                    Name = "Exempel Etikett"
                });
                return Ok(demoTags);
            }
            
            var matchedTags = tagRepository.Where(t => t.ClientId == clientId).AsNoTracking();
            return Ok(matchedTags);
            
        }

        [HttpDelete]
        [Authorize(Roles = "consultant, client")]
        [AuthorizeClientAPIAccess]
        [Route("api/client/{clientId}/tag/{tagId}/contact/{contactId}")]
        public HttpResponseMessage UnSetTag(int clientId, int contactId, int tagId)
        {
            IQueryable<Contact> matchedContacts = contactRepository.Where(l => l.Id == contactId && l.ClientId == clientId);
            if (!matchedContacts.Any())
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }

            var matchedContact = matchedContacts
                .Include(l => l.Tags)
                .First();

            var hasTag = matchedContact.Tags.Any(t => t.Id == tagId);

            if (hasTag)
            {
                var matchedTag = matchedContact.Tags.First(t => t.Id == tagId);
                matchedContact.Tags.Remove(matchedTag);
            }
            contactRepository.SaveChanges();
            return new HttpResponseMessage(HttpStatusCode.OK);
        }


        [HttpPut]
        [Authorize(Roles = "consultant, client")]
        [AuthorizeClientAPIAccess]
        [Route("api/client/{clientId}/tag/{tagId}/contact/{contactId}")]
        public HttpResponseMessage SetTag(int clientId, int contactId, int tagId)
        {

            IQueryable<Contact> matchedContacts = contactRepository.Where(l => l.Id == contactId && l.ClientId == clientId);
            if (!matchedContacts.Any())
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }

            var matchedContact = matchedContacts
                .Include(l => l.Tags)
                .First();

            
            var tag = tagRepository.Where(t => t.Id == tagId);
            if (tag.Any())
            {
                if (matchedContact.Tags.All(t => t.Id != tag.First().Id))
                {
                    matchedContact.Tags.Add(tag.First());
                }   
            }
            else
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }
            
            contactRepository.SaveChanges();
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        [HttpPut]
        [Authorize(Roles = "consultant, client")]
        [AuthorizeClientAPIAccess]
        [Route("api/client/{clientId}/tag/create")]
        public IHttpActionResult Create(int clientId, string tagName)
        {

            var hasMatchedTags = tagRepository.Any(t => t.Name == tagName && t.ClientId == clientId);
            if (hasMatchedTags)
            {
                return BadRequest();
            }

            var newTag = new Tag
            {
                ClientId = clientId,
                Name = tagName
            };

            tagRepository.Add(newTag);

            tagRepository.SaveChanges();
            return Ok(newTag);
        }

        [HttpDelete]
        [Authorize(Roles = "consultant, client")]
        [AuthorizeClientAPIAccess]
        [Route("api/client/{clientId}/tag/{tagId}/delete")]
        public IHttpActionResult Delete(int clientId, int tagId)
        {

            var hasMatchedTags = tagRepository.Any(t => t.Id == tagId && t.ClientId == clientId);
            if (!hasMatchedTags) return BadRequest();

            var tagToDelete = tagRepository.First(t => t.Id == tagId && t.ClientId == clientId);
            tagRepository.Delete(tagToDelete);
            tagRepository.SaveChanges();
            return Ok(tagToDelete);
        }

    }

    public class TagWithContactCount
    {
        public Tag Tag { get; set; }

        public int ContactCount
        {
            get { return Tag.Contacts.Count(); }
        }
    }

     
}
