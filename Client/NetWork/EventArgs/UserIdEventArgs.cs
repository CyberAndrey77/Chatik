using System;

namespace Client.NetWork.EventArgs
{
    public class UserIdEventArgs: System.EventArgs
    {
        public int UserId { get; set; }

        public UserIdEventArgs(int id)
        {
            UserId = id;
        }
    }
}