using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class ClientMessageResponse
    {
        public Guid MessageId { get; }
        public string Message { get; set; }
        public string Name { get; set; }

        public ClientMessageResponse(string name, string message)
        {
            Message = message;
            Name = name;
            MessageId = Guid.NewGuid();
        }


        public MessageContainer GetContainer()
        {
            var container = new MessageContainer
            {
                Identifier = nameof(ClientMessageResponse),
                Payload = this
            };

            return container;
        }
    }
}
