using Client.Services.EventArgs;
using Common.EventArgs;
using System;

namespace Client.Services
{
    public interface IConnectionService
    {
        EventHandler<GetUsersEventArgs> GetOnlineUsers { get; set; }
        EventHandler<GetUsersEventArgs> AllUsersEvent { get; set; }
        EventHandler<GetUserEventArgs> UserEvent { get; set; }
        EventHandler<ConnectStatusChangeEventArgs> ConnectStatusChangeEvent { get; set; }

        int Id { get; set; }
        string Name { get; set; }
        string IpAddress { get; set; }
        int Port { get; set; }

        void ConnectToServer();
        void Disconnect();
    }
}