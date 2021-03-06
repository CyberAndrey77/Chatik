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
    public class ChatService : IChatService
    {
        private readonly ITransport _transport;
        private readonly ILogger _logger;
        public EventHandler<ChatEventArgs> ChatIsCreatedEvent { get; set; }
        public EventHandler<UserChatEventArgs<Chat>> GetUserChats { get; set; }
        public EventHandler<ChatEventArgs> ChatIsCreated { get; set; }
        public EventHandler<ChatEventArgs> CreatedChat { get; set; }

        public void CreateChat(string chatName, int chatId, string creator, List<int> invented, bool isDialog)
        {
            _transport.SendRequest(new CreateChatResponse(chatName, chatId, creator, invented, DateTime.Now, isDialog).GetContainer());
        }

        public ChatService(ITransport transport)
        {
            _transport = transport;
            _logger = LogManager.GetCurrentClassLogger();

        }

        private void OnGetChats(MessageContainer message)
        {
            if (message.Identifier != nameof(UserChats<Chat>))
            {
                return;
            }

            var userChats = ((JObject)message.Payload).ToObject(typeof(UserChats<Chat>)) as UserChats<Chat>;
            if (userChats == null)
            {
                _logger.Error($"Answer from server {message}:{message.Identifier} is null");
                return;
            }
            GetUserChats?.Invoke(this, new UserChatEventArgs<Chat>(userChats.Chats));
        }

        private void OnChatCreated(MessageContainer message)
        {
            if (message.Identifier != nameof(CreateChatResponse))
            {
                return;
            }
            var createChatResponse = ((JObject)message.Payload).ToObject(typeof(CreateChatResponse)) as CreateChatResponse;
            if (createChatResponse == null)
            {
                _logger.Error($"Answer from server {message}:{message.Identifier} is null");
                return;
            }
            CreatedChat?.Invoke(this, new ChatEventArgs(createChatResponse.ChatName, createChatResponse.ChatId, createChatResponse.CreatorName,
                createChatResponse.UserIds, createChatResponse.IsDialog, createChatResponse.Time));
        }

        public void Subscribe()
        {
            _transport.Subscribe(EnumKey.ChatKeyGetChats, OnGetChats);
            _transport.Subscribe(EnumKey.ChatKeyCreateChat, OnChatCreated);
        }
    }
}
