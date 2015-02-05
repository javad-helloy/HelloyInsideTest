using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Routing;
using Helpers.test;
using Inside.ContactService;
using Inside.ExternalData;
using Inside.HelloyIndex;
using Inside.Time;
using InsideModel.Models;
using InsideModel.repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using RestSharp;

namespace Inside.Test.ContactIndex
{
    [TestClass]
    public class ContactIndexCalculatorTest
    {
        [TestMethod]
        public void CanCreate()
        {
            var calculator = new ContactIndexCalculator();
        }

        [TestMethod]
        public void PremierContactCollectionWithMoreVisitors()
        {
            var calculator = new ContactIndexCalculator();

            var collectionWithMoreContacts = new ContactCollection("more contacts", 1000m, 100, new List<Contact>());
            var collectionWithLessContacts = new ContactCollection("more contacts", 1000m, 10, new List<Contact>());

            var contactCollecions = new List<ContactCollection>();
            contactCollecions.Add(collectionWithMoreContacts);
            contactCollecions.Add(collectionWithLessContacts);

            calculator.SetIndexValues(contactCollecions);

            Assert.IsTrue(collectionWithMoreContacts.IndexValue > collectionWithLessContacts.IndexValue);
        }

        [TestMethod]
        public void PremierContactCollectionWithLowerCost()
        {
            var calculator = new ContactIndexCalculator();

            var collectionWithLowestCost = new ContactCollection("more contacts", 800m, 100, new List<Contact>());
            var collectionWithHigherCost = new ContactCollection("more contacts", 1000m, 100, new List<Contact>());
            var collectionWithHigherCost2 = new ContactCollection("more contacts", 1000m, 100, new List<Contact>());

            // add the same contact to all collections
            collectionWithLowestCost.Contacts.Add(GetContact(null,3));
            collectionWithHigherCost.Contacts.Add(GetContact(null, 3));
            collectionWithHigherCost2.Contacts.Add(GetContact(null, 3));

            collectionWithLowestCost.Contacts.Add(GetContact(4, 3));
            collectionWithHigherCost.Contacts.Add(GetContact(4, 3));
            collectionWithHigherCost2.Contacts.Add(GetContact(4, 3));

            collectionWithLowestCost.Contacts.Add(GetContact(2, 5));
            collectionWithHigherCost.Contacts.Add(GetContact(2, 5));
            collectionWithHigherCost2.Contacts.Add(GetContact(2, 5));


            var contactCollecions = new List<ContactCollection>();
            contactCollecions.Add(collectionWithLowestCost);
            contactCollecions.Add(collectionWithHigherCost);
            contactCollecions.Add(collectionWithHigherCost2);

            calculator.SetIndexValues(contactCollecions);

            Assert.IsTrue(collectionWithLowestCost.IndexValue > collectionWithHigherCost.IndexValue);
            Assert.IsTrue(collectionWithLowestCost.IndexValue > collectionWithHigherCost2.IndexValue);
        }


        [TestMethod]
        public void PremierContactCollectionHighterUserRaiting()
        {
            var calculator = new ContactIndexCalculator();

            var collectionWithHighUserRaitingCost = new ContactCollection("more contacts", 800m, 100, new List<Contact>());
            var collectionWithNotAllUserRaitingCost = new ContactCollection("more contacts", 800m, 100, new List<Contact>());

            collectionWithHighUserRaitingCost.Contacts.Add(GetContact(3, 3));
            collectionWithNotAllUserRaitingCost.Contacts.Add(GetContact(null, 3));

            collectionWithHighUserRaitingCost.Contacts.Add(GetContact(4, 3));
            collectionWithNotAllUserRaitingCost.Contacts.Add(GetContact(null, 3));

            collectionWithHighUserRaitingCost.Contacts.Add(GetContact(1, 1));
            collectionWithNotAllUserRaitingCost.Contacts.Add(GetContact(null, 1));
 

            var contactCollecions = new List<ContactCollection>();
            contactCollecions.Add(collectionWithHighUserRaitingCost);
            contactCollecions.Add(collectionWithNotAllUserRaitingCost);
 
            calculator.SetIndexValues(contactCollecions);

            Assert.IsTrue(collectionWithHighUserRaitingCost.IndexValue > collectionWithNotAllUserRaitingCost.IndexValue);
        }

        [TestMethod]
        public void ContactIndexValuesAreScaled()
        {
            var calculator = new ContactIndexCalculator();

            var collectionWithHighIndexValue = new ContactCollection("more contacts", 1000m, 100, new List<Contact>());
            var collectionWithMiddleIndexValue = new ContactCollection("more contacts", 1500m, 100, new List<Contact>());
            var collectionWithLowIndexValue = new ContactCollection("more contacts", 2000m, 100, new List<Contact>());

            collectionWithHighIndexValue.Contacts.Add(GetContact(4, 3));
            collectionWithHighIndexValue.Contacts.Add(GetContact(4, 2));
            collectionWithHighIndexValue.Contacts.Add(GetContact(null, 5));
            collectionWithHighIndexValue.Contacts.Add(GetContact(3, 3));

            collectionWithMiddleIndexValue.Contacts.Add(GetContact(3, 3));
            collectionWithMiddleIndexValue.Contacts.Add(GetContact(4, 2));
            collectionWithMiddleIndexValue.Contacts.Add(GetContact(1, 4));

            collectionWithLowIndexValue.Contacts.Add(GetContact(1, 3));

            var contactCollecions = new List<ContactCollection>();
            contactCollecions.Add(collectionWithHighIndexValue);
            contactCollecions.Add(collectionWithMiddleIndexValue);
            contactCollecions.Add(collectionWithLowIndexValue);

            calculator.SetIndexValues(contactCollecions);

            Assert.IsTrue(collectionWithHighIndexValue.IndexValue > collectionWithMiddleIndexValue.IndexValue);
            Assert.IsTrue(collectionWithMiddleIndexValue.IndexValue > collectionWithLowIndexValue.IndexValue);

            Assert.IsTrue(collectionWithHighIndexValue.IndexValue >= 0.2m);
            Assert.IsTrue(collectionWithHighIndexValue.IndexValue <= 1.0m);

            Assert.IsTrue(collectionWithLowIndexValue.IndexValue >= 0.2m);
            Assert.IsTrue(collectionWithLowIndexValue.IndexValue <= 1.0m);
        }


        private Contact GetContact(int? raitingScore, int? autoRaitingScore)
        {
            Contact contact = new Contact();

            if (raitingScore.HasValue)
            {
                contact.RatingScore = raitingScore.Value;
            }


            if (autoRaitingScore.HasValue)
            {
                contact.AutoRatingScore = autoRaitingScore.Value;
            }

            return contact;
        }
    }
}
