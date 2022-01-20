using System;
using System.Collections.Generic;
using System.Text;
using Client.Models;
using Client.NetWork;
using Client.NetWork.EventArgs;
using Client.Services.EventArgs;
using Common.EventArgs;

namespace Client.Services
{
    public interface IConnectionService
    {
        EventHandler<ConnectionEventArgs> ConnectionEvent { get; set; }
        EventHandler<MessageEventArgs> MessageEvent { get; set; }
        EventHandler<GetUsersEventArgs> UserListEvent { get; set; }
        EventHandler<GetUserEventArgs> UserEvent { get; set; }
        EventHandler<ChatMessageEventArgs> GetPrivateMessageEvent { get; set; }

        EventHandler<ChatEventArgs> ChatCreated { get; set; }

        EventHandler<ChatMessageEventArgs> ChatMessageEvent { get; set; }
        EventHandler<ChatEventArgs> ChatIsCreatedEvent { get; set; }
        EventHandler<UserChatEventArgs<Chat>> GetUserChats { get; set; }
        EventHandler<GetMessagesEventArgs<Message>> GetMessagesEvent { get; set; }

        void CreateChat(string chatName, int chatId, string creator, List<int> invented, bool isDialog);

        EventHandler<MessageRequestEvent> MessageStatusChangeEvent { get; set; }

        int Id { get; set; }
        string Name { get; set; }
        string IpAddress { get; set; }
        int Port { get; set; }

        void ConnectToServer();
        void Disconnect();
        void GetMessages(int chatId);
        void SendMessage(string name, string message);
        void SendPrivateMessage(int senderUserId, string message, int chatId, List<int> userIds);
        void SendChatMessage(int name, string text, int chatId, List<int> userIds, bool isDialog);
    }
}