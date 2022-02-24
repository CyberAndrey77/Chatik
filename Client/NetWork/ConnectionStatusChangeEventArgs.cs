using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.NetWork
{
    public class ConnectionStatusChangeEventArgs: EventArgs
    {
        public int Code { get; set; }

        public ConnectionStatusChangeEventArgs(int code)
        {
            Code = code;
        }
    }
}
