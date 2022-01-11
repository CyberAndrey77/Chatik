using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Common;
using Common.Enums;
using Common.EventArgs;
using WebSocketSharp.Server;

namespace Server
{
    class WsServer
    {
        private readonly IPEndPoint _listenAddress;
        private WebSocketServer _server;
        private readonly ConcurrentDictionary<Guid, WsConnection> _connections;

        public event EventHandler<ConnectStatusChangeEventArgs> ConnectionStatusChanged;
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;

        public WsServer(IPEndPoint listenAddress)
        {
            _listenAddress = listenAddress;
            _connections = new ConcurrentDictionary<Guid, WsConnection>();
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
            _connections.TryAdd(connection.Id, connection);
        }

        public void Stop()
        {
            foreach (var connection in _connections)
            {
                FreeConnection(connection.Key);
                connection.Value.Close();
            }
            _connections.Clear();
            _server?.Stop();
            _server = null;
        }

        public void HandleConnect(Guid id, ConnectionResponse response)
        {
            //поиск поьзователей
            if (_connections.Values.Any(x => x.Login == response.Login))
            {
                SendMessageToClient(new ConnectionRequest(response.Login, ConnectionRequestCode.LoginIsAlreadyTaken, id).GetContainer(), id);
                return;
            }

            if (!_connections.TryGetValue(id, out WsConnection connection))
            {
                return;
            }

            connection.Login = response.Login;
            ConnectionStatusChanged?.Invoke(this, new ConnectStatusChangeEventArgs(response.Login, ConnectionRequestCode.Connect));
            SendMessageAll(new ConnectionRequest(response.Login, ConnectionRequestCode.Connect, id).GetContainer(), id);

            var connectionUsers = new Dictionary<Guid, string>();
            foreach (var user in _connections)
            {
                if (user.Value.Login == null)
                {
                    continue;
                }
                connectionUsers.Add(user.Key, user.Value.Login);
            }
            SendMessageToClient(new ConnectedUser(connectionUsers).GetContainer(), id);
        }

        internal void CreateChat(Guid id, CreateChatResponse createChatResponse)
        {
            //TODO тут занесение в бд происходит

            //TODO ответ клинту
            SendMessageToClient(new CreateChatRequest(createChatResponse.ChatName, createChatResponse.CreatorName,
                DateTime.Now).GetContainer(), createChatResponse.UserIds[0]);



            for (int i = 1; i < createChatResponse.UserIds.Count; i++)
            {
                SendMessageToClient(
                    new CreateChatResponse(createChatResponse.ChatName, createChatResponse.CreatorName,
                        createChatResponse.UserIds, DateTime.Now).GetContainer(), createChatResponse.UserIds[i]);
            }
        }

        private void SendMessageToClient(MessageContainer messageContainer, Guid id)
        {
            if (!_connections.TryGetValue(id, out WsConnection connection))
            {
                return;
            }
            connection.Send(messageContainer);
        }

        public void HandleChatMessage(Guid id, ChatMessageResponse chatMessage)
        {
            SendMessageToClient(new MessageRequest(MessageStatus.Delivered, DateTime.Now, chatMessage.MessageId).GetContainer(), id);

            foreach (var userId in chatMessage.UserIds.Where(userId => userId != id))
            {
                SendMessageToClient(
                    new ChatMessageResponseServer(userId, chatMessage.Message, chatMessage.ChatName, chatMessage.UserIds, DateTime.Now).GetContainer(), userId);
            }
        }

        public void SendMessageAll(MessageContainer container, Guid id)
        {
            foreach (var connect in _connections)
            {
                connect.Value.Send(container);
            }
        }

        public void FreeConnection(Guid id)
        {
            if (_connections.TryRemove(id, out WsConnection connection) && connection.Login != null)
            {
                SendMessageAll(new ConnectionRequest(connection.Login, ConnectionRequestCode.Disconnect, id).GetContainer(), id);
                ConnectionStatusChanged?.Invoke(this, new ConnectStatusChangeEventArgs(connection.Login, ConnectionRequestCode.Disconnect));
            }
        }

        public void HandleMessageToClient(Guid id, PrivateMessageResponseClient privateMessageResponseClient)
        {
            if (!_connections.TryGetValue(id, out WsConnection senderUser))
            {
                return;
            }

            if (!_connections.TryGetValue(privateMessageResponseClient.ReceiverUserId, out WsConnection receiverUser))
            {
                return;
            }

            SendMessageToClient(new MessageRequest(MessageStatus.Delivered, DateTime.Now, privateMessageResponseClient.MessageId).GetContainer(), id);

            MessageReceived?.Invoke(this, new MessageReceivedEventArgs(senderUser.Login, privateMessageResponseClient.Message, receiverUser.Login, DateTime.Now));

            SendMessageToClient(new PrivateMessageResponseServer(privateMessageResponseClient.SenderUserId, privateMessageResponseClient.Message,
                privateMessageResponseClient.ReceiverUserId, DateTime.Now).GetContainer(), receiverUser.Id);
        }
    }
}