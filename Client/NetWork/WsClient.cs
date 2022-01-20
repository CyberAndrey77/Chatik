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

        public event EventHandler<UserIdEventArgs> GetUserIdEvent;
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;
        public event EventHandler<UsersTakenEventArgs> UsersTaken;
        public event EventHandler<UserStatusChangeEventArgs> UserEvent;
        public event EventHandler<MessageRequestEvent> MessageRequestEvent;
        public event EventHandler<ChatMessageEventArgs> PrivateMessageEvent;
        public event EventHandler<ChatEventArgs> CreatedChat;
        public event EventHandler<ChatMessageEventArgs> ChatMessageEvent;
        public event EventHandler<ChatEventArgs> ChatIsCreated;
        public event EventHandler<UserChatEventArgs<Chat>> GetUserChats;
        public event EventHandler<GetMessagesEventArgs<Message>> GetMessagesEvent; 

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
            if (_socket != null)
            {
                _socket.WaitTime = TimeSpan.MaxValue;
            }
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
                                UserEvent?.Invoke(this, new UserStatusChangeEventArgs(connectionRequest.Login, true, connectionRequest.Id));
                            }
                            break;
                        case ConnectionRequestCode.Disconnect:
                            answer = "отключился";
                            if (_login != connectionRequest.Login)
                            {
                                UserEvent?.Invoke(this, new UserStatusChangeEventArgs(connectionRequest.Login, false, connectionRequest.Id));
                            }
                            break;
                        case ConnectionRequestCode.LoginIsAlreadyTaken:
                            answer = "Логин уже занят";
                            _socket.Close();
                            return;
                    }

                    if (_login == connectionRequest.Login)
                    {
                        GetUserIdEvent?.Invoke(this, new UserIdEventArgs(connectionRequest.Id));
                        break;
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
                    MessageRequestEvent?.Invoke(this, new MessageRequestEvent(messageRequest.MessageId, messageRequest.Status, messageRequest.Time){ ChatId = messageRequest.ChatId});
                    break;
                case nameof(PrivateMessageResponseServer):
                    var privateMessageResponseServer = ((JObject)message.Payload).ToObject(typeof(PrivateMessageResponseServer)) as PrivateMessageResponseServer;
                    if (privateMessageResponseServer == null)
                    {
                        throw new ArgumentNullException();
                    }
                    PrivateMessageEvent?.Invoke(this, new ChatMessageEventArgs
                        (privateMessageResponseServer.SenderId, privateMessageResponseServer.Message, privateMessageResponseServer.ChatId, privateMessageResponseServer.UserIds, privateMessageResponseServer.Time));
                    break;
                case nameof(CreateChatResponse):
                    var createChatResponse = ((JObject)message.Payload).ToObject(typeof(CreateChatResponse)) as CreateChatResponse;
                    if (createChatResponse == null)
                    {
                        throw new ArgumentNullException();
                    }
                    CreatedChat?.Invoke(this, new ChatEventArgs(createChatResponse.ChatName, createChatResponse.ChatId, createChatResponse.CreatorName, 
                        createChatResponse.UserIds, createChatResponse.IsDialog, createChatResponse.Time));
                    break;
                case nameof(ChatMessageResponseServer):
                    var chatMessageResponseServer = ((JObject)message.Payload).ToObject(typeof(ChatMessageResponseServer)) as ChatMessageResponseServer;
                    if (chatMessageResponseServer == null)
                    {
                        throw new ArgumentNullException();
                    }
                    ChatMessageEvent?.Invoke(this, new ChatMessageEventArgs(chatMessageResponseServer.SenderUserId, chatMessageResponseServer.Message, 
                        chatMessageResponseServer.ChatId, chatMessageResponseServer.UserIds, chatMessageResponseServer.Time));
                    break;
                case nameof(CreateChatRequest):
                    var createChatRequest = ((JObject)message.Payload).ToObject(typeof(CreateChatRequest)) as CreateChatRequest;
                    if (createChatRequest == null)
                    {
                        throw new ArgumentNullException();
                    }
                    ChatIsCreated?.Invoke(this, new ChatEventArgs(createChatRequest.ChatName, createChatRequest.ChatId, createChatRequest.CreatorName,
                        createChatRequest.UserIds, createChatRequest.IsDialog, createChatRequest.Time));
                    break;
                case nameof(UserChats<Chat>):
                    var userChats = ((JObject)message.Payload).ToObject(typeof(UserChats<Chat>)) as UserChats<Chat>;
                    if (userChats == null)
                    {
                        throw new ArgumentNullException();
                    }
                    GetUserChats?.Invoke(this, new UserChatEventArgs<Chat>(userChats.Chats));
                    break;
                case nameof(GetMessageRequest<Message>):
                    var getMessages = ((JObject) message.Payload).ToObject(typeof(GetMessageRequest<Message>)) as GetMessageRequest<Message>;
                    if (getMessages == null)
                    {
                        throw new ArgumentNullException();
                    }
                    GetMessagesEvent?.Invoke(this, new GetMessagesEventArgs<Message>(getMessages.ChatId){Messages = getMessages.Messages, Users = getMessages.Users});
                    break;
                default:
                    throw new ArgumentNullException();
            }

        }

        internal void GetMessage(int chatId)
        {
            _sendQueue.Enqueue(new GetMessageResponse(chatId).GetContainer());
            Send();
        }

        internal void SendChatMessage(int senderUserId, string text, int chatId, List<int> users, bool isDialog)
        {
            _sendQueue.Enqueue(new ChatMessageResponse(senderUserId, text, chatId, users, isDialog).GetContainer());
            Send();
        }

        internal void SendPrivateMessage(int senderUserId, string message, int chatId, List<int> userIds)
        {
            _sendQueue.Enqueue(new PrivateMessageResponseClient(senderUserId, message, chatId, userIds).GetContainer());
            Send();
        }

        internal void CreateChat(string chatName, int chatId, string creator, List<int> users, bool isDialog)
        {
            _sendQueue.Enqueue(new CreateChatResponse(chatName, chatId, creator, users, DateTime.Now, isDialog).GetContainer());
            Send();
        }

        private void OnClose(object sender, CloseEventArgs e)
        {
            ConnectionStatusChanged?.Invoke(this, new ConnectStatusChangeEventArgs(_login, _code == ConnectionRequestCode.Connect ? ConnectionRequestCode.Disconnect : ConnectionRequestCode.LoginIsAlreadyTaken));

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
            _socket?.Close();
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