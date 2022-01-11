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
        public Guid SenderUserId { get; set; }
        public string ChatName { get; set; }
        public List<Guid> UserIds { get; set; }
        public DateTime Time { get; set; }

        public ChatMessageEventArgs(Guid senderUserId, string message, string chatName, List<Guid> userIds, DateTime time)
        {
            Message = message;
            SenderUserId = senderUserId;
            ChatName = chatName;
            UserIds = userIds;
            Time = time;
        }
    }
}
