using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using InsideReporting.Controllers;
using InsideReporting.Models.Layout;
using Newtonsoft.Json;
using Ninject.Infrastructure.Language;

namespace InsideReporting.Models
{
    public class ClientLoggedInViewModel : SiriusReportModel
    {
        public ClientLoggedInViewModel(IList<string> roleList, ClientViewModel clientViewModel)
        {
            this.ClientViewModel = clientViewModel;
        }

        public ClientLoggedInViewModel()
        {
            
        }
        public ClientViewModel ClientViewModel { get; set; }
    }

    public class ClientPageLoggedInViewModel : LoggedInViewModel
    {
        public ClientPageLoggedInViewModel(IList<string> roleList, ClientViewModel clientViewModel)
        {
            this.ClientViewModel = clientViewModel;
            base.Roles = roleList;
            AddMenu();
        }

        public ClientPageLoggedInViewModel()
        {

        }
        public ClientViewModel ClientViewModel { get; set; }
    }
    public class ClientViewModel 
    {
        public ClientViewModel()
        {
            this.Address = "";
            this.AnalyticsTableId = "";
            this.CallTrackingMetricId = null;
            this.Domain = "";
            this.ConsultantId = null;
            this.AccountManagerId = null;
            this.EmailAddress = "";
            this.IsActive = false;
            this.LastLogin = null;
            this.ActivityLevel = 0;
            this.FeeFixedMonthly = 0;
            this.FeePercent = 100;

        }
        public ClientViewModel(InsideModel.Models.Client client)
        {
            AccountManagerId = client.AccountManagerId;
            Address = client.Address;
            AnalyticsTableId = client.AnalyticsTableId;
            CallTrackingMetricId = client.CallTrackingMetricId;
            ConsultantId = client.ConsultantId;
            Domain = client.Domain;
            EmailAddress = client.EmailAddress;
            Id = client.Id;
            IsActive = client.IsActive;
            Latitude = client.Latitude;
            Longitude = client.Longitude;
            Name = client.Name;
            ConsultantName = client.Consultant == null ? "" : client.Consultant.Name;
            AccountManagerName = client.AccountManager == null ? "" : client.AccountManager.Name;

            FeeFixedMonthly = (int)Math.Truncate(client.FeeFixedMonthly) ;
            FeePercent = (int)Math.Truncate(client.FeePercent*100m);
            Labels = client.Labels.ToList().Select(l => new LabelViewModel(l)).ToEnumerable();

        }
        [JsonProperty]
        public int Id { get; set; }

        [JsonProperty]
        [DisplayName("Namn")]
        [System.ComponentModel.DataAnnotations.Required]
        public string Name { get; set; }

        [JsonProperty]
        [DisplayName("Domän")]
        public string Domain { get; set; }

        [JsonProperty]
        [DisplayName("Analytics Table Id")]
        public string AnalyticsTableId { get; set; }

        [JsonProperty]
        [DisplayName("Spårbara epostadresser")]
        public string EmailAddress { get; set; }

        [JsonProperty]
        [DisplayName("CalltrackingMetric Id")]
        public int? CallTrackingMetricId { get; set; }

        [JsonProperty]
        [DisplayName("Konsult")]
        public string ConsultantId { get; set; }

        [JsonProperty]
        [DisplayName("Adress")]
        public string Address { get; set; }

        [JsonProperty]
        [DisplayName("Longitud")]
        public decimal? Longitude { get; set; }

        [JsonProperty]
        [DisplayName("Lattitude")]
        public decimal? Latitude { get; set; }

        [JsonProperty]
        [DisplayName("Aktiv")]
        public bool IsActive { get; set; }

        [JsonProperty]
        [DisplayName("Status")]
        public DateTime? LastLogin { get; set; }

        [JsonProperty]
        [DisplayName("Aktivitets Nivå")]
        public int ActivityLevel { get; set; }

        [JsonProperty]
        [DisplayName("Kontoansvarig")]
        public string AccountManagerId { get; set; }

        [JsonProperty]
        public string ConsultantName { get; set; }

        [JsonProperty]
        public string AccountManagerName { get; set; }

        [JsonProperty]
        [DisplayName("Fast månadskostnad")]
        
        public int FeeFixedMonthly { get; set; }

        [JsonProperty]
        [DisplayName("Annonseringsarvode")]
        
        public int FeePercent { get; set; }

        public object WarningColor
        {
            get
            {
                if (ActivityLevel == 2)
                {
                    return "#47b947";
                }
                else if (ActivityLevel == 1)
                {
                    return "#009dd2";
                }
                else
                {
                    return "#ff8202";
                }
            }
        }

        [JsonProperty]
        public IEnumerable<LabelViewModel> Labels { get; set; }
    }
}