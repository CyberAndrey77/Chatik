namespace Server.EventArgs
{
    using System;
    using System.Collections.Generic;
    public class CreateChatEventArgs: EventArgs
    {
        public int ChatId
        {
            get;
            set;
        }
        public string ChatName { get; set; }
        public int SenderUserId { get; set; }
        public List<int> UserIds { get; set; }

        public bool IsDialog { get; set; }

        public DateTime Time { get; set; }

        public CreateChatEventArgs(int senderUserId, string chatName, List<int> userIds, bool isDialog)
        {
            ChatName = chatName;
            SenderUserId = senderUserId;
            UserIds = userIds;
            IsDialog = isDialog;
        }
    }
}