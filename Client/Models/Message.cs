using Common.Enums;
using System;

namespace Client.Models
{
    public class Message
    {
        public int ChatId { get; set; }

        public int SenderId { get; set; }

        public string Text { get; set; }

        public string Name { get; set; }

        public DateTime Time { get; set; }

        public MessageStatus Status { get; set; }
    }
}