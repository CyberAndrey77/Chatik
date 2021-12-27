using System;
using System.Collections.Generic;
using System.Text;
using Client.NetWork.EventArgs;
using Client.Services.EventArgs;

namespace Client.Services
{
    public interface IMessageService
    {
        EventHandler<MessageEventArgs> MessageEvent { get; set; }
        EventHandler<PrivateMessageEventArgs> GetPrivateMessageEvent { get; set; }
        EventHandler<ChatMessageEventArgs> ChatMessageEvent { get; set; }

        void SendMessage(string name, string message);

        void SendPrivateMessage(string senderName, string message, string receiverName);
        void GetMessage(object sender, MessageEventArgs e);
        void SendChatMessage(string name, string text, string chatName, List<string> users);
    }
}