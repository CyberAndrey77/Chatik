using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Models
{
    public class Message
    {
        public int ChatMessageId { get; set; }

        [ForeignKey("User")]
        public int SenderId { get; set; }
        public int? ChatId { get; set; }

        [ForeignKey("User")]
        public int? ReceiverId { get; set; }
        public int ContentId { get; set; }

        public User User { get; set; }
        public Chat Chat { get; set; }
        public Content Content { get; set; }
    }
}
