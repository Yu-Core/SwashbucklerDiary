using Microsoft.Extensions.Logging;

namespace SwashbucklerDiary.Maui.Extensions
{
    public static class ServiceProviderExtensions
    {
        public static IServiceProvider AddMauiExceptionHandle(this IServiceProvider services)
        {
            var logger = services.GetRequiredService<ILogger<App>>();
            MauiExceptions.UnhandledException += (sender, args) => { logger.LogError(message: args.ExceptionObject.ToString()); };
            return services;
        }
    }
}
