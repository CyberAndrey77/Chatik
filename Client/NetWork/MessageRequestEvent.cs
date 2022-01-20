using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Enums;

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
