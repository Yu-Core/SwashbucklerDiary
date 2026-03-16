using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSerilogConfig(this IServiceCollection services, string databasePath)
        {
            Log.Logger = new LoggerConfiguration()
                            .MinimumLevel.Information()
                            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                 .Enrich.FromLogContext()
                 .Enrich.WithThreadId()
#if DEBUG
                 .WriteTo.Debug()
#endif
                 .WriteTo.SQLite(databasePath, options =>
                 {
                     options.TableName = nameof(LogModel);
                     options.AutoCreateDatabase = true;
                     options.RetentionPeriod = TimeSpan.FromDays(7);
                     options.RetentionCount = 2000;
                 })
                 .CreateLogger();

            services.AddLogging(logging =>
            {
                logging.AddSerilog(dispose: true);
            });
            return services;
        }
    }
}
