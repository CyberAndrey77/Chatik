using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Models
{
    public class Chat
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsDialog { get; set; }
        public int CountUnreadMessages { get; set; }

        [ForeignKey("User")]
        public List<User> Users { get; set; }
    }
}
