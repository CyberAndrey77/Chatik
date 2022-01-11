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
        public Guid SenderUserId { get; set; }
        public Guid ReceiverUserId { get; set; }
        public PrivateMessageResponseClient(Guid senderUserId, string message, Guid receiverUserId)
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
