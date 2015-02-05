using System.Collections.Generic;
using InsideReporting.Models.Layout;

namespace InsideReporting.Models.Chat
{
    public class ChatListViewModel : CollectionViewModel<ChatViewModel>
    {
        public ChatListViewModel(IList<string> roleList) : base(roleList)
        {
        }
    }
}