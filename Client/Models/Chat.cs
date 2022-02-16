using System.Collections.Generic;

namespace Client.Models
{
    public class Chat
    {
        public int Id { get; set; }
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

        public Chat(string name)
        {
            Name = name;
        }

        public Chat()
        {

        }
    }
}
