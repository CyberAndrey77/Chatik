using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Models
{
    public class User:ICloneable
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public User(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        public object Clone()
        {
            return new User(Id, Name);
        }
    }
}
