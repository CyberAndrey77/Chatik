using System.Collections.Generic;

namespace Client.Services.EventArgs
{
    public class GetUsersEventArgs : System.EventArgs
    {
        public List<string> Users { get; set; }

        public GetUsersEventArgs(List<string> users)
        {
            Users = users;
        }
    }
}