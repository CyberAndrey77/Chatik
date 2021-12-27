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

        public void SendPrivateMessage(string senderName, string message, string receiverName)
        {
            _connectionService.SendPrivateMessage(senderName, message, receiverName);
        }

        public void SendChatMessage(string name, string text, string chatName, List<string> users)
        {
            _connectionService.SendChatMessage(name, text, chatName, users);
        }
    }
}