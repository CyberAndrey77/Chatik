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
                SendMessageToClient(new ConnectionRequest(response.Login, ConnectionRequestCode.LoginIsAlreadyTaken).GetContainer(), id);
                return;
            }

            if (!_connections.TryGetValue(id, out WsConnection connection))
            {
                return;
            }

            connection.Login = response.Login;
            ConnectionStatusChanged?.Invoke(this, new ConnectStatusChangeEventArgs(response.Login, ConnectionRequestCode.Connect));
            SendMessageAll(new ConnectionRequest(response.Login, ConnectionRequestCode.Connect).GetContainer(), id);

            var connectionUsers = new List<string>();
            foreach (var user in _connections)
            {
                if (user.Value.Login == null)
                {
                    continue;
                }
                connectionUsers.Add(user.Value.Login);
            }
            SendMessageToClient(new ConnectedUser(connectionUsers).GetContainer(), id);
        }

        internal void CreateDialog(CreateDialogResponse createDialogResponse)
        {
            
        }

        private void SendMessageToClient(MessageContainer messageContainer, Guid id)
        {
            if (!_connections.TryGetValue(id, out WsConnection connection))
            {
                return;
            }
            connection.Send(messageContainer);
        }

        public void HandleMessage(Guid id, ClientMessageResponse clientMessage)
        {
            //if (!_connections.TryGetValue(id, out WsConnection connection))
            //{
            //    return;
            //}

            //SendMessageToClient(new MessageRequest(MessageStatus.Delivered, DateTime.Now, clientMessage.MessageId).GetContainer(), id);

            //MessageReceived?.Invoke(this, new MessageReceivedEventArgs(connection.Login, clientMessage.Message, DateTime.Now));

            //SendMessageAll(new ServerMessageResponse(clientMessage.SenderName, clientMessage.Message, DateTime.Now).GetContainer(), id);
        }

        public void SendMessageAll(MessageContainer container, Guid id)
        {
            foreach (var connect in _connections)
            {
                if (connect.Key == id)
                {
                    continue;
                }
                connect.Value.Send(container);
            }
        }

        public void FreeConnection(Guid id)
        {
            if (_connections.TryRemove(id, out WsConnection connection) && connection.Login != null)
            {
                SendMessageAll(new ConnectionRequest(connection.Login, ConnectionRequestCode.Disconnect).GetContainer(), id);
                ConnectionStatusChanged?.Invoke(this, new ConnectStatusChangeEventArgs(connection.Login, ConnectionRequestCode.Disconnect));
            }
        }

        public void HandleMessageToClient(Guid id, PrivateMessageResponseClient privateMessageResponseClient)
        {
            if (!_connections.TryGetValue(id, out WsConnection connection))
            {
                return;
            }

            SendMessageToClient(new MessageRequest(MessageStatus.Delivered, DateTime.Now, privateMessageResponseClient.MessageId).GetContainer(), id);

            MessageReceived?.Invoke(this, new MessageReceivedEventArgs(connection.Login, privateMessageResponseClient.Message, privateMessageResponseClient.ReceiverName, DateTime.Now));

            var receiver = _connections.First(x => x.Value.Login == privateMessageResponseClient.ReceiverName);

            SendMessageToClient(new PrivateMessageResponseServer(privateMessageResponseClient.SenderName, privateMessageResponseClient.Message, privateMessageResponseClient.ReceiverName, DateTime.Now).GetContainer(), receiver.Key);
        }
    }
}