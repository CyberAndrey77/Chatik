using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;

namespace Client.ViewModels
{
    public class ShowMessageViewModel : BindableBase, IDialogAware
    {
        private string _title = "Message";
        private string _message;
        private DelegateCommand _closeDialogNegativeCommand;
        private DelegateCommand _closeDialogPositiveCommand;
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        public event Action<IDialogResult> RequestClose;

        public DelegateCommand CloseDialogNegativeCommand => _closeDialogNegativeCommand ??= new DelegateCommand(CloseNegativeDialog);
        public DelegateCommand CloseDialogPositiveCommand => _closeDialogPositiveCommand ??= new DelegateCommand(ClosePositiveDialog);

        private void ClosePositiveDialog()
        {
            ButtonResult result = ButtonResult.OK;
            RaiseRequestClose(new DialogResult(result));
        }

        private void CloseNegativeDialog()
        {
            ButtonResult result = ButtonResult.No;
            RaiseRequestClose(new DialogResult(result));
        }

        private void RaiseRequestClose(IDialogResult dialogResult)
        {
            RequestClose?.Invoke(dialogResult);
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            Message = "Не выбрано ни одного пользователя!\nЧат не будет создан. Хотите продолжить?";
        }
    }
}
