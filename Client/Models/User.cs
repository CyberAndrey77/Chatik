using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Models
{
    public class User:ICloneable
    {
        public string Name { get; set; }

        public User(string name)
        {
            Name = name;
        }

        public object Clone()
        {
            return new User(Name);
        }
    }
}
