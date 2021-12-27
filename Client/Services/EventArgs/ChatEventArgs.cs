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
        public List<string> InventedNames { get; set; }

        public ChatEventArgs(string chatName, string creator, List<string> inventedList, DateTime time)
        {
            ChatName = chatName;
            CreatorName = creator;
            InventedNames = inventedList;
            Time = time;
        }
    }
}
