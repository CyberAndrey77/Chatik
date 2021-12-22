using System;
using System.Collections.Generic;
using System.Text;

namespace Client.NetWork
{
    public interface ITransport
    {
        void Connect(string address, int port);

        void Disconnect();

        void Login(string login);

        void SendMessage(string name, string message);
    }
}