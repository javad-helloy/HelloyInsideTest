using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using InsideReporting.Models.Layout;

namespace InsideReporting.Models
{
    public class ChatViewModel : LoggedInViewModel{
        public ChatViewModel()
        {
            
            ClientName = "";
        }
        public ChatViewModel(IList<string> rolList)
        {
            
            ClientName = "";
            Roles = rolList;
            AddMenu();
        }
        [DisplayName("Id")]
        public int Id { get; set; }

        [DisplayName("Kund Id")]
        [Required]
        public int ClientId { get; set; }
        
        [DisplayName("Datum")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}")]
        [Required]
        public DateTime Date { get; set; }
        
        [DisplayName("Beskrivning")]
        [Required]
        public string Description { get; set; }
        [DisplayName("Telefonnummer")]
        public string Phone { get; set; }

        [EmailAddress(ErrorMessage = "Fyll i en riktig epostadress")]
        [DisplayName("Epostadress")]
        public string Email { get; set; }

        [DisplayName("Kund")]
        public string ClientName { get; set; }

        [DisplayName("Live Chat Id")]
        public string LiveChatId { get; set; }
    }
}