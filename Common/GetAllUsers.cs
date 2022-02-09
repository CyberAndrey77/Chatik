using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class GetAllUsers: ConnectedUser
    {
        public GetAllUsers(Dictionary<int, string> users) : base(users)
        {

        }

        public override MessageContainer GetContainer()
        {
            var container = new MessageContainer
            {
                Identifier = nameof(GetAllUsers),
                Payload = this
            };

            return container;
        }
    }
}
