using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace Client.ViewModels
{
    class CreateChatViewModel : BindableBase, IDialogAware
    {
        private DelegateCommand _closeDialogCommand;
        private SelectedItemViewModel _selectedUser;
        private string _chatName;
        
        public SelectedItemViewModel SelectedUser
        {
            get => _selectedUser;
            set
            {
                SetProperty(ref _selectedUser, value);
            }
        }

        public string ChatName
        {
            get => _chatName;
            set => SetProperty(ref _chatName, value);
        }

        public ObservableCollection<SelectedItemViewModel> Users { get; set; }

        public ObservableCollection<User> SelectedUsers { get; set; }

        public DelegateCommand CloseDialogCommand =>
            _closeDialogCommand ?? (_closeDialogCommand = new DelegateCommand(CloseDialog));

        private string _title = "Создание беседы";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public event Action<IDialogResult> RequestClose;

        protected virtual void CloseDialog()
        {
            ButtonResult result;

            SelectedUsers = new ObservableCollection<User>();

            foreach (var user in Users)
            {
                if (user.IsSelected)
                {
                    SelectedUsers.Add(user.User);
                }
            }

            result = SelectedUsers != null && SelectedUsers.Count != 0 ? ButtonResult.OK : ButtonResult.Cancel;

            RaiseRequestClose(new DialogResult(result));
        }

        public virtual void RaiseRequestClose(IDialogResult dialogResult)
        {
            dialogResult.Parameters.Add("chatName", ChatName);
            dialogResult.Parameters.Add("users", SelectedUsers);
            RequestClose?.Invoke(dialogResult);
        }

        public virtual bool CanCloseDialog()
        {
            return true;
        }

        public virtual void OnDialogClosed()
        {

        }

        public virtual void OnDialogOpened(IDialogParameters parameters)
        {
            parameters.TryGetValue("users", out ObservableCollection<User> users);
            //Users = users;
            Users = new ObservableCollection<SelectedItemViewModel>();
            foreach (var user in users)
            {
                Users.Add(new SelectedItemViewModel(user, false));
            }
        }
    }
}
