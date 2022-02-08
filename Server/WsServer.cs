using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using Common;
using Common.Enums;
using Common.EventArgs;
using Server.EventArgs;
using Server.Models;
using WebSocketSharp.Server;

namespace Server
{
    public class WsServer
    {
        private readonly IPEndPoint _listenAddress;
        private WebSocketServer _server;
        internal readonly ConcurrentDictionary<int, WsConnection> Connections;

        public event EventHandler<ConnectStatusChangeEventArgs> ConnectionStatusChanged;
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;
        public event EventHandler<UserChatEventArgs<Chat>> GetUserChats;
        public event EventHandler<ChatMessageEventArgs> ChatMessageEvent;
        public event EventHandler<CreateChatEventArgs> CreateChatEvent;
        public event EventHandler<GetMessagesEventArgs<Message>> GetMessageEvent;
        public event EventHandler<LogEventArgs<Log>> GetLogsEvent;
        public event EventHandler<UserDataEventArgs> GetAllUsersEvent;

        public WsServer(IPEndPoint listenAddress)
        {
            _listenAddress = listenAddress;
            Connections = new ConcurrentDictionary<int, WsConnection>();
        }

        public string Start()
        {
            _server = new WebSocketServer(_listenAddress.Address, _listenAddress.Port, false);
            _server.AddWebSocketService<WsConnection>("/", client => { client.AddServer(this); });
            _server.Start();
            _server.WaitTime = TimeSpan.MaxValue;
            return $"Сервер запущен IP: {_listenAddress.Address}, Port: {_listenAddress.Port}";
        }

        public void AddConnection(WsConnection connection)
        {
            Connections.TryAdd(connection.Id, connection);
        }

        public void Stop()
        {
            foreach (var connection in Connections)
            {
                //FreeConnection(connection.Key);
                connection.Value.Close( ConnectionRequestCode.Disconnect);
            }
            Connections.Clear();
            _server?.Stop();
            _server = null;
        }

        public bool HandleConnect(int id, ConnectionResponse response)
        {
            //поиск поьзователей
            if (Connections.Values.Any(x => x.Login == response.Login))
            {
                //SendMessageToClient(new ConnectionRequest(response.Login, ConnectionRequestCode.LoginIsAlreadyTaken, id).GetContainer(), id);
                return false;
            }

            if (!Connections.TryGetValue(id, out WsConnection connection))
            {
                return false;
            }

            connection.Login = response.Login;
            ConnectionStatusChanged?.Invoke(this, new ConnectStatusChangeEventArgs(connection.Id, response.Login, ConnectionRequestCode.Connect));
            GetAllUsersEvent?.Invoke(this, new UserDataEventArgs(connection.Id));

            var connectionUsers = new Dictionary<int, string>();

            foreach (var user in Connections)
            {
                if (user.Value.Login == null)
                {
                    continue;
                }
                connectionUsers.Add(user.Key, user.Value.Login);
            }
            SendMessageToClient(new ConnectedUser(connectionUsers).GetContainer(), connection.Id);

            var userChatsEvent = new UserChatEventArgs<Chat>(new List<Chat>(), connection.Id);
            GetUserChats?.Invoke(this, userChatsEvent);
            return true;
        }

        internal void CreateChat(int id, CreateChatResponse createChatResponse)
        {
            var chatEvent = new CreateChatEventArgs(id, createChatResponse.ChatName, createChatResponse.UserIds,
                createChatResponse.IsDialog);
            CreateChatEvent?.Invoke(this, chatEvent);
        }

        internal void GetLogs(int id, GetLogsResponse<Log> logs)
        {
            var logEvent = new LogEventArgs<Log>
            {
                UserId = id,
                Start = logs.Start,
                End = logs.End,
                Type = logs.Type
            };
            GetLogsEvent?.Invoke(this, logEvent);
        }

        internal void GetMessages(int userId, int chatId)
        {
            var getMessageEvent = new GetMessagesEventArgs<Message>(chatId)
            {
                SenderId = userId
            };
            GetMessageEvent?.Invoke(this, getMessageEvent);
        }

        internal void SendMessageToClient(MessageContainer messageContainer, int id)
        {
            if (!Connections.TryGetValue(id, out WsConnection connection))
            {
                return;
            }
            connection.Send(messageContainer);
        }

        public void HandleChatMessage(int id, ChatMessageResponse chatMessage)
        {
            var chatEvent = new ChatMessageEventArgs(id, chatMessage.Message, chatMessage.ChatId, chatMessage.UserIds, chatMessage.IsDialog, chatMessage.MessageId);
            ChatMessageEvent?.Invoke(this, chatEvent);
        }

        public void SendMessageAll(MessageContainer container, int id)
        {
            foreach (var connect in Connections)
            {
                connect.Value.Send(container);
            }
        }

        public void FreeConnection(int id, ConnectionRequestCode reason)
        {
            if (Connections.TryRemove(id, out WsConnection connection) && connection.Login != null)
            {
                SendMessageAll(new ConnectionRequest(connection.Login, reason, connection.Id).GetContainer(), connection.Id);
                ConnectionStatusChanged?.Invoke(this, new ConnectStatusChangeEventArgs(connection.Login, reason));
            }
            //if (Connections.TryGetValue(id, out WsConnection connection) && connection.Login != null)
            //{
            //    SendMessageAll(new ConnectionRequest(connection.Login, reason, connection.Id).GetContainer(), connection.Id);
            //    ConnectionStatusChanged?.Invoke(this, new ConnectStatusChangeEventArgs(connection.Login, reason));
            //    Connections.TryRemove(id, out connection);
            //}
            //Thread.Sleep(1000);
        }
    }
}