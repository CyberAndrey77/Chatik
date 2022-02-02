using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Enums;
using Client.Models;
using Client.NetWork;
using Common;
using Common.EventArgs;
using Newtonsoft.Json.Linq;

namespace Client.Services
{
    public class LogService : ILogService
    {
        private ITransport _transport;

        public EventHandler<LogEventArgs<Log>> GetLogsEvent { get; set; }

        public LogService(ITransport transport)
        {
            _transport = transport;
            _transport.Subscribe(EnumKey.LogKey, OnGetLogs);
        }

        public void GetLogs(int selectType, DateTime starTime, DateTime endTime)
        {
            //_transport.GetLogs(selectType, starTime, endTime);
        }

        private void OnGetLogs(MessageContainer message)
        {
            if (message.Identifier != nameof(GetLogsRequest<Log>))
            {
                return;
            }
            var logs = ((JObject)message.Payload).ToObject(typeof(GetLogsRequest<Log>)) as GetLogsRequest<Log>;
            if (logs == null)
            {
                //TODO excep
            }
            GetLogsEvent?.Invoke(this, new LogEventArgs<Log>() { LogsList = logs.LogsList });
        }
    }
}
