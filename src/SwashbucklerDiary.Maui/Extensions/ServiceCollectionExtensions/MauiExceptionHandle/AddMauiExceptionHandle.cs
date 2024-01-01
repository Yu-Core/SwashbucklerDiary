using Microsoft.Extensions.Logging;

namespace SwashbucklerDiary.Maui.Extensions
{
    public static class ServiceProviderExtensions
    {
        public static IServiceCollection AddMauiExceptionHandler(this IServiceCollection services)
        {
            var logger = services.BuildServiceProvider().GetRequiredService<ILogger<App>>();
            MauiExceptions.UnhandledException += (sender, args) => logger.LogError(message: args.ExceptionObject.ToString());
            return services;
        }
    }
}
