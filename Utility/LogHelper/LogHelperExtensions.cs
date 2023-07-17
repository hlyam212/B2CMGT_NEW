using NLog.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using Microsoft.Extensions.Logging;

namespace LogHelper
{
    public static class LogHelperExtensions
    {
        /// <summary>
        /// 註冊呼叫服務
        /// </summary>
        /// <param name="services"></param>
        public static void AddNLog(this WebApplicationBuilder builder)
        {
            var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
            logger.Debug("init main");
            builder.Logging.ClearProviders();
            builder.Host.UseNLog();
            builder.Services.AddScoped<LogHelper.Nlog.ILog, LogHelper.Nlog.LogModule>();
        }
    }
}
