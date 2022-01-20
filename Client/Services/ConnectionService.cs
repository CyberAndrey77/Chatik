﻿using System;
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
        public EventHandler<ChatMessageEventArgs> GetPrivateMessageEvent { get; set; }
        public EventHandler<ChatEventArgs> ChatCreated { get; set; }
        public EventHandler<ChatMessageEventArgs> ChatMessageEvent { get; set; }
        public EventHandler<ChatEventArgs> ChatIsCreatedEvent { get; set; }

        public EventHandler<UserChatEventArgs<Chat>> GetUserChats { get; set; }

        public EventHandler<GetMessagesEventArgs<Message>> GetMessagesEvent { get; set; }

        public string Name { get; set; }
        public int Id{ get; set; }
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
            _wsClient.GetUserIdEvent += OnGetUserId;
            _wsClient.ChatIsCreated += ChatIsCreated;
            _wsClient.GetUserChats += GetChats;
            _wsClient.GetMessagesEvent += OnGetMessages;
            _wsClient.Connect(IpAddress, Port);
            _wsClient.Login(Name);
        }

        private void OnGetMessages(object sender, GetMessagesEventArgs<Message> e)
        {
            GetMessagesEvent?.Invoke(this, e);
        }

        private void GetChats(object sender, UserChatEventArgs<Chat> e)
        {
            GetUserChats?.Invoke(this, e);
        }

        private void ChatIsCreated(object sender, ChatEventArgs e)
        {
            ChatIsCreatedEvent?.Invoke(this, e);
        }

        private void OnGetUserId(object sender, UserIdEventArgs e)
        {
            Id = e.UserId;
        }

        private void OnChatMessage(object sender, ChatMessageEventArgs e)
        {
            ChatMessageEvent?.Invoke(this, e);
        }

        private void OnChatCreated(object sender, ChatEventArgs e)
        {
            ChatCreated?.Invoke(this, e);
        }

        private void GetPrivateMessage(object sender, ChatMessageEventArgs e)
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
            UserEvent?.Invoke(this, new GetUserEventArgs(e.UserName, e.IsConnect, e.Id));
        }
        
        private void OnMessageStatusChange(object sender, MessageRequestEvent e)
        {
            MessageStatusChangeEvent?.Invoke(this, e);
        }

        public void CreateChat(string chatName, int chatId, string creator, List<int> invented, bool isDialog)
        {
            _wsClient.CreateChat(chatName, chatId, creator, invented, isDialog);
        }

        public void SendPrivateMessage(int senderUserId, string message, int chatId, List<int> userIds)
        {
            _wsClient.SendPrivateMessage(senderUserId, message, chatId, userIds);
        }

        public void SendChatMessage(int name, string text, int chatId, List<int> userIds, bool isDialog)
        {
            _wsClient.SendChatMessage(name, text, chatId, userIds, isDialog);
        }

        public void GetMessages(int chatId)
        {
            _wsClient.GetMessage(chatId);
        }
    }
}