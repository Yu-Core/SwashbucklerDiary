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
            services.AddScoped<IResourceService, Services.ResourceService>();

            services.AddScoped<INavigateService, NavigateService>();
            services.AddScoped<IIconService, Services.IconService>();
            services.AddScoped<ISettingService, Services.SettingService>();
            services.AddScoped<IAlertService, AlertService>();
            services.AddScoped<II18nService, Services.I18nService>();
            services.AddScoped<IVersionTracking, VersionTracking>();
            services.AddScoped<IPlatformIntegration, PlatformIntegration>();

            services.AddScoped<IAppLifecycle, AppLifecycle>();
            services.AddScoped<SystemThemeJSModule>();
            services.AddScoped<IThemeService, ThemeService>();
            services.AddScoped<IAppFileManager, Essentials.AppFileManager>();
            services.AddScoped<IAccessExternal, Services.AccessExternal>();
            services.AddScoped<IMediaResourceManager, Services.MediaResourceManager>();
            services.AddScoped<IDiaryFileManager, Services.DiaryFileManager>();
            services.AddScoped<IAvatarService, Services.AvatarService>();
            services.AddScoped<ScreenshotJSModule>();
            services.AddScoped<IScreenshot, Screenshot>();
            services.AddScoped<IStorageSpace, Services.StorageSpace>();
            services.AddScoped<IVersionUpdataManager, Services.VersionUpdataManager>();

            return services;
        }
    }
}
