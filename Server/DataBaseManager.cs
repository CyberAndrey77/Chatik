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
        private ChatDb _chatDbContext;
        private readonly string _connectionString;
        public Network Network { get; set; }
        public DataBaseManager(Network network, string connectionString)
        {
            Network = network;
            Network.ConnectionEvent += OnConnectionUser;
            Network.GetUserChats += OnGetUserChats;
            Network.ChatMessageEvent += OnChatMessage;
            _connectionString = connectionString;

            // под индексом 1 всегда находится Главный чат.
            using (_chatDbContext = new ChatDb(_connectionString))
            {
                var chat = SearchChat(1);
                if (chat != null) return;
                chat = new Chat() {Name = "Главный чат", IsDialog = false};
                CreateChat(chat);
            }
        }

        private void OnChatMessage(object sender, ChatMessageEventArgs e)
        {
            using (_chatDbContext = new ChatDb(_connectionString))
            {
                var chat = e.ChatId == 0 ? null : SearchChat(e.ChatId);
                if (chat == null)
                {
                    var users = e.UserIds.Select(userId => SearchUser(userId)).ToList();

                    StringBuilder chatNameBuilder = new StringBuilder();
                    foreach (var user in users)
                    {
                        chatNameBuilder.Append(user.Name);
                    }

                    chat = new Chat()
                    {
                        Name = chatNameBuilder.ToString(),
                        IsDialog = e.IsDialog
                    };
                    users.ForEach(item => chat.Users.Add(item));
                    CreateChat(chat);
                    e.ChatId = chat.Id;
                }

                //var senderUser = SearchUser(e.SenderUserId);
                CreateMessage(new Message()
                {
                    Text = e.Message,
                    Time = e.Time = DateTime.Now,
                    SenderId = e.SenderUserId,
                    ChatId = e.ChatId
                });
            }
        }

        private void OnCreateChat()
        {

        }

        private void OnGetUserChats(object sender, UserChatEventArgs<Chat> e)
        {
            using (_chatDbContext = new ChatDb(_connectionString))
            {
                var chats = SearchChats(e.UserId);
                e.Chats = chats;
            }
        }

        private void OnConnectionUser(object sender, ConnectStatusChangeEventArgs e)
        {
            if (e.ConnectionRequestCode == ConnectionRequestCode.Disconnect)
            {
                return;
            }

            using (_chatDbContext = new ChatDb(_connectionString))
            {
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
        }

        private User SearchUser(string login)
        {
            var repository = new UserRepository(_chatDbContext);
            return repository.GetElement(login);
        }

        private User SearchUser(int id)
        {
            var repository = new UserRepository(_chatDbContext);
            return repository.GetElement(id);
        }

        private Chat SearchChat(int id)
        {
            var repository = new ChatRepository(_chatDbContext);
            return repository.GetElement(id);
        }

        private List<Chat> SearchChats(int id)
        {
            var repository = new ChatRepository(_chatDbContext);
            var chatList = repository.GetElementList(id);
            return chatList;
        }

        private void CreateUser(User user)
        {
            var repository = new UserRepository(_chatDbContext);
            repository.Create(user);
        }

        private void CreateChat(Chat chat)
        {
            var repository = new ChatRepository(_chatDbContext);
            repository.Create(chat);
        }

        private void CreateMessage(Message message)
        {
            var repository = new MessageRepository(_chatDbContext);
            repository.Create(message);
        }
        private void DeleteUser(User user)
        {
            var repository = new UserRepository(_chatDbContext);
        }
    }
}
