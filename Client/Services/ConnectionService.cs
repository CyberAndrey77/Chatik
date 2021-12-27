using System;
using System.Collections.Generic;
using System.Text;
using Client.Models;
using Client.NetWork;
using Client.NetWork.EventArgs;
using Client.Services.EventArgs;
using Common;
using Common.Enums;
using Common.EventArgs;

namespace Client.Services
{
    public class ConnectionService : IConnectionService
    {
        private WsClient _wsClient;

        public EventHandler<ConnectionEventArgs> ConnectionEvent { get; set; }

        public EventHandler<MessageEventArgs> MessageEvent { get; set; }

        public EventHandler<GetUsersEventArgs> UserListEvent { get; set; }
        public EventHandler<GetUserEventArgs> UserEvent { get; set; }

        public EventHandler<MessageRequestEvent> MessageStatusChangeEvent { get; set; }

        public EventHandler<PrivateMessageEventArgs> GetPrivateMessageEvent { get; set; }
        public EventHandler<ChatEventArgs> ChatCreated { get; set; }
        public EventHandler<ChatMessageEventArgs> ChatMessageEvent { get; set; }

        public string Name { get; set; }
        public string IpAddress { get; set; }
        public int Port { get; set; }

        public void ConnectToServer()
        {
            _wsClient = new WsClient();
            _wsClient.ConnectionStatusChanged += HasConnected;
            _wsClient.MessageReceived += OnGetMessage;
            _wsClient.UsersTaken += OnUsersTaken;
            _wsClient.UserEvent += OnUserStatusChange;
            _wsClient.MessageRequestEvent += OnMessageStatusChange;
            _wsClient.PrivateMessageEvent += GetPrivateMessage;
            _wsClient.CreatedChat += OnChatCreated;
            _wsClient.ChatMessageEvent += OnChatMessage;
            _wsClient.Connect(IpAddress, Port);
            _wsClient.Login(Name);
        }

        private void OnChatMessage(object sender, ChatMessageEventArgs e)
        {
            ChatMessageEvent?.Invoke(this, e);
        }

        private void OnChatCreated(object sender, ChatEventArgs e)
        {
            ChatCreated?.Invoke(this, e);
        }

        private void GetPrivateMessage(object sender, PrivateMessageEventArgs e)
        {
            GetPrivateMessageEvent?.Invoke(this, e);
        }

        public void Disconnect()
        {
            if (_wsClient == null)
            {
                return;
            }
            _wsClient.Disconnect();
            _wsClient.ConnectionStatusChanged -= HasConnected;
            _wsClient.MessageReceived -= OnGetMessage;
            _wsClient.UsersTaken -= OnUsersTaken;
            _wsClient.UserEvent -= OnUserStatusChange;
            _wsClient = null;
        }

        public void SendMessage(string name, string message)
        {
            _wsClient.SendMessage(name, message);
        }

        private void HasConnected(object sender, ConnectStatusChangeEventArgs e)
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

        private void OnGetMessage(object sender, MessageReceivedEventArgs e)
        {
            MessageEvent?.Invoke(this, new MessageEventArgs(e.SenderName, e.Message, e.Time));
        }

        private void OnUsersTaken(object sender, NetWork.UsersTakenEventArgs e)
        {
            UserListEvent?.Invoke(this, new GetUsersEventArgs(e.Users));
        }
        
        private void OnUserStatusChange(object sender, UserStatusChangeEventArgs e)
        {
            UserEvent?.Invoke(this, new GetUserEventArgs(e.UserName, e.IsConnect));
        }
        
        private void OnMessageStatusChange(object sender, MessageRequestEvent e)
        {
            MessageStatusChangeEvent?.Invoke(this, e);
        }

        public void CreateChat(string chatName, string creator, List<string> invented)
        {
            _wsClient.CreateChat(chatName, creator, invented);
        }

        public void SendPrivateMessage(string senderName, string message, string receiverName)
        {
            _wsClient.SendPrivateMessage(senderName, message, receiverName);
        }

        public void SendChatMessage(string name, string text, string chatName, List<string> users)
        {
            _wsClient.SendChatMessage(name, text, chatName, users);
        }
    }
}