using Common.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Common;
using Common.Enums;
using Server.Models;

namespace Server
{
    public class Network
    {
        public delegate void MessageHandler(string message);

        public WsServer Server { get; set; }

        public EventHandler<ConnectStatusChangeEventArgs> ConnectionEvent;
        public EventHandler<UserChatEventArgs<Chat>> GetUserChats;

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
