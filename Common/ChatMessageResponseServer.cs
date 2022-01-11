using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class ChatMessageResponseServer
    {
        public Guid MessageId { get; }
        public string Message { get; set; }
        public Guid SenderUserId { get; set; }
        public string ChatName { get; set; }
        public List<Guid> UserIds { get; set; }
        public DateTime Time { get; set; }

        public ChatMessageResponseServer(Guid senderUserId, string message, string chatName, List<Guid> userIds,  DateTime time)
        {
            Message = message;
            SenderUserId = senderUserId;
            ChatName = chatName;
            UserIds = userIds;
            MessageId = Guid.NewGuid();
            Time = time;
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
