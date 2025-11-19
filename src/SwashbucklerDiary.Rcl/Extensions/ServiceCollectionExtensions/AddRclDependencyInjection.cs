using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SwashbucklerDiary.Rcl.Components;
using SwashbucklerDiary.Rcl.Services;

namespace SwashbucklerDiary.Rcl.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRclDependencyInjection(this IServiceCollection services)
        {
            services.TryAddScoped<InputJSModule>();
            services.TryAddScoped<AudioInterop>();
            services.TryAddScoped<ScreenshotJSModule>();
            return services;
        }
    }
}
