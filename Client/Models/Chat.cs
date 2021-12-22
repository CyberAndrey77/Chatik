﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Models
{
    public class Chat
    {
        public string Name { get; set; }
        public List<User> Users { get; set; }

        public bool IsDialog { get; set; }

        public int CountUnreadMessages { get; set; }

        public Chat(List<User> users, string name, bool isDialog)
        {
            Users = users;
            Name = name;
            IsDialog = isDialog;
        }

        public Chat()
        {
            
        }
    }
}
