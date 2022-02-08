using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Common;
using Common.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Server.Models;
using WebSocketSharp;
using WebSocketSharp.Server;
using Timer = System.Timers.Timer;

namespace Server
{
    public class WsConnection : WebSocketBehavior
    {
        private readonly ConcurrentQueue<MessageContainer> _sendQueue;
        private WsServer _wsServer;
        private Timer _timer;
        //TODO В коонфигуратор
        private double _waitTime = 600000;

        public int Id { get; set; }
        public string Login { get; set; }

        public WsConnection()
        {
            _sendQueue = new ConcurrentQueue<MessageContainer>();
            //UserId = Guid.NewGuid();
            Id = GetHashCode();
            _timer = new Timer(_waitTime);
            _timer.Elapsed += CloseTime;
            _timer.Start();
        }

        private void CloseTime(object sender, System.Timers.ElapsedEventArgs e)
        {
            _timer.Stop();
            Close(ConnectionRequestCode.Inactivity);
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
            Close(ConnectionRequestCode.Disconnect);
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            var message = JsonConvert.DeserializeObject<MessageContainer>(e.Data);
            _timer.Stop();
            _timer.Start();

            switch (message.Identifier)
            {
                case nameof(ConnectionResponse):
                    var messageRequest = ((JObject)message.Payload).ToObject(typeof(ConnectionResponse)) as ConnectionResponse;
                    if (messageRequest == null)
                    {
                        throw new ArgumentNullException();
                    }

                    if (!_wsServer.HandleConnect(Id, messageRequest))
                    {
                        Close(ConnectionRequestCode.LoginIsAlreadyTaken);
                    }
                    break;
                case nameof(CreateChatResponse):
                    var createDialogResponse = ((JObject) message.Payload).ToObject(typeof(CreateChatResponse)) as CreateChatResponse;

                    if (createDialogResponse == null)
                    {
                        throw new ArgumentNullException();
                    }

                    _wsServer.CreateChat(Id, createDialogResponse);
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

        public void Close(ConnectionRequestCode reason)
        {
            _wsServer.FreeConnection(Id, reason);
            Context.WebSocket.Close((ushort)reason);
            _timer.Stop();
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
            SendAsync(serializedMessages, SendCompleted);
        }

        private void SendCompleted(bool obj)
        {
            
        }

    }
}