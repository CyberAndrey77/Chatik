using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class CreateChatResponse
    {
        public int ChatId { get; set; }
        public string ChatName { get; set; }
        public string CreatorName { get; set; }
        public List<int> UserIds { get; set; }

        public DateTime Time { get; set; }
        public bool IsDialog { get; set; }

        //public CreateChatResponse(string chatName, string creator, List<string> user)
        //{
        //    Chats = chatName;
        //    CreatorName = creator;
        //    UserIds = user;
        //}


        public CreateChatResponse(string chatName, int chatId, string creator, List<int> user, DateTime time, bool isDialog)
        {
            ChatId = chatId;
            ChatName = chatName;
            CreatorName = creator;
            UserIds = user;
            Time = time;
            IsDialog = isDialog;
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
