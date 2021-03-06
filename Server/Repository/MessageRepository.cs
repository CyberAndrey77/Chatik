using Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Repository
{
    public class MessageRepository : IRepository<Message>
    {
        private readonly ChatDb _context;

        public MessageRepository(ChatDb context)
        {
            _context = context;
        }

        public void Create(Message item)
        {
            _context.Messages.Add(item);
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

        public Message GetElement(int id)
        {
            throw new NotImplementedException();
        }

        public List<Message> GetElementList(int id)
        {
            return _context.Messages.Where(m => m.ChatId == id).ToList();
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public void Update(Message item)
        {
            throw new NotImplementedException();
        }
    }
}
