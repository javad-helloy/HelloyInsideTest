using System;
using System.Linq;
using Inside.AutoRating;
using InsideModel.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Inside.Test.AutoRating
{
    [TestClass]
    public class ContactAutoRatingTest
    {
        [TestMethod]
        public void CanCreate()
        {
            var autoRating = new ContactAutoRating();
        }

        [TestMethod]
        public void CanSetPhoneCallWithCorrectRating()
        {
            var autoRating = new ContactAutoRating();

            var contactToAutorate = new Contact
            {
                Id = 1,
                LeadType = "Phone",
            };
            
            contactToAutorate.Property.Add(new ContactProperty("Status", "completed"));
            autoRating.SetAutoRating(contactToAutorate);
            Assert.AreEqual(2, contactToAutorate.AutoRatingScore);
            
            contactToAutorate.SetPropertyValue("Duration", "10");
            autoRating.SetAutoRating(contactToAutorate);
            Assert.AreEqual(1, contactToAutorate.AutoRatingScore);
            
            contactToAutorate.SetPropertyValue("Duration", "30");
            autoRating.SetAutoRating(contactToAutorate);
            Assert.AreEqual(2, contactToAutorate.AutoRatingScore);
            
            contactToAutorate.SetPropertyValue("Duration", "90");
            autoRating.SetAutoRating(contactToAutorate);
            Assert.AreEqual(3, contactToAutorate.AutoRatingScore);
            
            contactToAutorate.SetPropertyValue("Duration", "180");
            autoRating.SetAutoRating(contactToAutorate);
            Assert.AreEqual(4, contactToAutorate.AutoRatingScore);
            
            contactToAutorate.SetPropertyValue("Duration", "300");
            autoRating.SetAutoRating(contactToAutorate);
            Assert.AreEqual(5, contactToAutorate.AutoRatingScore);
            
            contactToAutorate.SetPropertyValue("Duration", "500");
            autoRating.SetAutoRating(contactToAutorate);
            Assert.AreEqual(5, contactToAutorate.AutoRatingScore);
         }

        [TestMethod]
        public void PhoneCallWithStatusInvalidSetDefaultRating()
        {
            var autoRating = new ContactAutoRating();

            var contactToAutorate = new Contact
            {
                Id = 1,
                LeadType = "Phone",
            };

            autoRating.SetAutoRating(contactToAutorate);
            Assert.AreEqual(2, contactToAutorate.AutoRatingScore);
            
            contactToAutorate.SetPropertyValue("Status", "busy");
            autoRating.SetAutoRating(contactToAutorate);
            Assert.AreEqual(1, contactToAutorate.AutoRatingScore);
            
        }

        [TestMethod]
        public void CanSetEmailCallWithCorrectRating()
        {
            var autoRating = new ContactAutoRating();

            var contactToAutorate = new Contact
            {
                Id = 1,
                LeadType = "Email",
            };

            autoRating.SetAutoRating(contactToAutorate);
            Assert.AreEqual(3, contactToAutorate.AutoRatingScore);
        }

        [TestMethod]
        public void CanSetChatCallWithCorrectRating()
        {
            var autoRating = new ContactAutoRating();

            var contactToAutorate = new Contact
            {
                Id = 1,
                LeadType = "Chat",
            };

            autoRating.SetAutoRating(contactToAutorate);
            Assert.AreEqual(5, contactToAutorate.AutoRatingScore);
        }

        
    }
}
