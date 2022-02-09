using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.EventArgs
{
    public class UserDataEventArgs: System.EventArgs
    {
        public int Id { get; set; }

        public UserDataEventArgs(int id)
        {
            Id = id;
        }
    }
}
