using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace InsideModel.Models
{
    [DataContract]
    public partial class ContactInteraction
    {
        public int Id { get; set; }
        public int ContactId	{ get; set; }
        
        [DataMember]
        [Required]
        public string Type { get; set; }

        [DataMember]
        public string Value { get; set; }
        
        public virtual Contact Contact { get; set; }
    }
}
