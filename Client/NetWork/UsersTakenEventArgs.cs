using System;
using System.Collections.Generic;
using System.Windows.Documents;

namespace Client.NetWork
{
    public class UsersTakenEventArgs : System.EventArgs
    {
        public Dictionary<int, string> Users { get; set; }

        public UsersTakenEventArgs(Dictionary<int, string> users)
        {
            Users = users;
        }
    }
}