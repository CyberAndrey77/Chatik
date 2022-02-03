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
using NLog;

namespace Client.Services
{
    public class LogService : ILogService
    {
        private ITransport _transport;
        private readonly ILogger _logger;

        public EventHandler<LogEventArgs<Log>> GetLogsEvent { get; set; }

        public LogService(ITransport transport)
        {
            _transport = transport;

            _logger = LogManager.GetCurrentClassLogger();
            _transport.Subscribe(EnumKey.LogKey, OnGetLogs);
        }

        public void GetLogs(int selectType, DateTime starTime, DateTime endTime)
        {
            _transport.GetLogs(new GetLogsResponse<Log>(selectType, starTime, endTime).GetContainer());
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
                _logger.Error($"Answer from server {message}:{message.Identifier} is null");
                return;
            }
            GetLogsEvent?.Invoke(this, new LogEventArgs<Log>() { LogsList = logs.LogsList });
        }
    }
}
