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
        public string ChatName { get; set; }
        public List<int> UserIds { get; set; }

        public ChatMessageResponse(int senderUserId, string message, string chatName, List<int> userIds)
        {
            Message = message;
            SenderUserId = senderUserId;
            ChatName = chatName;
            UserIds = userIds;
            MessageId = Guid.NewGuid();
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
