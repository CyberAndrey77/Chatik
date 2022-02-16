using System;
using System.Collections.Generic;

namespace Common
{
    public class GetLogsRequest<T> : GetLogsResponse<T>
    {
        public List<T> LogsList { get; set; }
        public GetLogsRequest(List<T> logsList, int type, DateTime start, DateTime end) : base(type, start, end)
        {
            LogsList = logsList;
        }

        public override MessageContainer GetContainer()
        {
            var container = new MessageContainer
            {
                Identifier = nameof(GetLogsRequest<T>),
                Payload = this
            };

            return container;
        }
    }
}
