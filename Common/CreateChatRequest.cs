using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class CreateChatRequest
    {
        public string ChatName { get; set; }
        public string CreatorName { get; set; }
        public DateTime Time { get; set; }

        public CreateChatRequest(string chatName, string creatorName, DateTime time)
        {
            ChatName = chatName;
            CreatorName = creatorName;
            Time = time;
        }
        
        public MessageContainer GetContainer()
        {
            var container = new MessageContainer
            {
                Identifier = nameof(CreateChatRequest),
                Payload = this
            };

            return container;
        }
    }
}
