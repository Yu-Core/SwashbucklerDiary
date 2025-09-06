using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SwashbucklerDiary.Rcl.Components;

namespace SwashbucklerDiary.Rcl.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRclDependencyInjection(this IServiceCollection services)
        {
            services.TryAddScoped<PanzoomJSModule>();
            services.TryAddScoped<MarkdownJSModule>();
            services.TryAddScoped<InputJSModule>();
            services.TryAddScoped<AudioInterop>();
            services.TryAddScoped<SwiperJsModule>();
            services.TryAddScoped<MarkdownPreviewJSModule>();
            services.TryAddScoped<PreviewMediaElementJSModule>();
            services.TryAddScoped<BetterSearchJSModule>();
            services.TryAddScoped<BackTopButtonJSModule>();
            services.TryAddScoped<WaterfallJSModule>();
            services.TryAddScoped<GestureUnlockJSModule>();
            return services;
        }
    }
}
