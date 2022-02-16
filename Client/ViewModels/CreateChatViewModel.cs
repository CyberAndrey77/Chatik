using Client.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Client.ViewModels
{
    public class CreateChatViewModel : BindableBase, IDialogAware
    {
        private DelegateCommand _closeDialogCommand;
        private SelectedItemViewModel _selectedUser;
        private IDialogService _dialogService;
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

        public List<SelectedItemViewModel> Users { get; set; }

        public List<User> SelectedUsers { get; set; }

        public DelegateCommand CloseDialogCommand => _closeDialogCommand ??= new DelegateCommand(CloseDialog);

        private string _title = "Создание беседы";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public event Action<IDialogResult> RequestClose;

        protected virtual void CloseDialog()
        {
            SelectedUsers = new List<User>();

            foreach (var user in Users.Where(user => user.IsSelected))
            {
                SelectedUsers.Add(user.User);
            }

            ButtonResult result = SelectedUsers != null && SelectedUsers.Count != 0 ? ButtonResult.OK : ButtonResult.Cancel;

            ButtonResult resultContinue = ButtonResult.OK;

            if (result == ButtonResult.Cancel)
            {
                var dialogParameters = new DialogParameters();
                _dialogService.ShowDialog("ShowMessage", dialogParameters, r =>
                {
                    resultContinue = r.Result;
                });

                if (resultContinue == ButtonResult.No)
                {
                    return;
                }
            }

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
            parameters.TryGetValue("users", out List<User> sourceUsers);
            //UserIds = users;
            Users = new List<SelectedItemViewModel>();
            foreach (var user in sourceUsers)
            {
                Users.Add(new SelectedItemViewModel(user, false));
            }
        }

        public CreateChatViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;
        }
    }
}
