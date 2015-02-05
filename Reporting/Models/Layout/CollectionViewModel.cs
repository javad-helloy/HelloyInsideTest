using System.Collections.Generic;

namespace InsideReporting.Models.Layout
{
    public class CollectionViewModel<T>: LoggedInViewModel
    {
        public CollectionViewModel(IList<string> roleList)
        {
            Collection = new List<T>();
            Roles = roleList;
            AddMenu();
        }
        public IList<T> Collection { get; set; }
    }
}