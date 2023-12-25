using Microsoft.Extensions.DependencyInjection;
using SwashbucklerDiary.Photino.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Photino.Extensions
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
