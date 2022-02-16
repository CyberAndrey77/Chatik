using System;
using System.Collections.Generic;

namespace Common
{
    public class ChatMessageResponseServer
    {
        public Guid MessageId { get; }
        public string Message { get; set; }
        public int SenderUserId { get; set; }
        public int ChatId { get; set; }
        public List<int> UserIds { get; set; }
        public DateTime Time { get; set; }
        public bool IsDialog { get; set; }

        public ChatMessageResponseServer(int senderUserId, string message, int chatId, List<int> userIds, bool isDialog, DateTime time)
        {
            Message = message;
            SenderUserId = senderUserId;
            ChatId = chatId;
            UserIds = userIds;
            MessageId = Guid.NewGuid();
            Time = time;
            IsDialog = isDialog;
        }

        public MessageContainer GetContainer()
        {
            var container = new MessageContainer
            {
                Identifier = nameof(ChatMessageResponseServer),
                Payload = this
            };

            return container;
        }
    }
}
