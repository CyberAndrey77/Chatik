using System.Collections.Generic;

namespace Client.Services.EventArgs
{
    public class GetUsersEventArgs : System.EventArgs
    {
        public Dictionary<int, string> Users { get; set; }

        public GetUsersEventArgs(Dictionary<int, string> users)
        {
            Users = users;
        }
    }
}