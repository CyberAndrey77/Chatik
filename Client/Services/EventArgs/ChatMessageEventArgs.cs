using System;
using System.Collections.Generic;

namespace Client.Services.EventArgs
{
    public class ChatMessageEventArgs : System.EventArgs
    {
        public string Message { get; set; }
        public int SenderUserId { get; set; }
        public int ChatId { get; set; }
        public List<int> UserIds { get; set; }
        public DateTime Time { get; set; }

        public ChatMessageEventArgs(int senderUserId, string message, int chatId, List<int> userIds, DateTime time)
        {
            Message = message;
            SenderUserId = senderUserId;
            ChatId = chatId;
            UserIds = userIds;
            Time = time;
        }
    }
}
