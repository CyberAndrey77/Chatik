using Common.Enums;
using Common.EventArgs;
using Server.EventArgs;
using Server.Models;
using System;
using System.Net;

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

        public MessageHandler MessageHandlerDelegate;

        public void StartSever(int time, int port)
        {
            Server = new WsServer(new IPEndPoint(IPAddress.Any, port));
            MessageHandlerDelegate(Server.Start(time));
            Server.ConnectionStatusChanged += OnConnection;
            Server.GetUserChats += OnGetUserChats;
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

        private void OnConnection(object sender, ConnectStatusChangeEventArgs e)
        {
            ConnectionEvent?.Invoke(this, e);
            string connect = e.ConnectionRequestCode == ConnectionRequestCode.Connect ? "Подключился" : "Отключился";
            string message = $"{DateTime.Now}: {connect} клиент {e.Name}\n";
            MessageHandlerDelegate(message);
        }
    }
}
