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
        PackageHelper _packageHelper;
        private readonly Dictionary<EnumKey, List<Action<MessageContainer>>> _events;

        private readonly ConcurrentQueue<MessageContainer> _sendQueue;

        private MethodHandler _methodHandler;
        //public delegate void MethodHandler();

        public EventHandler<ConnectStatusChangeEventArgs> ConnectionStatusChanged;
        public EventHandler<UserIdEventArgs> GetUserIdEvent;
        public EventHandler<MessageReceivedEventArgs> MessageReceived;
        public EventHandler<UsersTakenEventArgs> UsersTaken;
        public EventHandler<UserStatusChangeEventArgs> UserEvent;
        public EventHandler<MessageRequestEvent> MessageRequestEvent;
        public EventHandler<ChatMessageEventArgs> PrivateMessageEvent;
        public EventHandler<ChatMessageEventArgs> ChatMessageEvent;
        public EventHandler<UserChatEventArgs<Chat>> GetUserChats;
        public EventHandler<GetMessagesEventArgs<Message>> GetMessagesEvent;
        public EventHandler<LogEventArgs<Log>> GetLogsEvent;

        public WsClient()
        {
            _sendQueue = new ConcurrentQueue<MessageContainer>();
            _code = ConnectionRequestCode.Disconnect;
            _events = new Dictionary<EnumKey, List<Action<MessageContainer>>>();
            _packageHelper = new PackageHelper();
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
            
            //switch (message.Identifier)
            //{
            //    case nameof(ConnectionRequest):
            //        var connectionRequest = ((JObject)message.Payload).ToObject(typeof(ConnectionRequest)) as ConnectionRequest;
            //        if (connectionRequest == null)
            //        {
            //            throw new ArgumentNullException();
            //        }

            //        string answer = string.Empty;

            //        _code = connectionRequest.CodeConnected;
            //        switch (_code)
            //        {
            //            case ConnectionRequestCode.Connect:
            //                answer = "подключился";
            //                if (_login != connectionRequest.Login)
            //                {
            //                    UserEvent?.Invoke(this, new UserStatusChangeEventArgs(connectionRequest.Login, true, connectionRequest.Id));
            //                }
            //                break;
            //            case ConnectionRequestCode.Disconnect:
            //                answer = "отключился";
            //                if (_login != connectionRequest.Login)
            //                {
            //                    UserEvent?.Invoke(this, new UserStatusChangeEventArgs(connectionRequest.Login, false, connectionRequest.Id));
            //                }
            //                break;
            //            case ConnectionRequestCode.LoginIsAlreadyTaken:
            //                answer = "Логин уже занят";
            //                _socket.Close();
            //                return;
            //        }

            //        if (_login == connectionRequest.Login)
            //        {
            //            GetUserIdEvent?.Invoke(this, new UserIdEventArgs(connectionRequest.Id));
            //            ConnectionStatusChanged?.Invoke(this, new ConnectStatusChangeEventArgs(_login, ConnectionRequestCode.Connect));
            //            break;
            //        }
            //        // TODO Поменять на логичное событие
            //        MessageReceived?.Invoke(this, new MessageReceivedEventArgs(connectionRequest.Login, $"{connectionRequest.Login} {answer}"));
            //        break;
            //    case nameof(ServerMessageResponse):
            //        var messageResponse = ((JObject)message.Payload).ToObject(typeof(ServerMessageResponse)) as ServerMessageResponse;
            //        if (messageResponse == null)
            //        {
            //            throw new ArgumentNullException();
            //        }

            //        MessageReceived?.Invoke(this, new MessageReceivedEventArgs(messageResponse.Name, messageResponse.Message, messageResponse.Time));
            //        break;
            
            //   
            //    default:
            //        throw new ArgumentNullException();
            //}

        }

        public void GetLogs(int selectType, DateTime starTime, DateTime endTime)
        {
            _sendQueue.Enqueue(new GetLogsResponse<Log>(selectType, starTime, endTime).GetContainer());
            Send();
        }

        public void GetMessages(int chatId)
        {
            _sendQueue.Enqueue(new GetMessageResponse(chatId).GetContainer());
            Send();
        }

        public void SendChatMessage(int senderUserId, string text, int chatId, List<int> users, bool isDialog)
        {
            _sendQueue.Enqueue(new ChatMessageResponse(senderUserId, text, chatId, users, isDialog).GetContainer());
            Send();
        }

        public void CreateChat(string chatName, int chatId, string creator, List<int> users, bool isDialog)
        {
            _sendQueue.Enqueue(new CreateChatResponse(chatName, chatId, creator, users, DateTime.Now, isDialog).GetContainer());
            Send();
        }

        private void OnClose(object sender, CloseEventArgs e)
        {
            ConnectionStatusChanged?.Invoke(this, new ConnectStatusChangeEventArgs(_login, _code == ConnectionRequestCode.Disconnect ? ConnectionRequestCode.Disconnect : ConnectionRequestCode.LoginIsAlreadyTaken));

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
