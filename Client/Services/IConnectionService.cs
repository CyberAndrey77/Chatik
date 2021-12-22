using System;
using System.Collections.Generic;
using System.Text;
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

        void CreateDialog(string creator, string invented);

        EventHandler<MessageRequestEvent> MessageStatusChangeEvent { get; set; }
        string Name { get; set; }
        string IpAddress { get; set; }
        int Port { get; set; }

        void ConnectToServer();
        void Disconnect();
        void SendMessage(string name, string message);
        void SendPrivateMessage(string senderName, string message, string receiverName);
    }
}