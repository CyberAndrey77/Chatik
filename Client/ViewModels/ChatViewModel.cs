using Client.Models;
using Prism.Mvvm;
using System.Collections.Generic;

namespace Client.ViewModels
{
    public class ChatViewModel : BindableBase
    {
        private string _name;
        private bool _isDialog;
        private int _countUnreadMessages;
        public Chat Chat { get; set; }
        public string Name
        {
            get => _name;
            set
            {
                SetProperty(ref _name, value);
                Chat.Name = value;
            }
        }
        public List<User> Users { get; set; }

        public bool IsDialog
        {
            get => _isDialog;
            set
            {
                SetProperty(ref _isDialog, value);
                Chat.IsDialog = value;
            }
        }

        public int CountUnreadMessages
        {
            get => _countUnreadMessages;
            set
            {
                SetProperty(ref _countUnreadMessages, value);
                Chat.CountUnreadMessages = value;
            }
        }

        public ChatViewModel(List<User> users, string name, bool isDialog)
        {
            Chat = new Chat(users, name, isDialog);
            Users = users;
            Name = name;
            IsDialog = isDialog;
        }

        public ChatViewModel(string name)
        {
            Chat = new Chat(name);
            Name = name;
        }
    }
}
