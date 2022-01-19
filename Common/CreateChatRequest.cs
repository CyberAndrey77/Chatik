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
        public int ChatId { get; set; }
        public string ChatName { get; set; }
        public string CreatorName { get; set; }
        public bool IsDialog { get; set; }
        public List<int> UserIds { get; set; }
        public DateTime Time { get; set; }

        public CreateChatRequest(string chatName, int chatId, string creatorName, bool isDialog, List<int> userIds, DateTime time)
        {
            ChatId = chatId;
            ChatName = chatName;
            IsDialog = isDialog;
            CreatorName = creatorName;
            UserIds = userIds;
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
