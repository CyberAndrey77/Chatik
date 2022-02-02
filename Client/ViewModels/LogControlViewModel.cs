using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Models;
using Client.NetWork;
using Client.Services;
using Common.EventArgs;
using Prism.Commands;
using Prism.Mvvm;

namespace Client.ViewModels
{
    public class LogControlViewModel: BindableBase
    {
        private int _id;
        private RecordType _type;
        private string _message;
        private DateTime _time;
        private readonly IConnectionService _connectionService;
        private DateTime _startTime;
        private DateTime _endTime;
        private RecordType _selectType;

        public int Id
        {
            get => _id;
            set
            {
                SetProperty(ref _id, value);
            }
        }

        public RecordType Type
        {
            get => _type;
            set
            {
                SetProperty(ref _type, value);
            }
        }

        public string Message
        {
            get => _message;
            set
            {
                SetProperty(ref _message, value);
            }
        }

        public DateTime Time
        {
            get => _time;
            set
            {
                SetProperty(ref _time, value);
            }
        }

        public DateTime StarTime
        {
            get => _startTime;
            set
            {
                SetProperty(ref _startTime, value);
            }
        }

        public DateTime EndTime
        {
            get => _endTime;
            set
            {
                SetProperty(ref _endTime, value);
            }
        }

        public RecordType SelectType
        {
            get => _selectType;
            set
            {
                SetProperty(ref _selectType, value);
            }
        }

        public ObservableCollection<Log> Logs { get; set; }

        public DelegateCommand GetLogsCommand { get; }

        public LogControlViewModel(IConnectionService connectionService)
        {
            _connectionService = connectionService;
            //.GetLogsEvent += OnGetLogs;
            StarTime = new DateTime(2021, 12, 1);
            EndTime = DateTime.Today;
            SelectType = RecordType.All;
            GetLogsCommand = new DelegateCommand(GetLogs);
            Logs = new ObservableCollection<Log>();
        }

        private void OnGetLogs(object sender, LogEventArgs<Log> e)
        {
            App.Current.Dispatcher.Invoke(delegate
            {
                if (Logs.Count > 0)
                {
                    Logs.Clear();
                }
                Logs.AddRange(e.LogsList);
            });
        }

        private void GetLogs()
        {
            //_connectionService.GetLogs((int)SelectType, StarTime, EndTime);
        }
    }
}
