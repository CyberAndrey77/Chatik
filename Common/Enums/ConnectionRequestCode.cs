using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Enums
{
    public enum ConnectionRequestCode
    {
        Connect,
        Disconnect = 1005,
        LoginIsAlreadyTaken = 1101,
        Inactivity = 1100,
        ServerNotResponding = 1006
    }
}
