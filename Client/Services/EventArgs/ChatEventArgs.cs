using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Client.Services.EventArgs
{
    public class ChatEventArgs : System.EventArgs
    {
        public int ChatId { get; set; }
        public string ChatName { get; set; }
        public string CreatorName { get; set; }
        public DateTime Time { get; set; }
        public List<int> UserIds { get; set; }

        public bool IsDialog { get; set; }

        public ChatEventArgs(string chatName, string creatorName, List<int> userIds, DateTime time)
        {
            ChatName = chatName;
            CreatorName = creatorName;
            UserIds = userIds;
            Time = time;
        }

        public ChatEventArgs(string chatName, int chatId, string creatorName, List<int> userIds, bool isDialog, DateTime time)
        {
            ChatId = chatId;
            ChatName = chatName;
            CreatorName = creatorName;
            Time = time;
            UserIds = userIds;
            IsDialog = isDialog;
        }
    }
}
