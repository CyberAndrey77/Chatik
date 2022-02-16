using Server.Models;
using System.Data.Entity;

namespace Server.Repository
{
    public class ChatDb : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Log> Logs { get; set; }

        public ChatDb(string stringConnection) : base(stringConnection)
        {
            var ensureDLLIsCopied =
                System.Data.Entity.SqlServer.SqlProviderServices.Instance;
        }

        public ChatDb()
        {

        }
    }
}
