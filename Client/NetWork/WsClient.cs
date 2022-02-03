using Common;
using Common.Enums;
using Common.EventArgs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Client.Enums;
using Client.Models;
using Client.NetWork.EventArgs;
using Client.Services.EventArgs;

namespace Client.NetWork
{
    using System;
    using WebSocketSharp;

    public class WsClient : ITransport
    {
        private WebSocket _socket;
        private string _login;
        private ConnectionRequestCode _code;
        private readonly IPackageHelper _packageHelper;
        private readonly Dictionary<EnumKey, List<Action<MessageContainer>>> _events;

        private readonly ConcurrentQueue<MessageContainer> _sendQueue;

        public EventHandler<ConnectStatusChangeEventArgs> ConnectionStatusChanged { get; set; }

        public WsClient(IPackageHelper packageHelper)
        {
            _packageHelper = packageHelper;
            _sendQueue = new ConcurrentQueue<MessageContainer>();
            _code = ConnectionRequestCode.Disconnect;
            _events = new Dictionary<EnumKey, List<Action<MessageContainer>>>();
        }

        public void Connect(string address, int port)
        {
            _socket = new WebSocket($"ws://{address}:{port}");
            _socket.OnOpen += OnOpen;
            _socket.OnClose += OnClose;
            _socket.OnMessage += OnMessage;
            _socket.Connect();
            if (_socket != null)
            {
                _socket.WaitTime = TimeSpan.MaxValue;
            }
        }

        private EnumKey GetKey(string messageIdentifier)
        {
            return _packageHelper.Keys.TryGetValue(messageIdentifier, out var key) ? key : EnumKey.NotRegistration;
        }

        private void OnMessage(object sender, MessageEventArgs e)
        {
            var message = JsonConvert.DeserializeObject<MessageContainer>(e.Data);

            if (message == null)
            {
                return;
            }

            var key = GetKey(message.Identifier);

            if (key == EnumKey.NotRegistration)
            {
                return;
            }

            _events.TryGetValue(key, out var handlersList);
            handlersList?.ForEach(handler => handler(message));
        }

        public void GetLogs(MessageContainer container)
        {
            _sendQueue.Enqueue(container);
            Send();
        }

        public void GetMessages(MessageContainer container)
        {
            _sendQueue.Enqueue(container);
            Send();
        }

        public void SendChatMessage(MessageContainer container)
        {
            _sendQueue.Enqueue(container);
            Send();
        }

        public void CreateChat(MessageContainer container)
        {
            _sendQueue.Enqueue(container);
            Send();
        }

        private void OnClose(object sender, CloseEventArgs e)
        {
            ConnectionStatusChanged?.Invoke(this, new ConnectStatusChangeEventArgs(_login,
                _code == ConnectionRequestCode.Disconnect ? ConnectionRequestCode.Disconnect : ConnectionRequestCode.LoginIsAlreadyTaken));

            _socket.OnOpen -= OnOpen;
            _socket.OnClose -= OnClose;
            _socket.OnMessage -= OnMessage;
            _socket = null;
        }

        private void OnOpen(object sender, System.EventArgs e)
        {
            //ConnectionStatusChanged?.Invoke(this, new ConnectStatusChangeEventArgs(_login, ConnectionRequestCode.Connect));
        }

        public void Disconnect()
        {
            Unsubscribe();
            _socket?.Close();
        }

        public void Login(string login)
        {
            _login = login;
            _sendQueue.Enqueue(new ConnectionResponse(_login).GetContainer());
            Send();
        }

        private void Send()
        {
            if (Equals(!_sendQueue.TryDequeue(out MessageContainer message)))
            {
                return;
            }

            if (_socket == null)
            {
                return;
            }

            string serializedMessages = JsonConvert.SerializeObject(message);

            //if (_socket.IsAlive)
            //{
            //    _socket.SendAsync(serializedMessages, SendCompleted);
            //}
            _socket.SendAsync(serializedMessages, SendCompleted);
        }

        private void SendCompleted(bool completed)
        {
            if (!completed)
            {
                //MessageRequestEvent?.Invoke(this, new MessageRequestEvent(MessageStatus.NotDelivered, DateTime.Now));
            }
        }

        public void Subscribe(EnumKey key, Action<MessageContainer> handler)
        {
            _events.TryGetValue(key, out var handlerList);
            if (handlerList == null)
            {
                _events.Add(key, new List<Action<MessageContainer>>(){ handler });
            }
            else
            {
                handlerList.Add(handler);
            }
        }

        public void Unsubscribe()
        {
            _events.Clear();
        }
    }
}
