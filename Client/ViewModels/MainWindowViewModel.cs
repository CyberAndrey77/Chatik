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
        private object _currentContentVm;
        private ChatControlViewModel _chatControlViewModel;
        private LogControlViewModel _logControlView;
        private LoginViewModel _loginViewModel;
        private IConnectionService _connectionService;
        private readonly IMessageService _messageService;
        private IDialogService _dialogService;
        private IChatService _chatService;
        private bool _isConnect;

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public object CurrentContentVm
        {
            get => _currentContentVm;
            set => SetProperty(ref _currentContentVm, value);
        }

        public DelegateCommand ShowChat { get; }
        public DelegateCommand ShowLog { get; }
        //<!--Closing="{Binding OnClosingMainWindow}"-->
        //public CancelEventHandler OnClosingMainWindow { get; set; }
        public MainWindowViewModel(IMessageService messageService, IConnectionService connectionService, IChatService chatService, IDialogService dialogService)
        {
            ShowChat = new DelegateCommand(ShowChatCommand);
            ShowLog = new DelegateCommand(ShowLogCommand);
            _messageService = messageService;
            _connectionService = connectionService;
            _dialogService = dialogService;
            _chatService = chatService;
            //OnClosingMainWindow += CloseWindows;
            _connectionService.ConnectionEvent += OnConnection;
            _loginViewModel = new LoginViewModel(connectionService);
            CurrentContentVm = _loginViewModel;
            Application.Current.Exit += CloseWindows;
        }

        private void ShowLogCommand()
        {
            if (!_isConnect)
            {
                return;
            }

            if (_logControlView == null)
            {
                _logControlView = new LogControlViewModel(_connectionService);
            }
            CurrentContentVm = _logControlView;
        }

        private void ShowChatCommand()
        {
            if (!_isConnect)
            {
                return;
            }

            CurrentContentVm = _chatControlViewModel;
        }

        private void CloseWindows(object sender, ExitEventArgs eventArgs)
        {
            _connectionService.Disconnect();
        }

        private void OnConnection(object sender, ConnectionEventArgs e)
        {
            _isConnect = e.IsConnectSuccess;
            if (e.IsConnectSuccess)
            {
                _chatControlViewModel = new ChatControlViewModel(_messageService, _connectionService, _chatService, _dialogService);
                CurrentContentVm = _chatControlViewModel;
            }
            else
            {
                _loginViewModel.MessageError = e.ConnectedMessage;
                CurrentContentVm = _loginViewModel;
            }
        }
    }
}