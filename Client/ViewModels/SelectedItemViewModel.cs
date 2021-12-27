using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Models;
using Prism.Mvvm;

namespace Client.ViewModels
{
    public class SelectedItemViewModel: BindableBase
    {
        private User _user;
        private bool _isSelected;

        public User User
        {
            get => _user;
            set => SetProperty(ref _user, value);
        }

        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        public SelectedItemViewModel(User user, bool isSelected)
        {
            User = user;
            IsSelected = isSelected;
        }
    }
}
