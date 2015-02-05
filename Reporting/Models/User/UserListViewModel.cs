using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using InsideModel.Models;
using InsideReporting.Controllers;

namespace InsideReporting.Models.User
{
    public class UserListViewModel : SiriusReportModel
    {
        public UserListViewModel()
        {
            this.Users = new List<UserViewModel>();
        }

        public IList<UserViewModel> Users { get; set; }
    }

    public class UserEditViewModel : SiriusReportModel
    {
        public UserEditViewModel(){}

        public UserEditViewModel(UserViewModel userViewModel)
        {
            this.User = userViewModel;
        }   

        public UserViewModel User { get; set; }
    }

    public class UserCreateViewModel : SiriusReportModel
    {
        public UserCreateViewModel()
        {
            this.User = new UserViewModel();
        }

        public UserCreateViewModel(UserViewModel user)
        {
            this.User = user;
        }

        public UserViewModel User { get; set; }
    }

    public class UserViewModel
    {
        public UserViewModel()
        {
            IsEditingExcisting = false;
        }

        public UserViewModel(InsideUser user)
        {
            IsEditingExcisting = true;
            this.IsLockedOut = user.IsLockedOut;
            this.Name = user.Email;
            this.Phone = user.Phone;
            this.Id = user.Id;
   
            if (user.ReceiveEmail.HasValue)
            {
                this.ReceiveEmail = user.ReceiveEmail.Value;
            }
            else
            {
                this.ReceiveEmail = false;
            }

            if (user.ReceiveSms.HasValue)
            {
                this.ReceiveSms = user.ReceiveSms.Value;
            }
            else
            {
                this.ReceiveSms = false;
            }

            if (user.Client != null)
            {
                ClientId = user.Client.Id;
                ClientName = user.Client.Name;    
            }
        }

        public bool IsEditingExcisting { get; set; }

        public string Id { get; set; }

        [Required]
        [RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", ErrorMessage = "Fyll i en riktig epostadress")]
        [Display(Name = "Inloggning (email)")]
        public string Name { get; set; }

        [Display(Name = "Telefon")]
        public string Phone { get; set; }

        [Required]
        [Display(Name = "Kund")]
        public int ClientId { get; set; }

        [Display(Name = "Lösenord")]
        [StringLength(255, MinimumLength = 6)]
        public string Password { get; set; }

        [Display(Name = "Tar emot SMS")]
        public bool ReceiveSms { get; set; }

        [Display(Name = "Tar emot E-post")]
        public bool ReceiveEmail { get; set; }

        [Display(Name = "Låst")]
        public bool IsLockedOut { get; set; }

        public string ClientName { get; set; }
    }
}