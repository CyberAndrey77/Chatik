using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class ChatMessageResponse
    {
        public Guid MessageId { get; }
        public string Message { get; set; }
        public string SenderName { get; set; }
        public string ChatName { get; set; }
        public List<string> Users { get; set; }

        public ChatMessageResponse(string senderName, string message, string chatName, List<string> users)
        {
            Message = message;
            SenderName = senderName;
            ChatName = chatName;
            Users = users;
            MessageId = Guid.NewGuid();
        }


        public virtual MessageContainer GetContainer()
        {
            var container = new MessageContainer
            {
                Identifier = nameof(ChatMessageResponse),
                Payload = this
            };

            return container;
        }
    }
}
