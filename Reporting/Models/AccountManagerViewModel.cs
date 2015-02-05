using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using InsideReporting.Models.Layout;

namespace InsideReporting.Models
{
    public class AccountManagerViewModel : LoggedInViewModel
    {

        public AccountManagerViewModel()
        {
            this.Password = "";
            this.Name = "";
            
        }
        public AccountManagerViewModel(IList<string> rolList)
        {
            this.Password = "";
            this.Name = "";
            Roles = rolList;
            AddMenu();

        }
        [DisplayName("Id")]
        public string Id { get; set; }

        [DisplayName("Namn")]
        [Required]
        public string Name { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Lösenord")]
        public string Password { get; set; }

        [RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", ErrorMessage = "Fyll i en riktig epostadress")]
        [Display(Name = "Mail")]
        [Required]
        public string Email { get; set; }

        [Display(Name = "Telefonnummer")]
        public string Phone { get; set; }

        [Url(ErrorMessage = "Invalid URL!")]
        [Display(Name = "Bild Url")]
        public string ImageUrl { get; set; }

        public bool IsLockedOut { get; set; }
    }
}