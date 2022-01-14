using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Services.EventArgs
{
    public class ChatEventArgs : System.EventArgs
    {
        public string ChatName { get; set; }
        public string CreatorName { get; set; }

        public DateTime Time { get; set; }
        public List<int> UserIds { get; set; }

        public ChatEventArgs(string chatName, string creatorName, List<int> userIds, DateTime time)
        {
            ChatName = chatName;
            CreatorName = creatorName;
            UserIds = userIds;
            Time = time;
        }

        public ChatEventArgs(string chatName, string creatorName, DateTime time)
        {
            ChatName = chatName;
            CreatorName = creatorName;
            Time = time;
            UserIds = new List<int>();
        }
    }
}
