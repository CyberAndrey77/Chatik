using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class ServerMessageResponse
    {
        public string Message { get; set; }
        public string Name { get; set; }
        public DateTime Time { get; set; }

        public ServerMessageResponse(string name, string message, DateTime time)
        {
            Message = message;
            Name = name;
            Time = time;
        }
        public MessageContainer GetContainer()
        {
            var container = new MessageContainer
            {
                Identifier = nameof(ServerMessageResponse),
                Payload = this
            };

            return container;
        }
    }
}
