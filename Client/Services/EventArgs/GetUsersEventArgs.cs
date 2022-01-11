using System;
using System.Collections.Generic;

namespace Client.Services.EventArgs
{
    public class GetUsersEventArgs : System.EventArgs
    {
        public Dictionary<Guid, string> Users { get; set; }

        public GetUsersEventArgs(Dictionary<Guid, string> users)
        {
            Users = users;
        }
    }
}