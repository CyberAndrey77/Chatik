using Common.Enums;
using System;

namespace Client.NetWork
{
    public class MessageRequestEvent : System.EventArgs
    {
        public int ChatId { get; set; }
        public Guid Id { get; private set; }
        public MessageStatus Status { get; set; }

        public DateTime Time { get; set; }

        public MessageRequestEvent(Guid id, MessageStatus status, DateTime time)
        {
            Id = id;
            Status = status;
            Time = time;
        }
    }
}
