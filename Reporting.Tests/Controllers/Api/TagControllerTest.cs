using System.Linq;
using System.Net;
using System.Web.Http.Results;
using Helpers.test;
using Inside.membership;
using InsideModel.Models;
using InsideModel.repositories;
using InsideReporting.Controllers.Api;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace InsideReporting.Tests.Controllers.Api
{
    [TestClass]
    public class TagControllerTest
    {
        [TestMethod]
        public void CanCreate()
        {
            var tagRepository = new Mock<IRepository<Tag>>();
            var userManager = new Mock<IIdentityMembershipProvider>();
            var contactRepository = new Mock<IRepository<Contact>>();

            var controller = new TagController(userManager.Object, tagRepository.Object, contactRepository.Object);
        }


        [TestMethod]
        public void CanGetTags()
        {
            var tagRepository = new LocalRepository<Tag>();
            var userManager = new Mock<IIdentityMembershipProvider>();
            var contactRepository = new Mock<IRepository<Contact>>();

            tagRepository.Add(new Tag
            {
                Id = 1,
                Name = "Tag1",
                ClientId = 1
            });

            tagRepository.Add(new Tag
            {
                Id = 2,
                Name = "Tag2",
                ClientId = 1
            });

            var controller = new TagController(userManager.Object, tagRepository, contactRepository.Object) ;

            var result = controller.GetTags(1) as OkNegotiatedContentResult<IQueryable<Tag>>;

            Assert.AreEqual(2, result.Content.Count());

            Assert.AreEqual("Tag1", result.Content.Single(t=>t.Id==1).Name);

            Assert.AreEqual("Tag2", result.Content.Single(t => t.Id == 2).Name);

            tagRepository.Add(new Tag
            {
                Id = 3,
                Name = "Tag1Client2",
                ClientId = 2
            });

            var result2 = controller.GetTags(2) as OkNegotiatedContentResult<IQueryable<Tag>>;

            Assert.AreEqual(1, result2.Content.Count());

            Assert.AreEqual("Tag1Client2", result2.Content.First().Name);
        }

        [TestMethod]
        public void CanCreateNewTags()
        {
            var tagRepository = new Mock<IRepository<Tag>>();
            var userManager = new Mock<IIdentityMembershipProvider>();
            var contactRepository = new Mock<IRepository<Contact>>();

            var controller = new TagController(userManager.Object, tagRepository.Object, contactRepository.Object);

            var result = controller.Create(1, "NewTag") as OkNegotiatedContentResult<Tag>;

            tagRepository.Verify(tr=>tr.Add(It.IsAny<Tag>()), Times.Once);
            tagRepository.Verify(tr => tr.SaveChanges(), Times.Once);
            Assert.AreEqual("NewTag", result.Content.Name);
        }


        [TestMethod]
        public void CreateExistingTagReturnBAdRequest()
        {
            var tagRepository = new LocalRepository<Tag>();
            var userManager = new Mock<IIdentityMembershipProvider>();
            var contactRepository = new Mock<IRepository<Contact>>();

            var controller = new TagController(userManager.Object, tagRepository, contactRepository.Object);

            tagRepository.Add(new Tag
            {
                Id=1,
                Name = "TagInRepo",
                ClientId = 1
            });
            var result = controller.Create(1, "TagInRepo") ;

            Assert.AreEqual(typeof(BadRequestResult), result.GetType());
            Assert.AreEqual(1, tagRepository.All().Count());
        }

        [TestMethod]
        public void CanDeleteTags()
        {
            var tagRepository = new LocalRepository<Tag>();
            var userManager = new Mock<IIdentityMembershipProvider>();
            var contactRepository = new Mock<IRepository<Contact>>();

            var controller = new TagController(userManager.Object, tagRepository, contactRepository.Object);

            tagRepository.Add(new Tag
            {
                Id = 1,
                Name = "TagInRepo",
                ClientId = 1
            });

            var result = controller.Delete(1,1) as OkNegotiatedContentResult<Tag>;

            Assert.AreEqual(0, tagRepository.All().Count());
            Assert.AreEqual("TagInRepo", result.Content.Name);
        }

        [TestMethod]
        public void DeleteTagWhichIsNotInRepositoryReturnsBadREquest()
        {
            var tagRepository = new Mock<IRepository<Tag>>();
            var userManager = new Mock<IIdentityMembershipProvider>();
            var contactRepository = new Mock<IRepository<Contact>>();

            var controller = new TagController(userManager.Object, tagRepository.Object, contactRepository.Object);

            var result = controller.Delete(1, 1) ;

            Assert.AreEqual(typeof(BadRequestResult), result.GetType());
        }

        [TestMethod]
        public void CanUnsetTagsForContactForClient()
        {
            var tagRepository = new LocalRepository<Tag>();
            var userManager = new Mock<IIdentityMembershipProvider>();
            var contactRepository = new LocalRepository<Contact>();

            var tag1 = new Tag
            {
                Id = 1,
                Name = "Tag1",
                ClientId = 1
            };
            tagRepository.Add(tag1);

            var tag2 = new Tag
            {
                Id = 2,
                Name = "Tag2",
                ClientId = 1
            };
            tagRepository.Add(tag2);

            var contact = new Contact
            {
                Id=1,
                ClientId = 1
                
            };

            contact.Tags.Add(tag1);
            contact.Tags.Add(tag2);
            contactRepository.Add(contact);

            var controller = new TagController(userManager.Object, tagRepository, contactRepository);

            var result = controller.UnSetTag(contact.ClientId, contact.Id, tag1.Id);

            Assert.AreEqual(2, tagRepository.All().Count());

            Assert.AreEqual(1, contactRepository.Single(c => c.Id == 1).Tags.Count());
            Assert.AreEqual("Tag2", contactRepository.Single(c => c.Id == 1).Tags.First().Name);

            var result2 = controller.UnSetTag(contact.ClientId, 123, tag1.Id);
            Assert.AreEqual(HttpStatusCode.NotFound, result2.StatusCode);

        }

        [TestMethod]
        public void CanSetTagsForContactForClientDuplicateIgnored()
        {
            var tagRepository = new LocalRepository<Tag>();
            var userManager = new Mock<IIdentityMembershipProvider>();
            var contactRepository = new LocalRepository<Contact>();

            var tag1 = new Tag
            {
                Id = 1,
                Name = "Tag1",
                ClientId = 1
            };
            tagRepository.Add(tag1);

            var tagToSet = new Tag
            {
                Id = 2,
                Name = "Tag2",
                ClientId = 1
            };
            tagRepository.Add(tagToSet);

            var contact = new Contact
            {
                Id = 1,
                ClientId = 1

            };

            contact.Tags.Add(tag1);
            
            contactRepository.Add(contact);

            var controller = new TagController(userManager.Object, tagRepository, contactRepository);

            var result = controller.SetTag(contact.ClientId, contact.Id, tagToSet.Id);

            Assert.AreEqual(2, tagRepository.All().Count());

            Assert.AreEqual(2, contactRepository.Single(c => c.Id == 1).Tags.Count());
            Assert.AreEqual("Tag1", contactRepository.Single(c => c.Id == 1).Tags.Single(t=>t.Id==1).Name);
            Assert.AreEqual("Tag2", contactRepository.Single(c => c.Id == 1).Tags.Single(t => t.Id == 2).Name);

            var result2 = controller.SetTag(contact.ClientId, contact.Id, tagToSet.Id);
            Assert.AreEqual(2, contactRepository.Single(c => c.Id == 1).Tags.Count());
            Assert.AreEqual("Tag1", contactRepository.Single(c => c.Id == 1).Tags.Single(t => t.Id == 1).Name);
            Assert.AreEqual("Tag2", contactRepository.Single(c => c.Id == 1).Tags.Single(t => t.Id == 2).Name);

        }

        
    }

}
