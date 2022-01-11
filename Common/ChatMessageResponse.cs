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
        public Guid SenderUserId { get; set; }
        public string ChatName { get; set; }
        public List<Guid> UserIds { get; set; }

        public ChatMessageResponse(Guid senderUserId, string message, string chatName, List<Guid> userIds)
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
