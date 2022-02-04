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
using WebSocketSharp;

namespace Client.Services
{
    public class ConnectionService : IConnectionService
    {
        //private WsClient _wsClient;
        private readonly ITransport _transport; 
        private readonly ILogger _logger;
        
        public EventHandler<GetUsersEventArgs> UserListEvent { get; set; }
        public EventHandler<GetUserEventArgs> UserEvent { get; set; }

        public EventHandler<ConnectStatusChangeEventArgs> ConnectStatusChangeEvent { get; set; }

        public string Name { get; set; }
        public int Id{ get; set; }
        public string IpAddress { get; set; }
        public int Port { get; set; }

        public ConnectionService(ITransport transport)
        {
            _transport = transport;
            _transport.ConnectionStatusChanged += OnConnectionChange;
            _logger = LogManager.GetCurrentClassLogger();
            _transport.Subscribe(EnumKey.ConnectionKeyConnection, OnConnectionChange);
            _transport.Subscribe(EnumKey.ConnectionKeyConnectedUser, OnGetConnectedUser);
        }

        private void OnConnectionChange(object sender, CloseEventArgs e)
        {
            ConnectStatusChangeEvent?.Invoke(this, new ConnectStatusChangeEventArgs(Id, Name, (ConnectionRequestCode)e.Code));
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
                return;
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
                return;
            }

            if (Name == connectionRequest.Login)
            {
                ConnectStatusChangeEvent?.Invoke(this, new ConnectStatusChangeEventArgs(connectionRequest.Id, connectionRequest.Login, connectionRequest.CodeConnected));
            }
            else
            {
                UserEvent?.Invoke(this, new GetUserEventArgs(connectionRequest.Login, connectionRequest.CodeConnected == ConnectionRequestCode.Connect, connectionRequest.Id));
            }
        }

        public void ConnectToServer()
        {
            _transport.Connect(IpAddress, Port);
            _transport.Login(Name);
        }

        public void Disconnect()
        {
            _transport.Disconnect();
        }
    }
}