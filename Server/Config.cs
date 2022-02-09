using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Config
    {
        public int WaitTimeInSecond { get; set; }

        public int Port { get; set; }

        public string ConnectionString { get; set; }
    }
}
