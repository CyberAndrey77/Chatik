using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.NetWork.EventArgs
{
    public class PrivateMessageEventArgs : System.EventArgs
    {
        //public string SenderUserId { get; set; }
        public Guid SenderId { get; set; }
        public string Message { get; set; }
        //public string ReceiverName { get; set; }

        public Guid ReceiverId { get; set; }
        public DateTime Time { get; set; }

        public PrivateMessageEventArgs(Guid senderId, string message, Guid receiverId, DateTime time)
        {
            SenderId = senderId;
            ReceiverId = receiverId;
            Message = message;
            Time = time;
        }
    }
}
