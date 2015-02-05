using System.Collections.Generic;
using InsideReporting.Models.Layout;

namespace InsideReporting.Models.Client
{
    public class ClientsViewModel : LoggedInViewModel
    {
        public ClientsViewModel(IList<string> roleList)
        {
            Clients = new List<ClientViewModel>();
            Labels = new List<LabelViewModel>();
            Roles = roleList;
            AddMenu();
        }

        public IList<ClientViewModel> Clients { get; set; }
        public IList<LabelViewModel> Labels { get; set; }
    }
}