using Microsoft.Extensions.DependencyInjection;
using SwashbucklerDiary.Shared;
using SwashbucklerDiary.Wpf.Services;

namespace SwashbucklerDiary.Wpf.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDependencyInjection(this IServiceCollection services)
        {
            services.AddSingleton<IStaticWebAssets, StaticWebAssets>();
            return services;
        }
    }
}
