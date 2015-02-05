using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using InsideReporting.Models.Layout;

namespace InsideReporting.Models
{
    public class InsideUserViewModel:LoggedInViewModel
    {
        public InsideUserViewModel()
        {
            this.Password = "";
            this.Name = "";
            this.ClientId = -1;
            ClientName = "";
        }

        public InsideUserViewModel(IList<string> rolList)
        {
            this.Password = "";
            this.Name = "";
            this.ClientId = -1;
            ClientName = "";
            Roles = rolList;
            AddMenu();
        }
        public string Id { get; set; }

        [Required]
        [RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", ErrorMessage = "Fyll i en riktig epostadress")]
        [Display(Name = "Inloggning (email)")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Kund")]
        public int ClientId { get; set; }

        [Display(Name = "Lösenord")]
        public string Password { get; set; }

        [Display(Name = "Tar emot E-post")]
        public bool IsReceiveEmail { get; set; }

        public bool IsLockedOut { get; set; }

        public string ClientName { get; set; }
    }
}