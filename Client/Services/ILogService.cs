using Client.Models;
using Common.EventArgs;
using System;

namespace Client.Services
{
    public interface ILogService
    {
        EventHandler<LogEventArgs<Log>> GetLogsEvent { get; set; }
        void GetLogs(int selectType, DateTime starTime, DateTime endTime);
    }
}