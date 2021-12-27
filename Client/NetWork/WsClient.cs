using Common;
using Common.Enums;
using Common.EventArgs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Concurrent;
using System.Collections.Generic;
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

        private readonly ConcurrentQueue<MessageContainer> _sendQueue;

        public event EventHandler<ConnectStatusChangeEventArgs> ConnectionStatusChanged;
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;
        public event EventHandler<UsersTakenEventArgs> UsersTaken;
        public event EventHandler<UserStatusChangeEventArgs> UserEvent;
        public event EventHandler<MessageRequestEvent> MessageRequestEvent;
        public event EventHandler<PrivateMessageEventArgs> PrivateMessageEvent;
        public event EventHandler<ChatEventArgs> CreatedChat;
        public event EventHandler<ChatMessageEventArgs> ChatMessageEvent;

        public WsClient()
        {
            _sendQueue = new ConcurrentQueue<MessageContainer>();
            _code = ConnectionRequestCode.Disconnect;
        }

        public void Connect(string address, int port)
        {
            _socket = new WebSocket($"ws://{address}:{port}");
            _socket.OnOpen += OnOpen;
            _socket.OnClose += OnClose;
            _socket.OnMessage += OnMessage;
            _socket.Connect();
            _socket.WaitTime = TimeSpan.MaxValue;
        }

        private void OnMessage(object sender, MessageEventArgs e)
        {
            var message = JsonConvert.DeserializeObject<MessageContainer>(e.Data);
            switch (message.Identifier)
            {
                case nameof(ConnectionRequest):
                    var connectionRequest = ((JObject)message.Payload).ToObject(typeof(ConnectionRequest)) as ConnectionRequest;
                    if (connectionRequest == null)
                    {
                        throw new ArgumentNullException();
                    }

                    string answer = string.Empty;

                    _code = connectionRequest.CodeConnected;
                    switch (_code)
                    {
                        case ConnectionRequestCode.Connect:
                            answer = "подключился";
                            if (_login != connectionRequest.Login)
                            {
                                UserEvent?.Invoke(this, new UserStatusChangeEventArgs(connectionRequest.Login, true));
                            }
                            break;
                        case ConnectionRequestCode.Disconnect:
                            answer = "отключился";
                            if (_login != connectionRequest.Login)
                            {
                                UserEvent?.Invoke(this, new UserStatusChangeEventArgs(connectionRequest.Login, false));
                            }
                            break;
                        case ConnectionRequestCode.LoginIsAlreadyTaken:
                            answer = "Логин уже занят";
                            _socket.Close();
                            return;
                    }
                    MessageReceived?.Invoke(this, new MessageReceivedEventArgs(connectionRequest.Login, $"{connectionRequest.Login} {answer}"));
                    break;
                case nameof(ServerMessageResponse):
                    var messageResponse = ((JObject)message.Payload).ToObject(typeof(ServerMessageResponse)) as ServerMessageResponse;
                    if (messageResponse == null)
                    {
                        throw new ArgumentNullException();
                    }

                    MessageReceived?.Invoke(this, new MessageReceivedEventArgs(messageResponse.Name, messageResponse.Message, messageResponse.Time));
                    break;
                case nameof(ConnectedUser):
                    var connectedUser = ((JObject)message.Payload).ToObject(typeof(ConnectedUser)) as ConnectedUser;
                    if (connectedUser == null)
                    {
                        throw new ArgumentNullException();
                    }

                    UsersTaken?.Invoke(this, new UsersTakenEventArgs(connectedUser.Users));
                    break;
                case nameof(MessageRequest):
                    var messageRequest = ((JObject)message.Payload).ToObject(typeof(MessageRequest)) as MessageRequest;
                    if (messageRequest == null)
                    {
                        throw new ArgumentNullException();
                    }
                    MessageRequestEvent?.Invoke(this, new MessageRequestEvent(messageRequest.MessageId, messageRequest.Status, messageRequest.Time));
                    break;
                case nameof(PrivateMessageResponseServer):
                    var privateMessageResponseServer = ((JObject)message.Payload).ToObject(typeof(PrivateMessageResponseServer)) as PrivateMessageResponseServer;
                    if (privateMessageResponseServer == null)
                    {
                        throw new ArgumentNullException();
                    }
                    PrivateMessageEvent?.Invoke(this, new PrivateMessageEventArgs
                        (privateMessageResponseServer.SenderName, privateMessageResponseServer.Message, privateMessageResponseServer.ReceiverName, privateMessageResponseServer.Time));
                    break;
                case nameof(CreateChatResponse):
                    var createChatResponse = ((JObject)message.Payload).ToObject(typeof(CreateChatResponse)) as CreateChatResponse;
                    if (createChatResponse == null)
                    {
                        throw new ArgumentNullException();
                    }
                    CreatedChat?.Invoke(this, new ChatEventArgs(createChatResponse.ChatName, createChatResponse.CreatorName, createChatResponse.InventedNames, createChatResponse.Time));
                    break;
                case nameof(ChatMessageResponseServer):
                    var chatMessageResponseServer = ((JObject)message.Payload).ToObject(typeof(ChatMessageResponseServer)) as ChatMessageResponseServer;
                    if (chatMessageResponseServer == null)
                    {
                        throw new ArgumentNullException();
                    }
                    ChatMessageEvent?.Invoke(this, new ChatMessageEventArgs(chatMessageResponseServer.SenderName, chatMessageResponseServer.Message, chatMessageResponseServer.ChatName, chatMessageResponseServer.Users, chatMessageResponseServer.Time));
                    break;
                default:
                    throw new ArgumentNullException();
            }

        }

        internal void SendChatMessage(string name, string text, string chatName, List<string> users)
        {
            _sendQueue.Enqueue(new ChatMessageResponse(name, text, chatName, users).GetContainer());
            Send();
        }

        internal void SendPrivateMessage(string senderName, string message, string receiverName)
        {
            _sendQueue.Enqueue(new PrivateMessageResponseClient(senderName, message, receiverName).GetContainer());
            Send();
        }

        internal void CreateChat(string chatName, string creator, List<string> invented)
        {
            _sendQueue.Enqueue(new CreateChatResponse(chatName, creator, invented, DateTime.Now).GetContainer());
            Send();
        }

        private void OnClose(object sender, CloseEventArgs e)
        {
            ConnectionStatusChanged?.Invoke(this, new ConnectStatusChangeEventArgs(_login, _code));

            _socket.OnOpen -= OnOpen;
            _socket.OnClose -= OnClose;
            _socket.OnMessage -= OnMessage;
            _socket = null;
        }

        private void OnOpen(object sender, System.EventArgs e)
        {
            ConnectionStatusChanged?.Invoke(this, new ConnectStatusChangeEventArgs(_login, ConnectionRequestCode.Connect));
        }

        public void Disconnect()
        {
            if (_socket == null)
            {
                return;
            }

            _socket.Close();
        }

        public void Login(string login)
        {
            _login = login;
            _sendQueue.Enqueue(new ConnectionResponse(_login).GetContainer());
            Send();
        }

        public void SendMessage(string name, string message)
        {
            _sendQueue.Enqueue(new ClientMessageResponse(name, message).GetContainer());
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
    }
}