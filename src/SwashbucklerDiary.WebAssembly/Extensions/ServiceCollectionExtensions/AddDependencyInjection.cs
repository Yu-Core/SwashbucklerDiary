using SwashbucklerDiary.Shared;
using SwashbucklerDiary.WebAssembly.Services;

namespace SwashbucklerDiary.WebAssembly.Extensions
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
