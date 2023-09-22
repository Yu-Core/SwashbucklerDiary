using Serilog;

namespace SwashbucklerDiary.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMauiExceptionHandle(this IServiceCollection services)
        {
            MauiExceptions.UnhandledException += (sender, args) => { Log.Error(args.ExceptionObject.ToString()!); };
            return services;
        }
    }
}
