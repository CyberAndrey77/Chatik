using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;
using Client.Services;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace Client.ViewModels
{
    public class LoginViewModel : BindableBase
    {
        private readonly IConnectionService _connectionService;

        private string _name;
        private string _ipAddress;
        private int _port;
        private bool _isButtonEnable;
        private string _messageError;

        public string Name
        {
            get => _name;
            set
            {
                IsButtonEnable = !string.IsNullOrWhiteSpace(value) && !string.IsNullOrWhiteSpace(IpAddress) && Port != 0;
                RaisePropertyChanged(nameof(IsButtonEnable));
                SetProperty(ref _name, value);
            }
        }
        public string IpAddress
        {
            get => _ipAddress;
            set
            {
                IsButtonEnable = !string.IsNullOrWhiteSpace(Name) && !string.IsNullOrWhiteSpace(value) && Port != 0;
                RaisePropertyChanged(nameof(IsButtonEnable));
                SetProperty(ref _ipAddress, value);
            }
        }

        public int Port
        {
            get => _port;
            set
            {
                IsButtonEnable = !string.IsNullOrWhiteSpace(Name) && !string.IsNullOrWhiteSpace(IpAddress) && value != 0;
                RaisePropertyChanged(nameof(IsButtonEnable));
                SetProperty(ref _port, value);
            }
        }

        public string MessageError
        {
            get => _messageError;
            set
            {
                SetProperty(ref _messageError, value);
            }
        }

        public bool IsButtonEnable
        {
            get => _isButtonEnable;
            set
            {
                SetProperty(ref _isButtonEnable, value);
            }
        }

        public DelegateCommand SendCommand { get; }

        public LoginViewModel(IConnectionService connectionService)
        {
            _messageError = string.Empty;
            _connectionService = connectionService;
            IpAddress = "127.0.0.1";
            Port = 8080;
            Name = "Andrey";
            IsButtonEnable = true;
            SendCommand = new DelegateCommand(ConnectToServer);
        }

        private void ConnectToServer()
        {
            _connectionService.Port = Port;
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
