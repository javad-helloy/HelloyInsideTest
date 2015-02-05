using System.Collections.Generic;
using System.Runtime.Serialization;
using InsideModel.Models;

namespace Inside.HelloyIndex
{
    public interface IContactIndexCalculator
    {
        void SetIndexValues(IEnumerable<ContactCollection> contactBins);
    }

    [DataContract]
    public class ContactCollection
    {
        public ContactCollection(string id, decimal cost, int visitors, IList<Contact> contacts)
        {
            Contacts = new List<Contact>();
            Id = id;
            Cost = cost;
            Contacts = contacts;
            Visitors = visitors;
        }

        public string Id { get; set; }
        public decimal Cost { get; set; }
        public int Visitors { get; set; }
        [DataMember]
        public decimal IndexValue { get; set; }
        public IList<Contact> Contacts { get; set; }
    }
}
