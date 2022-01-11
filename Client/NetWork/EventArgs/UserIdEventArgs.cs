using System;

namespace Client.NetWork.EventArgs
{
    public class UserIdEventArgs: System.EventArgs
    {
        public Guid UserId { get; set; }

        public UserIdEventArgs(Guid id)
        {
            UserId = id;
        }
    }
}