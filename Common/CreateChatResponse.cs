using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class CreateChatResponse
    {
        public string ChatName { get; set; }
        public string CreatorName { get; set; }
        public List<int> UserIds { get; set; }

        public DateTime Time { get; set; }

        //public CreateChatResponse(string chatName, string creator, List<string> user)
        //{
        //    Chats = chatName;
        //    CreatorName = creator;
        //    UserIds = user;
        //}
        

        public CreateChatResponse(string chatName, string creator, List<int> user, DateTime time)
        {
            ChatName = chatName;
            CreatorName = creator;
            UserIds = user;
            Time = time;
        }

        public MessageContainer GetContainer()
        {
            var container = new MessageContainer
            {
                Identifier = nameof(CreateChatResponse),
                Payload = this
            };

            return container;
        }
    }
}
