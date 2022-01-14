using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Server.Models;

namespace Server.Repository
{
    public class ChatRepository<T> : IRepository<T> where T : class
    {
        private readonly ChatDB _chatDb;

        public ChatRepository(string connectionString)
        {
            _chatDb = new ChatDB(connectionString);
        }

        public void Create(T item)
        {
            switch (item)
            {
                case User user:
                    _chatDb.Users.Add(user);
                    break;
                case Chat chat:
                    _chatDb.Chats.Add(chat);
                    break;
            }
            Save();
            Dispose();
        }

        public void Delete(int id)
        {
            switch (GetType().GenericTypeArguments[0].Name)
            {
                case nameof(User):
                    var user = _chatDb.Users.FirstOrDefault(x => x.Id == id);
                    if (user == null)
                    {
                        return;
                    }
                    _chatDb.Users.Remove(user);
                    break;
            }
            Save();
            Dispose();
        }

        public void Dispose()
        {
            _chatDb.Dispose();
        }

        public T GetElement(int id)
        {
            switch (GetType().GenericTypeArguments[0].Name)
            {
                case nameof(Chat):
                    var chat = _chatDb.Chats.FirstOrDefault(x => x.Id == id);
                    return chat as T;
                default:
                    throw new NotImplementedException();
            }
        }

        public T GetElement(string name)
        {
            switch (GetType().GenericTypeArguments[0].Name)
            {
                case nameof(User):
                    var user = _chatDb.Users.FirstOrDefault(x => x.Name == name);
                    return user as T;
                default:
                    throw new NotImplementedException();
            }
        }

        public List<T> GetElementList(int id)
        {
            switch (GetType().GenericTypeArguments[0].Name)
            {
                case nameof(Chat):
                    var users = _chatDb.Users.Where(u => u.Id == id).Include(c => c.Chats).ToList()[0];//.Select(c => c.Chats);
                    var chats = users.Chats as List<T>;
                    //var chatList = chats.Select(chat => chat.Name).ToList();
                    return chats;// as List<T>;
                default:
                    throw new NotImplementedException();
            }

            throw new NotImplementedException();
        }

        public void Save()
        {
            _chatDb.SaveChanges();
        }

        public void Update(T item)
        {
            throw new NotImplementedException();
        }
    }
}
