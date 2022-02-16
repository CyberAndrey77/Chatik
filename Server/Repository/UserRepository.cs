using Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Repository
{
    public class UserRepository : IRepository<User>
    {
        private readonly ChatDb _context;

        public UserRepository(ChatDb context)
        {
            _context = context;
        }

        public void Create(User item)
        {
            _context.Users.Add(item);
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

        public User GetElement(int id)
        {
            return _context.Users.FirstOrDefault(x => x.Id == id);
        }

        public User GetElement(string login)
        {
            return _context.Users.FirstOrDefault(x => x.Name == login);
        }

        public List<User> GetElementList(int id)
        {
            throw new NotImplementedException();
        }

        public List<User> GetElementList()
        {
            return _context.Users.ToList();
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public void Update(User item)
        {
            throw new NotImplementedException();
        }
    }
}
