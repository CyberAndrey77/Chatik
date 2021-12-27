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
        public string SenderName { get; set; }
        public string ChatName { get; set; }
        public List<string> Users { get; set; }
        public DateTime Time { get; set; }

        public ChatMessageResponseServer(string senderName, string message, string chatName, List<string> users,  DateTime time)
        {
            Message = message;
            SenderName = senderName;
            ChatName = chatName;
            Users = users;
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
