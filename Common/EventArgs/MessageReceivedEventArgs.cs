using System;

namespace Common.EventArgs
{
    public class MessageReceivedEventArgs : System.EventArgs
    {
        public string SenderName { get; set; }
        public string Message { get; set; }
        public DateTime Time { get; set; }

        public string ReceiverName { get; set; }

        public MessageReceivedEventArgs(string senderName, string message)
        {
            SenderName = senderName;
            Message = message;
        }

        public MessageReceivedEventArgs(string senderName, string message, DateTime time)
        {
            SenderName = senderName;
            Message = message;
            Time = time;
        }

        public MessageReceivedEventArgs(string senderName, string message, string receiverName, DateTime time)
        {
            SenderName = senderName;
            Message = message;
            ReceiverName = receiverName;
            Time = time;
        }
    }
}
