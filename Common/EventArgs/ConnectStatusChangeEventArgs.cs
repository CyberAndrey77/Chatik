using Common.Enums;

namespace Common.EventArgs
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class ConnectStatusChangeEventArgs : EventArgs
    {
        public string Name { get; set; }
        public ConnectionRequestCode ConnectionRequestCode { get; set; }

        public ConnectStatusChangeEventArgs(string name, ConnectionRequestCode connectionRequestCode)
        {
            Name = name;
            ConnectionRequestCode = connectionRequestCode;
        }
    }
}