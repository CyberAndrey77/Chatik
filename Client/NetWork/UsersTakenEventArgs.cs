using System;
using System.Collections.Generic;
using System.Windows.Documents;

namespace Client.NetWork
{
    public class UsersTakenEventArgs : System.EventArgs
    {
        public Dictionary<Guid, string> Users { get; set; }

        public UsersTakenEventArgs(Dictionary<Guid, string> users)
        {
            Users = users;
        }
    }
}