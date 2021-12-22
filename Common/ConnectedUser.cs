using System.Collections.Generic;

namespace Common
{
    public class ConnectedUser
    {
        public List<string> Users { get; set; }

        public ConnectedUser(List<string> users)
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