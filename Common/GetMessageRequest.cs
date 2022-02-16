using System.Collections.Generic;

namespace Common
{
    public class GetMessageRequest<T> : GetMessageResponse
    {
        public List<T> Messages { get; set; }
        public Dictionary<int, string> Users { get; set; }
        public GetMessageRequest(int chatId, List<T> messages, Dictionary<int, string> users) : base(chatId)
        {
            Messages = messages;
            Users = users;
        }

        public override MessageContainer GetContainer()
        {
            var container = new MessageContainer
            {
                Identifier = nameof(GetMessageRequest<T>),
                Payload = this
            };

            return container;
        }
    }
}
