using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.EventArgs
{
    public class UserChatEventArgs<T>: System.EventArgs
    {
        public List<T> Chats { get; set; }

        public int UserId { get; set; }

        public UserChatEventArgs(List<T> chats, int userId)
        {
            Chats = chats;
            UserId = userId;
        }

        public UserChatEventArgs(List<T> chats)
        {
            Chats = chats;
        }
    }
}
