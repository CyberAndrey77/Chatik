using System.Collections.Generic;

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