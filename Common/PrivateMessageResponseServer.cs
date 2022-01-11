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
        //public string SenderUserId { get; set; }
        public Guid SenderId { get; set; }
        //public string ReceiverUserId { get; set; }
        public Guid ReceiverId { get; set; }

        public DateTime Time { get; set; }
        public PrivateMessageResponseServer(Guid senderId, string message, Guid receiverId, DateTime time)
        {
            Message = message; 
            SenderId = senderId;
            ReceiverId = receiverId;
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
