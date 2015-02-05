using System.Collections.Generic;
using System.Web.Mvc;

namespace InsideReporting.Models
{
    public class ClientPageViewModel : ClientLoggedInViewModel
    {
        public ClientPageViewModel(IList<string> roleList, ClientViewModel clientViewModel)
            : base(roleList,clientViewModel)
        {
            
        }

        public ClientPageViewModel()
        {
           
        }

        public SelectList Consultant { get; set; }
        public SelectList AccountManager { get; set; }
    }

    public class ClientPageLoggedViewModel : ClientPageLoggedInViewModel
    {
        public ClientPageLoggedViewModel(IList<string> roleList, ClientViewModel clientViewModel)
            : base(roleList, clientViewModel)
        {

        }

        public ClientPageLoggedViewModel()
        {

        }

        public SelectList Consultant { get; set; }
        public SelectList AccountManager { get; set; }
    }
}