using System;
using System.Collections.Generic;
using System.Text;
using Client.Models;
using Client.NetWork.EventArgs;
using Client.Services.EventArgs;
using Common.EventArgs;

namespace Client.Services
{
    public class MessageService : IMessageService
    {
        private readonly IConnectionService _connectionService;
        public EventHandler<MessageEventArgs> MessageEvent { get; set; }
        public EventHandler<ChatMessageEventArgs> GetPrivateMessageEvent { get; set; }
        public EventHandler<ChatMessageEventArgs> ChatMessageEvent { get; set; }
        public EventHandler<GetMessagesEventArgs<Message>> GetMessagesEvent { get; set; }

        public void SendMessage(string name, string message)
        {
            _connectionService.SendMessage(name, message);
        }

        public MessageService(IConnectionService connectionService)
        {
            _connectionService = connectionService;
            _connectionService.MessageEvent += GetMessage;
            _connectionService.GetPrivateMessageEvent += GetPrivateMessage;
            _connectionService.ChatMessageEvent += OnChatMessage;
            _connectionService.GetMessagesEvent += OnGetMessages;
        }

        private void OnGetMessages(object sender, GetMessagesEventArgs<Message> e)
        {
            GetMessagesEvent?.Invoke(this, e);
        }

        private void OnChatMessage(object sender, ChatMessageEventArgs e)
        {
            ChatMessageEvent?.Invoke(this, e);
        }

        private void GetPrivateMessage(object sender, ChatMessageEventArgs e)
        {
            GetPrivateMessageEvent?.Invoke(this, e);
        }

        public void GetMessage(object sender, MessageEventArgs e)
        {
            MessageEvent?.Invoke(this, new MessageEventArgs(e.Name, e.Message, e.Time));
        }

        public void SendPrivateMessage(int senderUserId, string message, int chatId, List<int> userIds)
        {
            _connectionService.SendPrivateMessage(senderUserId, message, chatId, userIds);
        }

        public void SendChatMessage(int senderUserId, string text, int chatId, List<int> userIds, bool isDialog)
        {
            _connectionService.SendChatMessage(senderUserId, text, chatId, userIds, isDialog);
        }
        
        public void GetMessages(int chatId)
        {
            _connectionService.GetMessages(chatId);
        }
    }
}