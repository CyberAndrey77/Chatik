using System;
using System.Collections.Generic;
using System.Text;
using Client.NetWork.EventArgs;
using Client.Services.EventArgs;

namespace Client.Services
{
    public class MessageService : IMessageService
    {
        private readonly IConnectionService _connectionService;
        public EventHandler<MessageEventArgs> MessageEvent { get; set; }
        public EventHandler<PrivateMessageEventArgs> GetPrivateMessageEvent { get; set; }
        public EventHandler<ChatMessageEventArgs> ChatMessageEvent { get; set; }

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
        }

        private void OnChatMessage(object sender, ChatMessageEventArgs e)
        {
            ChatMessageEvent?.Invoke(this, e);
        }

        private void GetPrivateMessage(object sender, PrivateMessageEventArgs e)
        {
            GetPrivateMessageEvent?.Invoke(this, e);
        }

        public void GetMessage(object sender, MessageEventArgs e)
        {
            MessageEvent?.Invoke(this, new MessageEventArgs(e.Name, e.Message, e.Time));
        }

        public void SendPrivateMessage(Guid senderUserId, string message, Guid receiverUSerId)
        {
            _connectionService.SendPrivateMessage(senderUserId, message, receiverUSerId);
        }

        public void SendChatMessage(Guid senderUserId, string text, string chatName, List<Guid> userIds)
        {
            _connectionService.SendChatMessage(senderUserId, text, chatName, userIds);
        }
    }
}