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
        public int ReceiverUserId { get; set; }
        public PrivateMessageResponseClient(int senderUserId, string message, int receiverUserId)
        {
            Message = message;
            SenderUserId = senderUserId;
            ReceiverUserId = receiverUserId;
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
