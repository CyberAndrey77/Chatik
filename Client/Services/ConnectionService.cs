using System;
using System.Collections.Generic;
using System.Text;
using Client.Enums;
using Client.Models;
using Client.NetWork;
using Client.NetWork.EventArgs;
using Client.Services.EventArgs;
using Common;
using Common.Enums;
using Common.EventArgs;
using Newtonsoft.Json.Linq;
using NLog;

namespace Client.Services
{
    public class ConnectionService : IConnectionService
    {
        //private WsClient _wsClient;
        private readonly ITransport _transport; 
        private readonly ILogger _logger;

        public EventHandler<ConnectionEventArgs> ConnectionEvent { get; set; }
        public EventHandler<MessageEventArgs> MessageEvent { get; set; }
        public EventHandler<GetUsersEventArgs> UserListEvent { get; set; }
        public EventHandler<GetUserEventArgs> UserEvent { get; set; }
        public EventHandler<ChatMessageEventArgs> GetPrivateMessageEvent { get; set; }
        public EventHandler<ChatEventArgs> ChatCreated { get; set; }
        public EventHandler<ChatMessageEventArgs> ChatMessageEvent { get; set; }
        public EventHandler<ChatEventArgs> ChatIsCreatedEvent { get; set; }

        public EventHandler<UserChatEventArgs<Chat>> GetUserChats { get; set; }

        public EventHandler<GetMessagesEventArgs<Message>> GetMessagesEvent { get; set; }

        public EventHandler<ConnectStatusChangeEventArgs> ConnectStatusChangeEvent { get; set; }

        public string Name { get; set; }
        public int Id{ get; set; }
        public string IpAddress { get; set; }
        public int Port { get; set; }

        public ConnectionService(ITransport transport)
        {
            _transport = transport;
            _logger = LogManager.GetCurrentClassLogger();
            _transport.Subscribe(EnumKey.ConnectionKeyConnection, OnConnectionChange);
            _transport.Subscribe(EnumKey.ConnectionKeyConnectedUser, OnGetConnectedUser);
        }

        private void OnGetConnectedUser(MessageContainer message)
        {
            if (message.Identifier != nameof(ConnectedUser))
            {
                return;
            }
            var connectedUser = ((JObject)message.Payload).ToObject(typeof(ConnectedUser)) as ConnectedUser;
            if (connectedUser == null)
            {
                _logger.Error($"Answer from server {message}:{message.Identifier} is null");
            }
            UserListEvent?.Invoke(this, new GetUsersEventArgs(connectedUser.Users));
        }

        private void OnConnectionChange(MessageContainer message)
        {
            if (message.Identifier != nameof(ConnectionRequest))
            {
                return;
            }
            var connectionRequest = ((JObject)message.Payload).ToObject(typeof(ConnectionRequest)) as ConnectionRequest;
            if (connectionRequest == null)
            {
                _logger.Error($"Answer from server {message}:{message.Identifier} is null");
            }
            ConnectStatusChangeEvent?.Invoke(this, new ConnectStatusChangeEventArgs(connectionRequest.Id, connectionRequest.Login, connectionRequest.CodeConnected));
        }

        public void ConnectToServer()
        {
            _transport.Connect(IpAddress, Port);
            _transport.Login(Name);
        }

        private void OnGetUserId(object sender, UserIdEventArgs e)
        {
            Id = e.UserId;
        }

        public void Disconnect()
        {
            _transport.Disconnect();
            //if (_wsClient == null)
            //{
            //    return;
            //}
            //_wsClient.Disconnect();
            //_wsClient.ConnectionStatusChanged -= OnConnectionChange;
            //_wsClient.MessageReceived -= OnGetMessage;
            //_wsClient.UsersTaken -= OnUsersTaken;
            //_wsClient.UserEvent -= OnUserStatusChange;
            //_wsClient = null;
        }

        private void OnConnectionChange(object sender, ConnectStatusChangeEventArgs e)
        {
            switch (e.ConnectionRequestCode)
            {
                case ConnectionRequestCode.Connect:
                    ConnectionEvent?.Invoke(this, new ConnectionEventArgs(true, "Успешное подключение"));
                    break;
                case ConnectionRequestCode.Disconnect:
                    ConnectionEvent?.Invoke(this, new ConnectionEventArgs(false, "Соединение потеряно"));
                    break;
                case ConnectionRequestCode.LoginIsAlreadyTaken:
                    ConnectionEvent?.Invoke(this, new ConnectionEventArgs(false, "Логин уже занят"));
                    break;
            }
        }

        private void OnUsersTaken(object sender, NetWork.UsersTakenEventArgs e)
        {
            UserListEvent?.Invoke(this, new GetUsersEventArgs(e.Users));
        }
        
        private void OnUserStatusChange(object sender, UserStatusChangeEventArgs e)
        {
            UserEvent?.Invoke(this, new GetUserEventArgs(e.UserName, e.IsConnect, e.Id));
        }
    }
}