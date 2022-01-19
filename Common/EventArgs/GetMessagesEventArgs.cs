using System;
using System.Collections.Generic;

namespace Common.EventArgs
{
    public class GetMessagesEventArgs<T>:System.EventArgs
    {
        //public string Text { get; set; }
        //public DateTime Time { get; set; }
        //public int SenderId { get; set; }
        public int ChatId { get; set; }

        public List<T> Messages { get; set; }

        public Dictionary<int, string> Users { get; set; }

        public GetMessagesEventArgs(int chatId)
        {
            ChatId = chatId;
            Messages = new List<T>();
            Users = new Dictionary<int, string>();
        }
    }
}