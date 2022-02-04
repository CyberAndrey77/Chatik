using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.ServiceModel.Configuration;
using System.Text;
using System.Text.RegularExpressions;
using Client.NetWork;
using Client.Services;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;

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

        public string Name
        {
            get => _name;
            set
            {
                IsButtonEnable = !string.IsNullOrWhiteSpace(value) && !string.IsNullOrWhiteSpace(IpAddress) && string.IsNullOrEmpty(Port);
                RaisePropertyChanged(nameof(IsButtonEnable));
                SetProperty(ref _name, value);
            }
        }
        public string IpAddress
        {
            get => _ipAddress;
            set
            {
                IsButtonEnable = !string.IsNullOrWhiteSpace(Name) && !string.IsNullOrWhiteSpace(value) && string.IsNullOrEmpty(Port);
                RaisePropertyChanged(nameof(IsButtonEnable));
                SetProperty(ref _ipAddress, value);
            }
        }

        public string Port
        {
            get => _port;
            set
            {
                IsButtonEnable = !string.IsNullOrWhiteSpace(Name) && !string.IsNullOrWhiteSpace(IpAddress) && string.IsNullOrEmpty(value);
                RaisePropertyChanged(nameof(IsButtonEnable));
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

        public LoginViewModel(IConnectionService connectionService)
        {
            _messageError = string.Empty;
            _connectionService = connectionService;
            IpAddress = "127.0.0.1";
            Port = "8080";
            Name = "Andrey";
            IsButtonEnable = true;
            SendCommand = new DelegateCommand(ConnectToServer);
        }

        private void ConnectToServer()
        {
            _connectionService.Port = int.Parse(Port);
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
    }
}
