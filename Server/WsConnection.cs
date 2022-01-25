using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Server.Models;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace Server
{
    public class WsConnection : WebSocketBehavior
    {
        private readonly ConcurrentQueue<MessageContainer> _sendQueue;
        private WsServer _wsServer;

        public int Id { get; set; }
        public string Login { get; set; }

        public WsConnection()
        {
            _sendQueue = new ConcurrentQueue<MessageContainer>();
            //UserId = Guid.NewGuid();
            Id = GetHashCode();
        }

        public void AddServer(WsServer wsServer)
        {
            _wsServer = wsServer;
        }

        protected override void OnOpen()
        {
            _wsServer.AddConnection(this);
        }

        protected override void OnClose(CloseEventArgs e)
        {
            _wsServer.FreeConnection(Id);
            Context.WebSocket.Close();
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            var message = JsonConvert.DeserializeObject<MessageContainer>(e.Data);

            switch (message.Identifier)
            {
                case nameof(ConnectionResponse):
                    var messageRequest = ((JObject)message.Payload).ToObject(typeof(ConnectionResponse)) as ConnectionResponse;
                    if (messageRequest == null)
                    {
                        throw new ArgumentNullException();
                    }
                    _wsServer.HandleConnect(Id, messageRequest);
                    break;
                //case nameof(ClientMessageResponse):
                //    var messageResponse = ((JObject)message.Payload).ToObject(typeof(ClientMessageResponse)) as ClientMessageResponse;
                //    if (messageResponse == null)
                //    {
                //        throw new ArgumentNullException();
                //    }
                //    _wsServer.HandleMessage(UserId, messageResponse);
                //    break;
                case nameof(CreateChatResponse):
                    var createDialogResponse = ((JObject) message.Payload).ToObject(typeof(CreateChatResponse)) as CreateChatResponse;

                    if (createDialogResponse == null)
                    {
                        throw new ArgumentNullException();
                    }

                    _wsServer.CreateChat(Id, createDialogResponse);
                    break;
                case nameof(PrivateMessageResponseClient):
                    var privateMessageResponse = ((JObject)message.Payload).ToObject(typeof(PrivateMessageResponseClient)) as PrivateMessageResponseClient;

                    if (privateMessageResponse == null)
                    {
                        throw new ArgumentNullException();
                    }
                    _wsServer.HandleMessageToClient(Id, privateMessageResponse);
                    break;
                case nameof(ChatMessageResponse):
                    var chatMessageResponse = ((JObject)message.Payload).ToObject(typeof(ChatMessageResponse)) as ChatMessageResponse;
                    if (chatMessageResponse == null)
                    {
                        throw new ArgumentNullException();
                    }

                    _wsServer.HandleChatMessage(Id, chatMessageResponse);
                    break;

                case nameof(GetMessageResponse):
                    var getMessage = ((JObject) message.Payload).ToObject(typeof(GetMessageResponse)) as GetMessageResponse;
                    if (getMessage == null)
                    {
                        throw new ArgumentNullException();
                    }

                    _wsServer.GetMessages(Id, getMessage.ChatId);
                    break;
                case nameof(GetLogsResponse<Log>):
                    var logs =
                        ((JObject) message.Payload).ToObject(typeof(GetLogsResponse<Log>)) as GetLogsResponse<Log>;
                    if (logs == null)
                    {
                        throw new ArgumentNullException();
                    }
                    _wsServer.GetLogs(Id, logs);
                    break;
                default:
                    throw new ArgumentNullException();
            }
           // Thread.Sleep(10000);
        }

        protected override void OnError(ErrorEventArgs e)
        {
            base.OnError(e);
        }

        public void Close()
        {
            Context.WebSocket.Close();
        }

        public void Send(MessageContainer container)
        {
            _sendQueue.Enqueue(container);

            if (!_sendQueue.TryDequeue(out MessageContainer message))
            {
                return;
            }

            string serializedMessages = JsonConvert.SerializeObject(message, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            try
            {
                SendAsync(serializedMessages, SendCompleted);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private void SendCompleted(bool obj)
        {
            
        }

    }
}