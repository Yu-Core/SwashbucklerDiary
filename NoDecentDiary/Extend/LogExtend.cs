using NoDecentDiary.Config;
using Serilog;
using Serilog.Events;

namespace NoDecentDiary.Extend
{
    public static class LogExtend
    {
        public static IServiceCollection AddLog(this IServiceCollection services)
        {
            if (!Directory.Exists(SerilogConstants.folderPath))
            {
                Directory.CreateDirectory(SerilogConstants.folderPath);
            }

            Log.Logger = new LoggerConfiguration()
#if DEBUG
                            .MinimumLevel.Debug()
#else
                            .MinimumLevel.Information()
#endif
                            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                 .Enrich.FromLogContext()
#if DEBUG
                 .WriteTo.Debug()
#endif
                 .WriteTo.Async(c => c.File(path: SerilogConstants.filePath))
                 .CreateLogger();

            services.AddLogging(logging =>
            {
                logging.AddSerilog(dispose: true);
            });
            return services;
        }
    }
}
