using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Client.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace Client.ViewModels
{
    public class CreateDialogViewModel : BindableBase, IDialogAware
    {
        private DelegateCommand _closeDialogCommand;
        private User _selectedUser;
        private IDialogService _dialogService;

        public List<User> Users { get; set; }

        public User SelectedUser
        {
            get => _selectedUser;
            set => SetProperty(ref _selectedUser, value);
        }

        public DelegateCommand CloseDialogCommand =>
            _closeDialogCommand ??= new DelegateCommand(CloseDialog);
        
        private string _title = "Создание диалога";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public event Action<IDialogResult> RequestClose;

        protected virtual void CloseDialog()
        {
            ButtonResult result = SelectedUser != null ? ButtonResult.OK : ButtonResult.Cancel;
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
            dialogResult.Parameters.Add("user", _selectedUser);
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
            parameters.TryGetValue("users", out List<User> users);
            Users = users;
        }

        public CreateDialogViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;
        }
    }
}
