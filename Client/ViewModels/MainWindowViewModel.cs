using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Mime;
using System.Security.AccessControl;
using System.Windows;
using System.Windows.Input;
using Client.Models;
using Client.NetWork;
using Client.Services;
using Client.Services.EventArgs;
using Client.Views;
using Common.Enums;
using Common.EventArgs;
using NLog;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace Client.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "Chatik";
        private object _currentContentVm;
        private readonly ChatControlViewModel _chatControlViewModel;
        private readonly LogControlViewModel _logControlView;
        private readonly LoginViewModel _loginViewModel;
        private readonly IConnectionService _connection;
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

        public MainWindowViewModel(LoginViewModel loginViewModel, LogControlViewModel logControlViewModel, ChatControlViewModel chatControlViewModel, IConnectionService connection)
        {
            var config = new NLog.Config.LoggingConfiguration();
            var logfile = new NLog.Targets.FileTarget("logfile") { FileName = "file.txt" };
            config.AddRule(LogLevel.Error, LogLevel.Fatal, logfile);
            NLog.LogManager.Configuration = config;
            ShowChat = new DelegateCommand(ShowChatCommand);
            ShowLog = new DelegateCommand(ShowLogCommand);
            _chatControlViewModel = chatControlViewModel;
            _connection = connection;
            _connection.ConnectStatusChangeEvent += OnConnection;
            _loginViewModel = loginViewModel;
            _logControlView = logControlViewModel;
            CurrentContentVm = _loginViewModel;
            Application.Current.Exit += CloseWindows;
        }

        private void OnConnection(object sender, ConnectStatusChangeEventArgs e)
        {
            if (e.ConnectionRequestCode == ConnectionRequestCode.Connect)
            {
                _isConnect = true;
                CurrentContentVm = _chatControlViewModel;
                _connection.Id = e.Id;
                _connection.Name = e.Name;
            }
            else
            {
                _loginViewModel.MessageError = e.ConnectionRequestCode.ToString();
                _isConnect = false;
                CurrentContentVm = _loginViewModel;
            }
        }

        private void ShowLogCommand()
        {
            if (!_isConnect)
            {
                return;
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
            _connection.Disconnect();
        }
    }
}