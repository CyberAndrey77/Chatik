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
        public int SenderUserId { get; set; }
        public string ChatName { get; set; }
        public List<int> UserIds { get; set; }
        public DateTime Time { get; set; }

        public ChatMessageResponseServer(int senderUserId, string message, string chatName, List<int> userIds,  DateTime time)
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
