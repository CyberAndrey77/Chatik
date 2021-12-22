using Common.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Common;
using Common.Enums;

namespace Server
{
    public class Network
    {
        public delegate void MessageHandler(string message);

        private MessageHandler _messageHandler;
        private readonly int _port;
        private WsServer _server;
        
        internal void StartSever(MessageHandler messageHandler)
        {
            _messageHandler = messageHandler;
            _server = new WsServer(new IPEndPoint(IPAddress.Any, _port));
            _messageHandler(_server.Start());
            _server.ConnectionStatusChanged += OnConnection;
            _server.MessageReceived += OnMessage;
        }

        public void StopServer()
        {
            _server.Stop();
        }

        public Network(Config config)
        {
            _port = config.Port;
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
            string connect = e.ConnectionRequestCode == ConnectionRequestCode.Connect ? "Подключился" : "Отключился";
            string message = $"{DateTime.Now}: {connect} клиент {e.Name}";
            _messageHandler(message);
        }
    }
}
