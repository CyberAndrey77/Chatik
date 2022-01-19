using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class GetMessageResponse
    {
        public int ChatId { get; set; }

        public GetMessageResponse(int chatId)
        {
            ChatId = chatId;
        }

        public virtual MessageContainer GetContainer()
        {
            var container = new MessageContainer
            {
                Identifier = nameof(GetMessageResponse),
                Payload = this
            };

            return container;
        }
    }
}
