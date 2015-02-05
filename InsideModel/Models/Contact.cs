using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace InsideModel.Models
{
    [ExcludeFromCodeCoverage]
    [DataContract]
    public partial class Contact
    {
        public Contact()
        {
            this.Property = new List<ContactProperty>();
            this.Interaction = new List<ContactInteraction>();
            this.Tags = new List<Tag>();
        }

        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public int ClientId { get; set; }
        [DataMember]
        public DateTime Date { get; set; }
        [DataMember]
        public string LeadType { get; set; }
        [DataMember]
        public int? AutoRatingScore { get; set; }
        [DataMember]
        public int? RatingScore { get; set; }
        [DataMember]
        public string Campaign { get; set; }
        [DataMember]
        public string Source { get; set; }
        [DataMember]
        public string Medium { get; set; }
        [DataMember]
        public string Product { get; set; }
        [DataMember]
        public string SearchPhrase { get; set; }
        [DataMember]
        public string ReadStatus{ get; set; }

        public virtual Client Client { get; set; }

        [DataMember]
        [JsonConverter(typeof(LeadPropertyCollectionConverter))]
        public virtual ICollection<ContactProperty> Property { get; set; }

        [DataMember]
        [JsonConverter(typeof(LeadInteractionCollectionConverter))]
        public virtual ICollection<ContactInteraction> Interaction { get; set; }

        [DataMember]
        public virtual ICollection<Tag> Tags { get; set; }

        public ContactProperty GetProperty(string property)
        {
            return Property.Single(lp => lp.Type == property);
        }

        public bool HasProperty(string property)
        {
            return (Property.Any(lp => lp.Type == property));
        }

        public void AddProperty(string property, string value)
        {
            Property.Add(new ContactProperty()
            {
                Type = property,
                Value = value,
            });
        }

        public void SetPropertyValue(string propertyName, string propertyValue)
        {
            if (propertyValue == null)
            {
                throw new ArgumentNullException();
            }
            
            if (Property.Any(lp => lp.Type == propertyName))
            {
                Property.First(lp => lp.Type == propertyName).Value = propertyValue;
            }
            else
            {
                Property.Add(new ContactProperty() { Type = propertyName, Value = propertyValue });
            }
        }
    }

    public class LeadPropertyCollectionConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            JObject  propertyObject = new JObject();
            
            var leadPropertyCollection = value as ICollection<ContactProperty>;

            var addedTypes = new List<String>();
            foreach (var leadProperty in leadPropertyCollection)
            {
                if (addedTypes.Contains(leadProperty.Type))
                {
                    continue;
                }
                else
                {
                    addedTypes.Add(leadProperty.Type);
                    propertyObject.Add(leadProperty.Type, new JValue(leadProperty.Value));
                }
            }

            propertyObject.WriteTo(writer);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            throw new NotImplementedException();
        }
    }

    public class LeadInteractionCollectionConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            JObject propertyObject = new JObject();

            var leadInteractionCollection = value as ICollection<ContactInteraction>;

            var addedTypes = new List<String>();
            foreach (var leadInteraction in leadInteractionCollection)
            {
                if (addedTypes.Contains(leadInteraction.Type))
                {
                    continue;
                }
                else
                {
                    addedTypes.Add(leadInteraction.Type);
                    propertyObject.Add(leadInteraction.Type, new JValue(leadInteraction.Value));    
                }
            }


            propertyObject.WriteTo(writer);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            throw new NotImplementedException();
        }
    }
}
