using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;

namespace LogHelper
{
    public static class LogHelperExtensions
    {
        /// <summary>
        /// 註冊呼叫服務
        /// </summary>
        /// <param name="services"></param>
        public static void AddSerilog(this WebApplicationBuilder builder, IConfigurationRoot config)
        {
            Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(config.GetSection("Serilog"))
                                                  .WriteTo.Console()
                                                  .CreateLogger();

            try
            {
                Log.Information("Starting web application");
                builder.Logging.ClearProviders();
                builder.Host.UseSerilog((context, services, configuration) => configuration
                                                                              .ReadFrom.Configuration(context.Configuration)
                                                                              .ReadFrom.Services(services)
                                                                              .Enrich.FromLogContext()
                                                                              .WriteTo.Console(), writeToProviders: true);

            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
