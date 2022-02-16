using System;

namespace Client.Models
{
    public class Log
    {
        public int Id { get; set; }
        public RecordType Type { get; set; }
        public string Message { get; set; }
        public DateTime Time { get; set; }
    }
}
