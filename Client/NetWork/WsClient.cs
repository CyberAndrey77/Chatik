using Client.Enums;
using Common;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Client.NetWork
{
    using System;
    using WebSocketSharp;

    public class WsClient : ITransport
    {
        private WebSocket _socket;
        private string _login;
        private readonly IPackageHelper _packageHelper;
        private readonly Dictionary<EnumKey, List<Action<MessageContainer>>> _events;

        private readonly ConcurrentQueue<MessageContainer> _sendQueue;

        public EventHandler<CloseEventArgs> ConnectionStatusChanged { get; set; }

        public WsClient(IPackageHelper packageHelper)
        {
            _packageHelper = packageHelper;
            _sendQueue = new ConcurrentQueue<MessageContainer>();
            _events = new Dictionary<EnumKey, List<Action<MessageContainer>>>();
        }

        public void Connect(string address, int port)
        {
            _socket = new WebSocket($"ws://{address}:{port}");
            _socket.OnClose += OnClose;
            _socket.OnMessage += OnMessage;
            _socket.WaitTime = TimeSpan.FromSeconds(200);
            _socket.Connect();
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

        public void SendRequest(MessageContainer container)
        {
            _sendQueue.Enqueue(container);
            Send();
        }

        private void OnClose(object sender, CloseEventArgs e)
        {
            ConnectionStatusChanged?.Invoke(this, e);
            _socket.OnClose -= OnClose;
            _socket.OnMessage -= OnMessage;
            _socket = null;
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
            _socket.SendAsync(serializedMessages, SendCompleted);
        }

        private void SendCompleted(bool completed)
        {
        }

        public void Subscribe(EnumKey key, Action<MessageContainer> handler)
        {
            _events.TryGetValue(key, out var handlerList);
            if (handlerList == null)
            {
                _events.Add(key, new List<Action<MessageContainer>>() { handler });
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
