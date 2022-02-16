using Client.Enums;
using Client.Models;
using Client.NetWork;
using Client.Services.EventArgs;
using Common;
using Common.EventArgs;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;

namespace Client.Services
{
    public class MessageService : IMessageService
    {
        private readonly ITransport _transport;
        private readonly ILogger _logger;
        public EventHandler<MessageEventArgs> MessageEvent { get; set; }
        public EventHandler<ChatMessageEventArgs> ChatMessageEvent { get; set; }
        public EventHandler<GetMessagesEventArgs<Message>> GetMessagesEvent { get; set; }

        public EventHandler<MessageRequestEvent> MessageStatusChangeEvent { get; set; }


        public MessageService(ITransport transport)
        {
            _transport = transport;
            _logger = LogManager.GetCurrentClassLogger();
        }

        private void OnGetMessages(MessageContainer message)
        {
            if (message.Identifier != nameof(GetMessageRequest<Message>))
            {
                return;
            }
            var getMessages = ((JObject)message.Payload).ToObject(typeof(GetMessageRequest<Message>)) as GetMessageRequest<Message>;
            if (getMessages == null)
            {
                _logger.Error($"Answer from server {message}:{message.Identifier} is null");
                return;
            }
            GetMessagesEvent?.Invoke(this, new GetMessagesEventArgs<Message>(getMessages.ChatId)
            {
                Messages = getMessages.Messages,
                Users = getMessages.Users
            });
        }

        private void OnChatMessage(MessageContainer message)
        {
            if (message.Identifier != nameof(ChatMessageResponseServer))
            {
                return;
            }
            var chatMessageResponseServer = ((JObject)message.Payload).ToObject(typeof(ChatMessageResponseServer)) as ChatMessageResponseServer;
            if (chatMessageResponseServer == null)
            {
                _logger.Error($"Answer from server {message}:{message.Identifier} is null");
                return;
            }
            ChatMessageEvent?.Invoke(this, new ChatMessageEventArgs(chatMessageResponseServer.SenderUserId, chatMessageResponseServer.Message,
                chatMessageResponseServer.ChatId, chatMessageResponseServer.UserIds, chatMessageResponseServer.Time));
        }

        private void OnMessageStatusChange(MessageContainer message)
        {
            if (message.Identifier != nameof(MessageRequest))
            {
                return;
            }

            var messageRequest = ((JObject)message.Payload).ToObject(typeof(MessageRequest)) as MessageRequest;
            if (messageRequest == null)
            {
                _logger.Error($"Answer from server {message}:{message.Identifier} is null");
                return;
            }
            MessageStatusChangeEvent?.Invoke(this, new MessageRequestEvent(messageRequest.MessageId, messageRequest.Status, messageRequest.Time)
            { ChatId = messageRequest.ChatId });
        }

        public void SendChatMessage(int senderUserId, string text, int chatId, List<int> userIds, bool isDialog, Guid messageId)
        {
            _transport.SendRequest(new ChatMessageResponse(senderUserId, text, chatId, userIds, isDialog, messageId).GetContainer());
        }

        public void GetMessages(int chatId)
        {
            _transport.SendRequest(new GetMessageResponse(chatId).GetContainer());
        }

        public void Subscribe()
        {
            _transport.Subscribe(EnumKey.MessageKeyOnChatMessage, OnChatMessage);
            _transport.Subscribe(EnumKey.MessageKeyRequestMessage, OnMessageStatusChange);
            _transport.Subscribe(EnumKey.MessageKeyGetMessages, OnGetMessages);
        }
    }
}