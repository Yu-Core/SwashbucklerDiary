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
            services.AddScoped<IStaticWebAssets, Essentials.StaticWebAssets>();
            services.AddScoped<IPreferences, Preferences>();
            services.AddScoped<ISettingService, Services.SettingService>();
            services.AddScoped<INavigateController, Essentials.NavigateController>();
            services.AddScoped<IIconService, Services.IconService>();
            services.AddScoped<IPopupServiceHelper, PopupServiceHelper>();
            services.AddScoped<II18nService, Services.I18nService>();
            services.AddScoped<IVersionTracking, VersionTracking>();
            services.AddScoped<IPlatformIntegration, PlatformIntegration>();
            services.AddScoped<IAppFileSystem, Essentials.AppFileSystem>();
            services.AddScoped<IMediaResourceManager, Services.MediaResourceManager>();
            services.AddScoped<MasaBlazorHelper>();

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
            services.AddScoped<IAchievementService, Services.AchievementService>();
            services.AddScoped<ILocationService, LocationService>();
            services.AddScoped<ILogService, LogService>();
            services.AddScoped<IResourceService, ResourceService>();

            services.AddScoped<IAppLifecycle, AppLifecycle>();
            services.AddScoped<SystemThemeJSModule>();
            services.AddScoped<IThemeService, ThemeService>();
            services.AddScoped<IAccessExternal, Services.AccessExternal>();

            services.AddScoped<IDiaryFileManager, Services.DiaryFileManager>();
            services.AddScoped<IAvatarService, Services.AvatarService>();
            services.AddScoped<ScreenshotJSModule>();
            services.AddScoped<IScreenshot, Screenshot>();
            services.AddScoped<IVersionUpdataManager, Services.VersionUpdataManager>();

            return services;
        }
    }
}
