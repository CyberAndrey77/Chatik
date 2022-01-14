using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Enums;
using Common.EventArgs;
using Server.Models;
using Server.Repository;

namespace Server
{
    public class DataBaseManager
    {
        private string _connectionString;
        public Network Network { get; set; }
        public DataBaseManager(Network network, string connectionString)
        {
            Network = network;
            Network.ConnectionEvent += OnConnectionUser;
            Network.GetUserChats += OnGetUserChats;
            _connectionString = connectionString;

            // под индексом 1 всегда находится Главный чат.
            var chat = SearchChat(1);
            if (chat != null) return;
            chat = new Chat() {Name = "Главный чат", IsDialog = false};
            CreateChat(chat);
        }

        private void OnGetUserChats(object sender, UserChatEventArgs<Chat> e)
        {
            var chats = SearchChats(e.UserId);
            e.Chats = chats;
        }

        private void OnConnectionUser(object sender, ConnectStatusChangeEventArgs e)
        {
            if (e.ConnectionRequestCode == ConnectionRequestCode.Disconnect)
            {
                return;
            }

            var user = SearchUser(e.Name);
            if (user == null)
            {
                user = new User() { Name = e.Name };

                // под индексом 1 всегда находится Главный чат.
                var chat = SearchChat(1);
                user.Chats.Add(chat);
                CreateUser(user);
            }

            Network.Server.Connections.TryRemove(e.Id, out var connection);
            connection.Id = user.Id;
            Network.Server.Connections.TryAdd(user.Id, connection);
        }

        private User SearchUser(string login)
        {
            var repository = new ChatRepository<User>(_connectionString);
            return repository.GetElement(login);
        }

        private Chat SearchChat(int id)
        {
            var repository = new ChatRepository<Chat>(_connectionString);
            return repository.GetElement(id);
        }

        private List<Chat> SearchChats(int id)
        {
            var repository = new ChatRepository<Chat>(_connectionString);
            var chatList = repository.GetElementList(id);
            return chatList;
        }

        private void CreateUser(User user)
        {
            var repository = new ChatRepository<User>(_connectionString);
            repository.Create(user);
        }

        private void CreateChat(Chat chat)
        {
            var repository = new ChatRepository<Chat>(_connectionString);
            repository.Create(chat);
        }

        private void DeleteUser(User user)
        {
            var repository = new ChatRepository<User>(_connectionString);
        }
    }
}
