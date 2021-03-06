namespace Server.EventArgs
{
    using System;
    using System.Collections.Generic;
    using Server.Models;
    public class ChatMessageEventArgs: EventArgs
    {
        public Guid MessageId { get; }
        public string Message { get; set; }
        public int SenderUserId { get; set; }
        public int ChatId { get; set; }
        public List<int> UserIds { get; set; }

        public bool IsDialog { get; set; }

        public DateTime Time { get; set; }

        public ChatMessageEventArgs(int senderUserId, string message, int chatId, List<int> userIds, bool isDialog, Guid messageId)
        {
            Message = message;
            SenderUserId = senderUserId;
            ChatId = chatId;
            UserIds = userIds;
            MessageId = messageId;
        }
    }
}