using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Services.EventArgs
{
    public class ConnectionEventArgs : System.EventArgs
    {
        public bool IsConnectSuccess { get; set; }
        public string ConnectedMessage { get; set; }

        public ConnectionEventArgs(bool isConnectSuccess, string connectedMessage)
        {
            IsConnectSuccess = isConnectSuccess;
            ConnectedMessage = connectedMessage;
        }
    }
}