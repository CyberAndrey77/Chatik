using Client.Services;
using Common.Enums;
using Common.EventArgs;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.ComponentModel;
using System.Net;
using System.Windows;
using Client.File;

namespace Client.ViewModels
{
    public class LoginViewModel : BindableBase, IDataErrorInfo
    {
        private readonly IConnectionService _connectionService;

        private string _name;
        private string _ipAddress;
        private string _port;
        private bool _isButtonEnable;
        private string _messageError;
        private int _intPort;
        private IConfig _config;

        public string Name
        {
            get => _name;
            set
            {
                CheckEmpty();
                SetProperty(ref _name, value);
            }
        }
        public string IpAddress
        {
            get => _ipAddress;
            set
            {
                CheckEmpty();
                SetProperty(ref _ipAddress, value);
            }
        }

        public string Port
        {
            get => _port;
            set
            {
                CheckEmpty();
                SetProperty(ref _port, value);
            }
        }

        public string MessageError
        {
            get => _messageError;
            set => SetProperty(ref _messageError, value);
        }

        public bool IsButtonEnable
        {
            get => _isButtonEnable;
            set => SetProperty(ref _isButtonEnable, value);
        }

        public DelegateCommand SendCommand { get; }

        public string Error { get; set; }

        public string this[string columnName] => GetError(columnName);

        private string GetError(string columnName)
        {
            switch (columnName)
            {
                case nameof(IpAddress):
                    if (!IPAddress.TryParse(IpAddress, out var address))
                    {
                        Error = "Введеный IP адресс некорректен!";
                        IsButtonEnable = false;
                    }
                    else
                    {
                        IsButtonEnable = true;
                        Error = string.Empty;
                    }
                    break;
                case nameof(Port):
                    if (!int.TryParse(Port, out _intPort))
                    {
                        Error = "Введеное значение содержет не корретные символы!";
                        IsButtonEnable = false;
                    }
                    else
                    {
                        IsButtonEnable = true;
                        Error = string.Empty;
                    }
                    break;
                default:
                    Error = string.Empty;
                    break;
            }
            return Error;
        }

        public LoginViewModel(IConnectionService connectionService,IConfig config)
        {
            _messageError = string.Empty;
            _connectionService = connectionService;
            _config = config;
            _connectionService.Name = _config.Name;
            _connectionService.IpAddress = _config.IpAddress;
            _connectionService.Port = _config.Port;
            _connectionService.ConnectStatusChangeEvent += OnConnection;
            //IpAddress = "127.0.0.1";
            //Port = "8080";
            //Name = "Andrey";
            IpAddress = _connectionService.IpAddress;
            Port = _connectionService.Port.ToString();
            Name = _connectionService.Name;
            IsButtonEnable = true;
            SendCommand = new DelegateCommand(ConnectToServer);
            Application.Current.Exit += CloseWindows;
        }

        private void CloseWindows(object sender, ExitEventArgs e)
        {
            _config.Name = Name;
            _config.IpAddress = IpAddress;
            _config.Port = _intPort;
            _config.SaveSelf();
        }

        private void OnConnection(object sender, ConnectStatusChangeEventArgs e)
        {
            switch (e.ConnectionRequestCode)
            {
                case ConnectionRequestCode.Connect:
                    MessageError = "Подключение успешно!";
                    break;
                case ConnectionRequestCode.Disconnect:
                    MessageError = "Отключен от сервера!";
                    break;
                case ConnectionRequestCode.LoginIsAlreadyTaken:
                    MessageError = "Логин уже занят!";
                    break;
                case ConnectionRequestCode.Inactivity:
                    MessageError = "Отключен из-за бездействия!";
                    break;
                case ConnectionRequestCode.ServerNotResponding:
                    MessageError = "Сервер не отвечает!";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ConnectToServer()
        {
            _connectionService.Port = _intPort;
            _connectionService.IpAddress = IpAddress;
            _connectionService.Name = Name;

            try
            {
                _connectionService.ConnectToServer();
            }
            catch (InvalidOperationException e)
            {
                MessageError = e.Message;
            }
        }


        private void CheckEmpty()
        {
            IsButtonEnable = !string.IsNullOrWhiteSpace(Name) && !string.IsNullOrWhiteSpace(IpAddress) &&
                             !string.IsNullOrWhiteSpace(Port);
            RaisePropertyChanged(nameof(IsButtonEnable));
        }
    }
}
