using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Models
{
    public class Content
    {
        [Key]
        [ForeignKey("Message")]
        public int ContentId { get; set; }
        public string Text { get; set; }
        public DateTime Time { get; set; }
        public Message Message { get; set; }
    }
}
