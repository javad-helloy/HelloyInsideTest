using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace InsideModel.Models
{
    [ExcludeFromCodeCoverage]
    [DataContract]
    public partial class ContactProperty
    {
        public ContactProperty(){}

        public ContactProperty(string type, string value)
        {
            this.Type = type;
            this.Value = value;
        }

        public int Id { get; set; }
        public int ContactId	{ get; set; }
        
        [DataMember]
        public string Type	{ get; set; }
        
        [DataMember]
        public string Value { get; set; }

        public virtual Contact Contact { get; set; }
    }
}
