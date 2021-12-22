using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class PrivateMessageResponseServer
    {
        public Guid MessageId { get; }
        public string Message { get; set; }
        public string SenderName { get; set; }
        public string ReceiverName { get; set; }

        public DateTime Time { get; set; }
        public PrivateMessageResponseServer(string senderName, string message, string receiverName, DateTime time)
        {
            Message = message;
            SenderName = senderName;
            ReceiverName = receiverName;
            MessageId = Guid.NewGuid();
            Time = time;
        }


        public MessageContainer GetContainer()
        {
            var container = new MessageContainer
            {
                Identifier = nameof(PrivateMessageResponseServer),
                Payload = this
            };

            return container;
        }
    }
}
