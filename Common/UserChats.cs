using System.Collections.Generic;

namespace Common
{
    public class UserChats<T>
    {
        public List<T> Chats { get; set; }

        public UserChats(List<T> chats)
        {
            Chats = chats;
        }

        public MessageContainer GetContainer()
        {
            var container = new MessageContainer
            {
                Identifier = nameof(UserChats<T>),
                Payload = this
            };

            return container;
        }
    }
}
