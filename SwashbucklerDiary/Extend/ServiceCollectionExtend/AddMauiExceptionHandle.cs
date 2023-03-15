using Serilog;

namespace SwashbucklerDiary.Extend
{
    public static partial class ServiceCollectionExtend
    {
        public static IServiceCollection AddMauiExceptionHandle(this IServiceCollection services)
        {
            MauiExceptions.UnhandledException += (sender, args) => { Log.Error(args.ExceptionObject.ToString()!); };
            return services;
        }
    }
}
