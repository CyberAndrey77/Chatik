﻿using Common.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Common;
using Common.Enums;
using Server.EventArgs;
using Server.Models;

namespace Server
{
    public class Network
    {
        public delegate void MessageHandler(string message);

        public WsServer Server { get; set; }
        
        public EventHandler<ConnectStatusChangeEventArgs> ConnectionEvent;
        public EventHandler<UserChatEventArgs<Chat>> GetUserChats;
        public event EventHandler<ChatMessageEventArgs> ChatMessageEvent;
        public event EventHandler<GetMessagesEventArgs<Message>> GetMessageEvent;
        public event EventHandler<CreateChatEventArgs> CreateChatEvent;
        public event EventHandler<LogEventArgs<Log>> GetLogsEvent;
        public event EventHandler<UserDataEventArgs> GetAllUsersEvent;

        private MessageHandler _messageHandler;
        private readonly int _port;
        
        internal void StartSever(MessageHandler messageHandler)
        {
            _messageHandler = messageHandler;
            Server = new WsServer(new IPEndPoint(IPAddress.Any, _port));
            _messageHandler(Server.Start());
            Server.ConnectionStatusChanged += OnConnection;
            Server.GetUserChats += OnGetUserChats;
            Server.MessageReceived += OnMessage;
            Server.ChatMessageEvent += OnChatMessageEvent;
            Server.GetMessageEvent += OnGetMessageEvent;
            Server.CreateChatEvent += OnChatCreateEvent;
            Server.GetLogsEvent += OnGetLogs;
            Server.GetAllUsersEvent += OnGetAllUsers;
        }

        private void OnGetAllUsers(object sender, UserDataEventArgs e)
        {
            GetAllUsersEvent?.Invoke(this, e);
        }

        private void OnGetLogs(object sender, LogEventArgs<Log> e)
        {
            GetLogsEvent?.Invoke(this, e);
        }

        private void OnChatCreateEvent(object sender, CreateChatEventArgs e)
        {
            CreateChatEvent?.Invoke(this, e);
        }

        private void OnGetMessageEvent(object sender, GetMessagesEventArgs<Message> e)
        {
            GetMessageEvent?.Invoke(this, e);
        }

        private void OnChatMessageEvent(object sender, ChatMessageEventArgs e)
        {
            ChatMessageEvent?.Invoke(this, e);
        }

        private void OnGetUserChats(object sender, UserChatEventArgs<Chat> e)
        {
            GetUserChats?.Invoke(this, e);
        }

        public void StopServer()
        {
            Server.Stop();
        }

        public Network(int port)
        {
            _port = port;
        }

        private void OnMessage(object sender, MessageReceivedEventArgs e)
        {
            if (e.ReceiverName == string.Empty)
            {
                _messageHandler($"{e.Time}: {e.SenderName}: {e.Message}");
            }
            _messageHandler($"{e.Time}: {e.SenderName}: {e.Message}: {e.ReceiverName}");
        }

        private void OnConnection(object sender, ConnectStatusChangeEventArgs e)
        {
            ConnectionEvent?.Invoke(this, e);
            string connect = e.ConnectionRequestCode == ConnectionRequestCode.Connect ? "Подключился" : "Отключился";
            string message = $"{DateTime.Now}: {connect} клиент {e.Name}";
            _messageHandler(message);
        }
    }
}
