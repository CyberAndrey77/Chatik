using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Mime;
using System.Security.AccessControl;
using System.Windows;
using System.Windows.Input;
using Client.Models;
using Client.Services;
using Client.Services.EventArgs;
using Client.Views;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace Client.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "Chatik";
        private object _currentContentVM;
        private ChatControlViewModel _chatControlViewModel;
        private LoginViewModel _loginViewModel;
        private IConnectionService _connectionService;
        private readonly IMessageService _messageService;
        private IDialogService _dialogService;

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public object CurrentContentVM
        {
            get => _currentContentVM;
            set => SetProperty(ref _currentContentVM, value);
        }
        //<!--Closing="{Binding OnClosingMainWindow}"-->
        //public CancelEventHandler OnClosingMainWindow { get; set; }
        public MainWindowViewModel(IMessageService messageService, IConnectionService connectionService, IDialogService dialogService)
        {
            _messageService = messageService;
            _connectionService = connectionService;
            _dialogService = dialogService;
            //OnClosingMainWindow += CloseWindows;
            _connectionService.ConnectionEvent += OnConnection;
            _loginViewModel = new LoginViewModel(connectionService);
            CurrentContentVM = _loginViewModel;
            Application.Current.Exit += CloseWindows;
        }

        private void CloseWindows(object sender, ExitEventArgs eventArgs)
        {
            _connectionService.Disconnect();
        }

        private void OnConnection(object sender, ConnectionEventArgs e)
        {
            if (e.IsConnectSuccess)
            {
                _chatControlViewModel = new ChatControlViewModel(_messageService, _connectionService, _dialogService);
                CurrentContentVM = _chatControlViewModel;
            }
            else
            {
                _loginViewModel.MessageError = e.ConnectedMessage;
                CurrentContentVM = _loginViewModel;
            }
        }
    }
}