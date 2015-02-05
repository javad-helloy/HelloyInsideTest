using System.Collections.Generic;
using System.Runtime.Serialization;

namespace InsideModel.Models
{
    [DataContract]
    public partial class Tag
    {
        public Tag()
        {
            this.Contacts = new List<Contact>();
        }

        [DataMember]
        public int Id { get; set; }
        
        public int ClientId { get; set; }
        [DataMember]
        public string Name { get; set; }

        public virtual ICollection<Contact> Contacts { get; set; }
        public virtual Client Client { get; set; }
    }
}
