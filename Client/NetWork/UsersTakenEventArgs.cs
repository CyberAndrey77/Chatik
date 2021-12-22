using System;
using System.Collections.Generic;
using System.Windows.Documents;

namespace Client.NetWork
{
    public class UsersTakenEventArgs : System.EventArgs
    {
        public List<string> Users { get; set; }

        public UsersTakenEventArgs(List<string> users)
        {
            Users = users;
        }
    }
}