using Serilog;

namespace SwashbucklerDiary.Maui.Extensions
{
    public static class ServiceProviderExtensions
    {
        public static IServiceCollection AddMauiExceptionHandler(this IServiceCollection services)
        {
            MauiExceptions.UnhandledException += (sender, args) => Log.Error(args.ExceptionObject.ToString() ?? "no message");
            return services;
        }
    }
}
