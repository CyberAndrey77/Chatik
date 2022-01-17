using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class PrivateMessageResponseClient
    {
        public Guid MessageId { get; }
        public string Message { get; set; }
        public int SenderUserId { get; set; }
        public int ChatId { get; set; }
        public List<int> UserIds { get; set; }
        public PrivateMessageResponseClient(int senderUserId, string message, int chatId, List<int> userIds)
        {
            ChatId = chatId;
            Message = message;
            SenderUserId = senderUserId;
            UserIds = userIds;
            MessageId = Guid.NewGuid();
        }


        public MessageContainer GetContainer()
        {
            var container = new MessageContainer
            {
                Identifier = nameof(PrivateMessageResponseClient),
                Payload = this
            };

            return container;
        }
    }
}
