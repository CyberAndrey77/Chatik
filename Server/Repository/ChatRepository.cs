using Server.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Server.Repository
{
    public class ChatRepository : IRepository<Chat>
    {
        private readonly ChatDb _context;

        public ChatRepository(ChatDb context)
        {
            _context = context;
        }
        public void Create(Chat item)
        {
            _context.Chats.Add(item);
            Save();
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Chat GetElement(int id)
        {
            return _context.Chats.Include(x => x.Users).ToList().Find(x => x.Id == id);
        }

        public List<Chat> GetElementList(int id)
        {
            var users = _context.Users.Include(c => c.Chats).ToList().Find(u => u.Id == id);
            return users.Chats;
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public void Update(Chat item)
        {
            throw new NotImplementedException();
        }
    }
}
