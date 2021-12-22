using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Models
{
    public class Chat
    {
        public int ChatId { get; set; }
        public string ChatName { get; set; }

        public List<User> Users { get; set; }
    }
}
