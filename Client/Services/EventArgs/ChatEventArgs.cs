using System;
using System.Collections.Generic;

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
