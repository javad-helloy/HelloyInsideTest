using System.Collections.Generic;
using InsideReporting.Models.Layout;

namespace InsideReporting.Models.User
{
    public class UsersViewModel : CollectionViewModel<InsideUserViewModel>
    {
        public UsersViewModel(IList<string> roleList) : base(roleList)
        {
        }
    }
}