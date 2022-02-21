using Client.Services;
using Common.Enums;
using Common.EventArgs;
using NLog;
using Prism.Commands;
using Prism.Mvvm;
using System.Windows;
using Client.File;

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

        public bool IsConnect
        {
            get => _isConnect;
            set => SetProperty(ref _isConnect, value);
        }

        public DelegateCommand ShowChat { get; }
        public DelegateCommand ShowLog { get; }
        public DelegateCommand DisconnectCommand { get; }
        public DelegateCommand CloseApp { get; }

        public MainWindowViewModel(LoginViewModel loginViewModel, LogControlViewModel logControlViewModel, ChatControlViewModel chatControlViewModel, IConnectionService connection)
        {
            var loggingConfiguration = new NLog.Config.LoggingConfiguration();
            var logfile = new NLog.Targets.FileTarget("logfile") { FileName = "file.txt" };
            loggingConfiguration.AddRule(LogLevel.Error, LogLevel.Fatal, logfile);
            NLog.LogManager.Configuration = loggingConfiguration;
            ShowChat = new DelegateCommand(ShowChatCommand);
            ShowLog = new DelegateCommand(ShowLogCommand);
            DisconnectCommand = new DelegateCommand(Disconnect);
            CloseApp = new DelegateCommand(Application.Current.Shutdown);
            _chatControlViewModel = chatControlViewModel;
            _connection = connection;
           
            _connection.ConnectStatusChangeEvent += OnConnection;
            _loginViewModel = loginViewModel;
            _logControlView = logControlViewModel;
            CurrentContentVm = _loginViewModel;
            Application.Current.Exit += CloseWindows;
        }

        private void Disconnect()
        {
            if (IsConnect)
            {
                _connection.Disconnect();
            }
        }

        private void OnConnection(object sender, ConnectStatusChangeEventArgs e)
        {
            if (e.ConnectionRequestCode == ConnectionRequestCode.Connect)
            {
                IsConnect = true;
                CurrentContentVm = _chatControlViewModel;
                _connection.Id = e.Id;
                _connection.Name = e.Name;
            }
            else
            {
                IsConnect = false;
                CurrentContentVm = _loginViewModel;
            }
        }

        private void ShowLogCommand()
        {
            if (!IsConnect)
            {
                return;
            }
            CurrentContentVm = _logControlView;
        }

        private void ShowChatCommand()
        {
            if (!IsConnect)
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