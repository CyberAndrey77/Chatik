using System;
using System.Collections.Generic;

namespace Common
{
    public class ConnectedUser
    {
        public Dictionary<int,string> Users { get; set; }

        public ConnectedUser(Dictionary<int, string> users)
        {
            Users = users;
        }

        public virtual MessageContainer GetContainer()
        {
            var container = new MessageContainer
            {
                Identifier = nameof(ConnectedUser),
                Payload = this
            };

            return container;
        }
    }
}