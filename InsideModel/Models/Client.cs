using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace InsideModel.Models
{
    [ExcludeFromCodeCoverage]
    [JsonObject(MemberSerialization.OptIn)]
    public partial class Client
    {
        public Client()
        {
            this.InsideUserSets = new List<InsideUser>();
            this.Leads = new List<Contact>();
            this.Budgets = new List<Budget>();
            this.Labels = new List<Label>();
            this.Tags = new List<Tag>();
        }


        public int Id { get; set; }

        public string Name { get; set; }

        public string Domain { get; set; }

        public string AnalyticsTableId { get; set; }

        public string EmailAddress { get; set; }

        public int? CallTrackingMetricId { get; set; }

        public string ConsultantId { get; set; }

        public string Address { get; set; }

        public decimal? Longitude { get; set; }

        public decimal? Latitude { get; set; }

        public bool IsActive { get; set; }

        public string AccountManagerId { get; set; }

        public decimal FeeFixedMonthly { get; set; }
        public decimal FeePercent { get; set; }

        public virtual InsideUser Consultant{ get; set; }
        public virtual InsideUser AccountManager { get; set; }

        public virtual ICollection<InsideUser> InsideUserSets { get; set; }
        public virtual ICollection<Contact> Leads { get; set; }
        public virtual ICollection<Budget> Budgets { get; set; }
        public virtual ICollection<Label> Labels { get; set; }
        public virtual ICollection<Tag> Tags { get; set; }
    }
}
