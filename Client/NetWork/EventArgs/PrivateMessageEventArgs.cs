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
        public int SenderId { get; set; }
        public string Message { get; set; }
        //public string ReceiverName { get; set; }

        public int ReceiverId { get; set; }
        public DateTime Time { get; set; }

        public PrivateMessageEventArgs(int senderId, string message, int receiverId, DateTime time)
        {
            SenderId = senderId;
            ReceiverId = receiverId;
            Message = message;
            Time = time;
        }
    }
}
