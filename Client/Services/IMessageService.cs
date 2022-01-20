using System;
using System.Collections.Generic;
using System.Text;
using Client.Models;
using Client.NetWork.EventArgs;
using Client.Services.EventArgs;
using Common.EventArgs;

namespace Client.Services
{
    public interface IMessageService
    {
        EventHandler<MessageEventArgs> MessageEvent { get; set; }
        EventHandler<ChatMessageEventArgs> GetPrivateMessageEvent { get; set; }
        EventHandler<ChatMessageEventArgs> ChatMessageEvent { get; set; }
        EventHandler<GetMessagesEventArgs<Message>> GetMessagesEvent { get; set; }

        void SendMessage(string name, string message);

        void SendPrivateMessage(int senderUserId, string message, int chatId, List<int> userIds);
        void GetMessage(object sender, MessageEventArgs e);
        void SendChatMessage(int senderUserId, string text, int chatId, List<int> userIds, bool isDialog);
        void GetMessages(int chatId);
    }
}