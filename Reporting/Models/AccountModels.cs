using System.ComponentModel.DataAnnotations;

namespace InsideReporting.Models
{

    public class LogOnModel
    {
        [Required(ErrorMessage = "Du måste ange ditt användarnamn")]
        [Display(Name = "Användarnamn")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Du måste ange ditt lösenord")]
        [DataType(DataType.Password)]
        [Display(Name = "Lösenord")]
        public string Password { get; set; }

        [Display(Name = "Kom ihåg mig?")]
        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }
    }
}
