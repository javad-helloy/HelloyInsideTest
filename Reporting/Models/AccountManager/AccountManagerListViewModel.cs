using System.Collections.Generic;
using InsideReporting.Models.Layout;

namespace InsideReporting.Models.AccountManager
{
    public class AccountManagerListViewModel : CollectionViewModel<AccountManagerViewModel>
    {
        public AccountManagerListViewModel(IList<string> roleList) : base(roleList)
        {
        }
    }
}