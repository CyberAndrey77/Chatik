using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.NetWork.EventArgs
{
    public class PrivateMessageEventArgs : System.EventArgs
    {
        public string SenderName { get; set; }
        public string Message { get; set; }
        public string ReceiverName { get; set; }
        public DateTime Time { get; set; }

        public PrivateMessageEventArgs(string senderName, string message, string receiverName, DateTime time)
        {
            SenderName = senderName;
            Message = message;
            ReceiverName = receiverName;
            Time = time;
        }
    }
}
