using System;

namespace Client.Services.EventArgs
{
    public class MessageEventArgs : System.EventArgs
    {
        public string Message { get; set; }

        public string Name { get; set; }

        public DateTime Time { get; set; }

        public MessageEventArgs(string name, string message, DateTime time)
        {
            Name = name;
            Message = message;
            Time = time;
        }
    }
}