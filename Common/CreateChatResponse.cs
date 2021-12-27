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
        public List<string> InventedNames { get; set; }

        public DateTime Time { get; set; }

        //public CreateChatResponse(string chatName, string creator, List<string> inventedList)
        //{
        //    ChatName = chatName;
        //    CreatorName = creator;
        //    InventedNames = inventedList;
        //}
        

        public CreateChatResponse(string chatName, string creator, List<string> inventedList, DateTime time)
        {
            ChatName = chatName;
            CreatorName = creator;
            InventedNames = inventedList;
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
