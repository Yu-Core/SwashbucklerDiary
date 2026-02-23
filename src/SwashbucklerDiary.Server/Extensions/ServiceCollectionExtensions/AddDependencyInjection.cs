using Masa.Blazor;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Extensions;
using SwashbucklerDiary.Rcl.Repository;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Rcl.Web.Essentials;
using SwashbucklerDiary.Server.Services;

namespace SwashbucklerDiary.Server.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDependencyInjection(this IServiceCollection services)
        {
            services.AddSingleton<IApiAuthService, ApiAuthService>();
            services.AddScoped<ApiAuthJSModule>();

            services.AddSingleton<IProxyService, Services.ProxyService>();
            services.AddSingleton<IStaticWebAssets, Essentials.StaticWebAssets>();
            services.AddScoped<IPreferences, Essentials.Preferences>();
            services.AddScoped<ISettingService, Services.SettingService>();
            services.AddSingleton<RouteMatcher>(_ => new RouteMatcher(Routes.Assemblies));
            services.AddScoped<INavigateController, NavigateController>();
            services.AddSingleton<IGlobalConfiguration, GlobalConfiguration>();
            services.AddScoped<IAlertService, AlertService>();
            services.AddScoped<II18nService>(sp =>
            {
                return (I18nService)sp.GetRequiredService<I18n>();
            });
            services.AddSingleton<IVersionTracking, Essentials.VersionTracking>();
            services.AddScoped<Essentials.PlatformIntegrationJSModule>();
            services.AddScoped<IPlatformIntegration, Essentials.PlatformIntegration>();
            services.AddSingleton<IAppFileSystem, Essentials.AppFileSystem>();
            services.AddScoped<IMediaResourceManager, Rcl.Web.Services.MediaResourceManager>();
            services.AddScoped<BreakpointService>();
            services.AddScoped<IAppLockService, AppLockService>();

            services.AddRclDependencyInjection();

            services.AddScoped<IDiaryRepository, DiaryRepository>();
            services.AddScoped<ITagRepository, TagRepository>();
            services.AddScoped<IUserAchievementRepository, UserAchievementRepository>();
            services.AddScoped<IUserStateModelRepository, UserStateModelRepository>();
            services.AddScoped<ILocationRepository, LocationRepository>();
            services.AddScoped<ILogRepository, LogRepository>();
            services.AddScoped<IResourceRepository, ResourceRepository>();

            services.AddScoped<ITagService, TagService>();
            services.AddScoped<IDiaryService, DiaryService>();
            services.AddScoped<IAchievementService, AchievementService>();
            services.AddScoped<ILocationService, LocationService>();
            services.AddScoped<ILogService, LogService>();
            services.AddScoped<IResourceService, ResourceService>();

            services.AddScoped<AppLifecycleJSModule>();
            services.AddScoped<IAppLifecycle, Rcl.Web.Essentials.AppLifecycle>();
            services.AddScoped<SystemThemeJSModule>();
            services.AddScoped<IThemeService, ThemeService>();
            services.AddScoped<IAccessExternal, Rcl.Web.Services.AccessExternal>();

            services.AddScoped<IDiaryFileManager, Services.DiaryFileManager>();
            services.AddScoped<IAvatarService, Services.AvatarService>();
            services.AddScoped<IScreenshot, Screenshot>();
            services.AddScoped<IVersionUpdataManager, Services.VersionUpdataManager>();

            return services;
        }
    }
}
