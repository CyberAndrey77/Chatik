using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.EventArgs
{
    public class MessageReceivedEventArgs : System.EventArgs
    {
        public string Name { get; set; }
        public string Message { get; set; }
        public DateTime Time { get; set; }
        
        public MessageReceivedEventArgs(string name, string message)
        {
            Name = name;
            Message = message;
        }

        public MessageReceivedEventArgs(string name, string message, DateTime time)
        {
            Name = name;
            Message = message;
            Time = time;
        }
    }
}
