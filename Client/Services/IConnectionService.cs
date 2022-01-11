using System;
using System.Collections.Generic;
using System.Text;
using Client.Models;
using Client.NetWork;
using Client.NetWork.EventArgs;
using Client.Services.EventArgs;

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

        void CreateChat(string chatName, string creator, List<Guid> invented);

        EventHandler<MessageRequestEvent> MessageStatusChangeEvent { get; set; }

        Guid Id { get; set; }
        string Name { get; set; }
        string IpAddress { get; set; }
        int Port { get; set; }

        void ConnectToServer();
        void Disconnect();
        void SendMessage(string name, string message);
        void SendPrivateMessage(Guid senderUserId, string message, Guid receiverUSerId);
        void SendChatMessage(Guid name, string text, string chatName, List<Guid> userIds);
    }
}