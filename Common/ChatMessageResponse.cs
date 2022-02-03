using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class ChatMessageResponse
    {
        public Guid MessageId { get; }
        public string Message { get; set; }
        public int SenderUserId { get; set; }
        public int ChatId { get; set; }
        public List<int> UserIds { get; set; }

        public bool IsDialog { get; set; }

        public ChatMessageResponse(int senderUserId, string message, int chatId, List<int> userIds, bool isDialog, Guid messageId)
        {
            Message = message;
            SenderUserId = senderUserId;
            ChatId = chatId;
            UserIds = userIds;
            MessageId = messageId;
            IsDialog = isDialog;
        }


        public virtual MessageContainer GetContainer()
        {
            var container = new MessageContainer
            {
                Identifier = nameof(ChatMessageResponse),
                Payload = this
            };

            return container;
        }
    }
}
