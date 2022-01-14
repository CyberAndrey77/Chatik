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
        EventHandler<PrivateMessageEventArgs> GetPrivateMessageEvent { get; set; }

        EventHandler<ChatEventArgs> ChatCreated { get; set; }

        EventHandler<ChatMessageEventArgs> ChatMessageEvent { get; set; }
        EventHandler<ChatEventArgs> ChatIsCreatedEvent { get; set; }
        EventHandler<UserChatEventArgs<Chat>> GetUserChats { get; set; }

        void CreateChat(string chatName, string creator, List<int> invented);

        EventHandler<MessageRequestEvent> MessageStatusChangeEvent { get; set; }

        int Id { get; set; }
        string Name { get; set; }
        string IpAddress { get; set; }
        int Port { get; set; }

        void ConnectToServer();
        void Disconnect();
        void SendMessage(string name, string message);
        void SendPrivateMessage(int senderUserId, string message, int receiverUSerId);
        void SendChatMessage(int name, string text, string chatName, List<int> userIds);
    }
}