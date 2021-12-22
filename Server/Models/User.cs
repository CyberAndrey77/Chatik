using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Models
{
    public class User
    {
        public int UserId { get; set; }

        public string Login { get; set; }

        public List<Chat> Chats { get; set; }
    }
}
