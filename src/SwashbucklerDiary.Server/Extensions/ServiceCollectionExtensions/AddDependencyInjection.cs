using SwashbucklerDiary.Server.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Server.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDependencyInjection(this IServiceCollection services)
        {
            services.AddScoped<IStaticWebAssets, StaticWebAssets>();
            return services;
        }
    }
}
