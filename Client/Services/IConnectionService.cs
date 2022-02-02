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
        EventHandler<ConnectStatusChangeEventArgs> ConnectStatusChangeEvent { get; set; }
        
        int Id { get; set; }
        string Name { get; set; }
        string IpAddress { get; set; }
        int Port { get; set; }

        void ConnectToServer();
        void Disconnect();
    }
}