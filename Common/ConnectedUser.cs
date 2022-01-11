using System;
using System.Collections.Generic;

namespace Common
{
    public class ConnectedUser
    {
        public Dictionary<Guid,string> Users { get; set; }

        public ConnectedUser(Dictionary<Guid, string> users)
        {
            Users = users;
        }

        public MessageContainer GetContainer()
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