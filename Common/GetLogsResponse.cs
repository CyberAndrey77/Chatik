using System;

namespace Common
{
    public class GetLogsResponse<T>
    {
        public int Type { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public GetLogsResponse(int type, DateTime start, DateTime end)
        {
            Type = type;
            Start = start;
            End = end;
        }

        public virtual MessageContainer GetContainer()
        {
            var container = new MessageContainer
            {
                Identifier = nameof(GetLogsResponse<T>),
                Payload = this
            };

            return container;
        }
    }
}
