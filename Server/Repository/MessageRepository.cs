using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.Models;

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
            throw new NotImplementedException();
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
