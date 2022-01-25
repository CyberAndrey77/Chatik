using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
