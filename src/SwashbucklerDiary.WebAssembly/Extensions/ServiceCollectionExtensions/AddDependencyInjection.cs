using Masa.Blazor;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Extensions;
using SwashbucklerDiary.Rcl.Repository;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.WebAssembly.Essentials;

namespace SwashbucklerDiary.WebAssembly.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDependencyInjection(this IServiceCollection services)
        {
            services.AddSingleton<IProxyService, Services.ProxyService>();
            services.AddSingleton<IStaticWebAssets, Essentials.StaticWebAssets>();
            services.AddSingleton<ISettingService, Services.SettingService>();
            services.AddSingleton<INavigateController, Essentials.NavigateController>();
            services.AddSingleton<IGlobalConfiguration, GlobalConfiguration>();
            services.AddSingleton<IAlertService, AlertService>();
            services.AddSingleton<II18nService>(sp =>
            {
                return (I18nService)sp.GetRequiredService<I18n>();
            });
            services.AddSingleton<IVersionTracking, VersionTracking>();
            services.AddSingleton<PlatformIntegrationJSModule>();
            services.AddSingleton<IPlatformIntegration, PlatformIntegration>();
            services.AddSingleton<IAppFileSystem, Essentials.AppFileSystem>();
            services.AddSingleton<IMediaResourceManager, Services.MediaResourceManager>();
            services.AddSingleton<BreakpointService>();

            services.AddRclDependencyInjection();

            services.AddSingleton<IDiaryRepository, DiaryRepository>();
            services.AddSingleton<ITagRepository, TagRepository>();
            services.AddSingleton<IUserAchievementRepository, UserAchievementRepository>();
            services.AddSingleton<IUserStateModelRepository, UserStateModelRepository>();
            services.AddSingleton<ILocationRepository, LocationRepository>();
            services.AddSingleton<ILogRepository, LogRepository>();
            services.AddSingleton<IResourceRepository, ResourceRepository>();

            services.AddSingleton<ITagService, TagService>();
            services.AddSingleton<IDiaryService, DiaryService>();
            services.AddSingleton<IAchievementService, AchievementService>();
            services.AddSingleton<ILocationService, LocationService>();
            services.AddSingleton<ILogService, LogService>();
            services.AddSingleton<IResourceService, ResourceService>();

            services.AddSingleton<AppLifecycleJSModule>();
            services.AddSingleton<IAppLifecycle, Essentials.AppLifecycle>();
            services.AddSingleton<SystemThemeJSModule>();
            services.AddSingleton<IThemeService, ThemeService>();
            services.AddSingleton<IAccessExternal, Services.AccessExternal>();

            services.AddSingleton<IDiaryFileManager, Services.DiaryFileManager>();
            services.AddSingleton<IAvatarService, Services.AvatarService>();
            services.AddScoped<IScreenshot, Screenshot>();
            services.AddSingleton<IVersionUpdataManager, Services.VersionUpdataManager>();

            return services;
        }
    }
}
