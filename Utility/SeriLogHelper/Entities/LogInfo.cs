using Microsoft.Extensions.Logging;

namespace SeriLogHelper.Entities
{
    public class LogInfo
    {
        public DateTime TimeStamp { get; set; }
        public LogLevel LogLevel { get; set; }
        public string Message { get; set; }
        public string Exception { get; set; }
        public string EventId { get; set; }
        public IDictionary<string, object> Properties { get; set; }
    }
}
