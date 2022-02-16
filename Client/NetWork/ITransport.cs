using System;
using System.Collections.Generic;
using System.Text;
using Client.Enums;
using Common;
using Common.EventArgs;
using WebSocketSharp;

namespace Client.NetWork
{
    public interface ITransport
    {
        EventHandler<CloseEventArgs> ConnectionStatusChanged { get; set; }
        void Connect(string address, int port);

        void Disconnect();

        void Login(string login);

        void Subscribe(EnumKey key, Action<MessageContainer> method);
        void SendRequest(MessageContainer container);
        void Unsubscribe();
    }
}