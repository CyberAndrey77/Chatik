using Client.Enums;
using Client.NetWork;
using Client.Services.EventArgs;
using Common;
using Common.Enums;
using Common.EventArgs;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Threading.Tasks;
using WebSocketSharp;

namespace Client.Services
{
    public class ConnectionService : IConnectionService
    {
        private readonly ITransport _transport;
        private readonly ILogger _logger;

        public EventHandler<GetUsersEventArgs> GetOnlineUsers { get; set; }
        public EventHandler<GetUsersEventArgs> AllUsersEvent { get; set; }
        public EventHandler<GetUserEventArgs> UserEvent { get; set; }

        public EventHandler<ConnectStatusChangeEventArgs> ConnectStatusChangeEvent { get; set; }

        public string Name { get; set; }
        public int Id { get; set; }
        public string IpAddress { get; set; }
        public int Port { get; set; }

        public ConnectionService(ITransport transport)
        {
            _transport = transport;
            _transport.ConnectionStatusChanged += OnConnectionChange;
            _logger = LogManager.GetCurrentClassLogger();

        }

        private void OnAllUser(MessageContainer message)
        {
            if (message.Identifier != nameof(GetAllUsers))
            {
                return;
            }
            var allUsers = ((JObject)message.Payload).ToObject(typeof(GetAllUsers)) as GetAllUsers;
            if (allUsers == null)
            {
                _logger.Error($"Answer from server {message}:{message.Identifier} is null");
                return;
            }
            AllUsersEvent?.Invoke(this, new GetUsersEventArgs(allUsers.Users));
        }

        private void OnConnectionChange(object sender, CloseEventArgs e)
        {
            ConnectStatusChangeEvent?.Invoke(this, new ConnectStatusChangeEventArgs(Id, Name, (ConnectionRequestCode)e.Code));
        }

        private void OnGetOnlineUsers(MessageContainer message)
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
            GetOnlineUsers?.Invoke(this, new GetUsersEventArgs(connectedUser.Users));
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

        public async void ConnectToServer()
        {
            await Task.Run(() => _transport.Connect(IpAddress, Port));
            _transport.Login(Name);
            _transport.Subscribe(EnumKey.ConnectionKeyConnection, OnConnectionChange);
            _transport.Subscribe(EnumKey.ConnectionKeyOnlineUsers, OnGetOnlineUsers);
            _transport.Subscribe(EnumKey.ConnectionKeyAllUsers, OnAllUser);
        }

        public void Disconnect()
        {
            _transport.Disconnect();
        }
    }
}