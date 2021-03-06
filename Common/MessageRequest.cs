using Common.Enums;
using System;

namespace Common
{
    public class MessageRequest
    {
        public int ChatId { get; set; }
        public Guid MessageId { get; private set; }
        public MessageStatus Status { get; set; }

        public DateTime Time { get; set; }

        public MessageRequest(MessageStatus status, DateTime time, Guid messageId)
        {
            Status = status;
            Time = time;
            MessageId = messageId;
        }

        public MessageContainer GetContainer()
        {
            var container = new MessageContainer
            {
                Identifier = nameof(MessageRequest),
                Payload = this
            };

            return container;
        }
    }
}
