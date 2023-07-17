using Serilog;

namespace SeriLogHelper.Services
{
    public interface ILoggerProvider
    {
        ILogger GetLogger();
    }
}
