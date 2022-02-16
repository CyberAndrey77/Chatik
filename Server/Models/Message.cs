using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Models
{
    public class Message
    {
        [Key]
        public int ChatMessageId { get; set; }
        public string Text { get; set; }
        public DateTime Time { get; set; }

        [ForeignKey("SenderUser")]
        public int SenderId { get; set; }
        [ForeignKey("Chat")]
        public int? ChatId { get; set; }
        public User SenderUser { get; set; }
        public Chat Chat { get; set; }
    }
}
