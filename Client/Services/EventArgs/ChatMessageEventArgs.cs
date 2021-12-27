using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Models;

namespace Client.Services.EventArgs
{
    public class ChatMessageEventArgs: System.EventArgs
    {
        public string Message { get; set; }
        public string SenderName { get; set; }
        public string ChatName { get; set; }
        public List<string> Users { get; set; }
        public DateTime Time { get; set; }

        public ChatMessageEventArgs(string senderName, string message, string chatName, List<string> users, DateTime time)
        {
            Message = message;
            SenderName = senderName;
            ChatName = chatName;
            Users = users;
            Time = time;
        }
    }
}
