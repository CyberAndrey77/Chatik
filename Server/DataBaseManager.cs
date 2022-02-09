using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Common.Enums;
using Common.EventArgs;
using Server.EventArgs;
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
            Network.GetMessageEvent += OnGetMessageEvent;
            Network.CreateChatEvent += OnCreateChatEvent;
            Network.GetLogsEvent += OnGetLogs;
            Network.GetAllUsersEvent += OnGetAllUser;
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

        private void OnGetAllUser(object sender, UserDataEventArgs e)
        {
            using (_chatDbContext = new ChatDb(_connectionString))
            {
                var users = GetUsers();
                var allUsers = users.ToDictionary(user => user.Id, user => user.Name);
                Network.Server.SendMessageToClient(new GetAllUsers(allUsers).GetContainer(), e.Id);
            }   
        }

        private void OnGetLogs(object sender, LogEventArgs<Log> e)
        {
            using (_chatDbContext = new ChatDb(_connectionString))
            {
                if (e.Type == -1)
                {
                    if (e.Start == new DateTime(2021, 12, 1) && e.End == DateTime.Today)
                    {
                        e.LogsList = GetLog(e.Type);
                    }
                    else
                    {
                        e.LogsList = GetLogsByTime(e.Start, e.End);
                    }
                }
                else
                {
                    if (e.Start == new DateTime(2021, 12, 1) && e.End == DateTime.Today)
                    {
                        e.LogsList = GetLogsByType((RecordType)e.Type);
                    }
                    else
                    {
                        e.LogsList = GetLogsByTypeAndTime((RecordType)e.Type, e.Start, e.End);
                    }
                }

                Network.Server.SendMessageToClient(new GetLogsRequest<Log>(e.LogsList, e.Type, e.Start, e.End).GetContainer(), e.UserId);
            }
        }

        private void OnCreateChatEvent(object sender, CreateChatEventArgs e)
        {
            using (_chatDbContext = new ChatDb(_connectionString))
            {
                var users = e.UserIds.Select(userId => SearchUser(userId)).ToList();
                var chat = new Chat()
                {
                    Name = e.ChatName,
                    Users = users,
                    IsDialog = e.IsDialog
                };
                CreateChat(chat);
                e.ChatId = chat.Id;

                CreateMessage(new Message()
                {
                    Text = $"{users[0].Name} создал беседу",
                    Time = e.Time = DateTime.Now,
                    SenderId = e.SenderUserId,
                    ChatId = e.ChatId
                });

                //Network.Server.SendMessageToClient(new CreateChatRequest(e.ChatName, e.ChatId, users[0].Name, e.IsDialog, e.UserIds,
                //    e.Time).GetContainer(), e.UserIds[0]);


                for (int i = 0; i < e.UserIds.Count; i++)
                {
                    Network.Server.SendMessageToClient(
                        new CreateChatResponse(e.ChatName, e.ChatId, users[0].Name,
                            e.UserIds, e.Time, e.IsDialog).GetContainer(), e.UserIds[i]);
                }
            }
        }

        private void OnGetMessageEvent<T>(object sender, GetMessagesEventArgs<T> e)
        {
            //хуярим получение сообщений из бд
            using (_chatDbContext = new ChatDb(_connectionString))
            {
                e.Messages = GetMessages(e.ChatId) as List<T>;
                var chat = SearchChat(e.ChatId);
                foreach (var user in chat.Users)
                {
                    e.Users.Add(user.Id, user.Name);
                }

                Network.Server.SendMessageToClient(new GetMessageRequest<Message>(e.ChatId, e.Messages as List<Message>, e.Users).GetContainer(), e.SenderId);
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

                    //var chatName = new StringBuilder();
                    //foreach (var user in users)
                    //{
                    //    chatName.Append(user.Name);
                    //}

                    var chatName = $"{users[0].Name}|{users[1].Name}";
                    chat = new Chat()
                    {
                        Name = chatName,
                        Users = users,
                        IsDialog = e.IsDialog
                    };
                    CreateChat(chat);
                    e.ChatId = chat.Id;
                }

                //var senderUser = SearchUser(e.SenderUserId);
                var message = new Message()
                {
                    Text = e.Message,
                    Time = e.Time = DateTime.Now,
                    SenderId = e.SenderUserId,
                    ChatId = e.ChatId
                };
                CreateMessage(message);

                Network.Server.SendMessageToClient(new MessageRequest(MessageStatus.Delivered, e.Time, e.MessageId) { ChatId = e.ChatId }.GetContainer(), e.SenderUserId);

                foreach (var userId in e.UserIds.Where(userId => userId != e.SenderUserId))
                {
                    Network.Server.SendMessageToClient(
                        new ChatMessageResponseServer(e.SenderUserId, e.Message, e.ChatId, e.UserIds, e.IsDialog, e.Time).GetContainer(), userId);
                }
            }
        }

        private void OnGetUserChats(object sender, UserChatEventArgs<Chat> e)
        {
            using (_chatDbContext = new ChatDb(_connectionString))
            {
                var chats = SearchChats(e.UserId);
                //e.Chats = chats;
                Network.Server.SendMessageToClient(new UserChats<Chat>(chats).GetContainer(), e.UserId);
            }
        }

        private void OnConnectionUser(object sender, ConnectStatusChangeEventArgs e)
        {
            using (_chatDbContext = new ChatDb(_connectionString))
            {
                RecordType type = RecordType.Connect;
                switch (e.ConnectionRequestCode)
                {
                    case ConnectionRequestCode.Disconnect:
                        type = RecordType.Disconnect;
                        break;
                    case ConnectionRequestCode.Connect:
                        type = RecordType.Connect;
                        break;
                }

                CreateLog(new Log() {Time = DateTime.Now, Message = $"{e.Name} {e.ConnectionRequestCode}", Type = type});
                if (e.ConnectionRequestCode != ConnectionRequestCode.Connect)
                {
                    return;
                }

                var user = SearchUser(e.Name);
                if (user == null)
                {
                    user = new User() { Name = e.Name };

                    // под индексом 1 всегда находится Главный чат.
                    var chat = SearchChat(1);
                    if (chat == null)
                    {
                        CreateChat(chat = new Chat() {Id = 1, Name = "Главный чат", IsDialog = false });
                    }
                    user.Chats = new List<Chat>(){chat};
                    CreateUser(user);
                }

                Network.Server.Connections.TryRemove(e.Id, out var connection);
                connection.Id = user.Id;
                Network.Server.Connections.TryAdd(user.Id, connection);

                Network.Server.SendMessageAll(new ConnectionRequest(e.Name, ConnectionRequestCode.Connect, connection.Id).GetContainer(), connection.Id);
            }
        }

        private User SearchUser(string login)
        {
            var repository = new UserRepository(_chatDbContext);
            User user = null;
            try
            {
                user = repository.GetElement(login);
            }
            catch (ArgumentNullException e)
            {
                var log = new Log
                {
                    Time = DateTime.Now,
                    Message = $"Не удалось получить пользователя с login = {login}. {e.Message}",
                    Type = RecordType.Error
                };
                CreateLog(log);
            }

            return user;
        }

        private User SearchUser(int id)
        {
            var repository = new UserRepository(_chatDbContext);
            User user = null;
            try
            {
                user = repository.GetElement(id);
            }
            catch (ArgumentNullException e)
            {
                var log = new Log
                {
                    Time = DateTime.Now,
                    Message = $"Не удалось получить пользователя с id = {id}. {e.Message}",
                    Type = RecordType.Error
                };
                CreateLog(log);
            }

            return user;
        }

        private List<User> GetUsers()
        {
            var repository = new UserRepository(_chatDbContext);
            List<User> users = null;
            try
            {
                users = repository.GetElementList();
            }
            catch (ArgumentNullException e)
            {
                var log = new Log
                {
                    Time = DateTime.Now,
                    Message = $"Не удалось получить всех пользователей. {e.Message}",
                    Type = RecordType.Error
                };
                CreateLog(log);
            }

            return users;
        }

        private Chat SearchChat(int id)
        {
            var repository = new ChatRepository(_chatDbContext);
            Chat chat = null;
            try
            {
                chat = repository.GetElement(id);
            }
            catch (ArgumentNullException e)
            {
                var log = new Log
                {
                    Time = DateTime.Now,
                    Message = $"Не удалось получить чат с id = {id}. {e.Message}",
                    Type = RecordType.Error
                };
                CreateLog(log);
            }

            return chat;
        }

        private List<Chat> SearchChats(int id)
        {
            var repository = new ChatRepository(_chatDbContext);
            List<Chat> chatList = new List<Chat>();
            try
            {
                chatList = repository.GetElementList(id);
            }
            catch (ArgumentNullException e)
            {
                var log = new Log
                {
                    Time = DateTime.Now,
                    Message = $"Не удалось получить чаты для пользователя с id = {id}. {e.Message}",
                    Type = RecordType.Error
                };
                CreateLog(log);
            }
            return chatList;
        }

        private void CreateUser(User user)
        {
            var repository = new UserRepository(_chatDbContext);
            try
            {
                repository.Create(user);
            }
            catch (Exception e)
            {
                var log = new Log
                {
                    Time = DateTime.Now,
                    Message = $"Не удалось записать пользователя в бд. {e.Message}",
                    Type = RecordType.Error
                };
                CreateLog(log);
            }
        }

        private void CreateChat(Chat chat)
        {
            var repository = new ChatRepository(_chatDbContext);
            try
            {
                repository.Create(chat);
            }
            catch (Exception e)
            {
                var log = new Log
                {
                    Time = DateTime.Now,
                    Message = $"Не удалось записать созданный чат в бд. {e.Message}",
                    Type = RecordType.Error
                };
                CreateLog(log);
            }
        }

        private void CreateMessage(Message message)
        {
            var repository = new MessageRepository(_chatDbContext);
            try
            {
                repository.Create(message);
            }
            catch (Exception e)
            {
                var log = new Log
                {
                    Time = DateTime.Now,
                    Message = $"Не удалось записать сообщение в бд. {e.Message}",
                    Type = RecordType.Error
                };
                CreateLog(log);
            }
        }

        private List<Message> GetMessages(int chatId)
        {
            var repository = new MessageRepository(_chatDbContext);
            List<Message> messages = new List<Message>();
            try
            {
                messages = repository.GetElementList(chatId);
            }
            catch (ArgumentNullException e)
            {
                var log = new Log
                {
                    Time = DateTime.Now,
                    Message = $"Не удалось получить сообщения. {e.Message}",
                    Type = RecordType.Error
                };
                CreateLog(log);
            }

            return messages;
        }

        private List<Log> GetLogsByTypeAndTime(RecordType type, DateTime start, DateTime end)
        {
            var repository = new LogRepository(_chatDbContext);
            List<Log> logs = new List<Log>();
            try
            {
                logs = repository.GetElementList(type, start, end);
            }
            catch (ArgumentNullException e)
            {
                var log = new Log
                {
                    Time = DateTime.Now,
                    Message = $"Не удалось получить логи по типу лога и дате. {e.Message}",
                    Type = RecordType.Error
                };
                CreateLog(log);
            }

            return logs;
        }

        private List<Log> GetLogsByTime(DateTime start, DateTime end)
        {
            var repository = new LogRepository(_chatDbContext);
            List<Log> logs = new List<Log>();
            try
            {
                logs = repository.GetElementList(start, end);
            }
            catch (ArgumentNullException e)
            {
                var log = new Log
                {
                    Time = DateTime.Now,
                    Message = $"Не удалось получить логи по дате. {e.Message}",
                    Type = RecordType.Error
                };
                CreateLog(log);
            }

            return logs;
        }

        private List<Log> GetLogsByType(RecordType type)
        {
            var repository = new LogRepository(_chatDbContext);
            List<Log> logs = new List<Log>();
            try
            {
                logs = repository.GetElementList(type);
            }
            catch (ArgumentNullException e)
            {
                var log = new Log
                {
                    Time = DateTime.Now,
                    Message = $"Не удалось получить логи по типу лога. {e.Message}",
                    Type = RecordType.Error
                };
                CreateLog(log);
            }

            return logs;
        }

        private List<Log> GetLog(int id)
        {
            var repository = new LogRepository(_chatDbContext);
            List<Log> logs = new List<Log>();
            try
            {
                logs = repository.GetElementList(id);
            }
            catch (ArgumentNullException e)
            {
                var log = new Log
                {
                    Time = DateTime.Now,
                    Message = $"Не удалось получить логи. {e.Message}",
                    Type = RecordType.Error
                };
                CreateLog(log);
            }

            return logs;
        }

        private void CreateLog(Log item)
        {
            var repository = new LogRepository(_chatDbContext);
            try
            {
                repository.Create(item);
            }
            catch (Exception e)
            {
                var log = new Log
                {
                    Time = DateTime.Now,
                    Message = $"Не удалось записать лог в бд. {e.Message}",
                    Type = RecordType.Error
                };
                Network.MessageHandlerDelegate($"ОШИБКА!!!! {log.Type}: {log.Message}: {log.Time}");
                //Console.ForegroundColor = ConsoleColor.Red;
                //Console.WriteLine($"ОШИБКА!!!! {log.Type}: {log.Message}: {log.Time}");
                //Console.ResetColor();
                //CreateLog(log);
            }
        }

        private void DeleteUser(User user)
        {
            var repository = new UserRepository(_chatDbContext);
        }
    }
}
