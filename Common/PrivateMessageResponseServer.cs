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
        public int SenderId { get; set; }
        //public string UserIds { get; set; }
        public int ReceiverId { get; set; }

        public DateTime Time { get; set; }
        
        public int ChatId { get; set; }
        public List<int> UserIds { get; set; }

        public PrivateMessageResponseServer(int senderId, string message,int chatId, List<int> userIds, DateTime time)
        {
            Message = message; 
            SenderId = senderId;
            ChatId = chatId;
            UserIds = userIds;
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
