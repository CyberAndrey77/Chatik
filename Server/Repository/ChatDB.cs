using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.Models;

namespace Server.Repository
{
    public class ChatDb: DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Log> Logs { get; set; }

        public ChatDb(string stringConnection): base(stringConnection)
        {
            
        }

        public ChatDb()
        {
            
        }
    }
}
