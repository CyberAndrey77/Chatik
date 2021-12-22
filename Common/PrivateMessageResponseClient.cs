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
        public string SenderName { get; set; }
        public string ReceiverName { get; set; }
        public PrivateMessageResponseClient(string senderName, string message, string receiverName)
        {
            Message = message;
            SenderName = senderName;
            ReceiverName = receiverName;
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
