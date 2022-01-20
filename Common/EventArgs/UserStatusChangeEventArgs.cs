﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.EventArgs
{
    public class UserStatusChangeEventArgs : System.EventArgs
    {
        public string UserName { get; set; }
        public bool IsConnect { get; set; }
        public  int Id { get; set; }

        public UserStatusChangeEventArgs(string userNAme, bool isConnect, int id)
        {
            UserName = userNAme;
            IsConnect = isConnect;
            Id = id;
        }
    }
}
