namespace Common.EventArgs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    public class LogEventArgs<T>: EventArgs
    {
        public int UserId { get; set; }
        public List<T> LogsList { get; set; }
        public int Type { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}
