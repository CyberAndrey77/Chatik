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
        EventHandler<GetUsersEventArgs> UserListEvent { get; set; }
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