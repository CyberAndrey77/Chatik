using Client.Models;
using Client.Services;
using Common.EventArgs;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;

namespace Client.ViewModels
{
    public class LogControlViewModel : BindableBase
    {
        private int _id;
        private RecordType _type;
        private string _message;
        private DateTime _time;
        private readonly ILogService _logService;
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

        public LogControlViewModel(ILogService logService)
        {
            _logService = logService;
            _logService.GetLogsEvent += OnGetLogs;
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
            _logService.GetLogs((int)SelectType, StarTime, EndTime);
        }
    }
}
