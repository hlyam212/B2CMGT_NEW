using System;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace SeriLogHelper.Services
{
    public class LogService : ILoggerProvider
    {
        private readonly ILogger _logger;

        public LogService()
        {
        }

        public ILogger GetLogger()
        {
            throw new NotImplementedException();
        }
    }
}
