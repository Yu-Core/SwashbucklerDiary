using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Extensions;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Repository;
using SwashbucklerDiary.Services;
using SwashbucklerDiary.WebAssembly.Essentials;
using SwashbucklerDiary.WebAssembly.Services;

namespace SwashbucklerDiary.WebAssembly.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDependencyInjection(this IServiceCollection services)
        {
            services.AddScoped<IStaticWebAssets, Essentials.StaticWebAssets>();

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

            services.AddScoped<INavigateService, NavigateService>();
            services.AddScoped<IIconService, Services.IconService>();
            services.AddScoped<Rcl.Essentials.IPreferences, Essentials.Preferences>();
            services.AddScoped<IAlertService, AlertService>();
            services.AddScoped<II18nService, Services.I18nService>();
            services.AddScoped<IPlatformIntegration, PlatformIntegration>();
            services.AddScoped<IVersionTracking, VersionTracking>();

            services.AddScoped<IAppLifecycle, AppLifecycle>();
            services.AddScoped<SystemThemeJSModule>();
            services.AddScoped<IThemeService, ThemeService>();
            services.AddScoped<IAppFileManager, Essentials.AppFileManager>();
            services.AddScoped<IAccessExternal, Services.AccessExternal>();
            services.AddScoped<IMediaResourceManager, Services.MediaResourceManager>();
            services.AddScoped<IDiaryFileManager, Services.DiaryFileManager>();
            services.AddScoped<IAvatarService, AvatarService>();
            services.AddScoped<ScreenshotJSModule>();
            services.AddScoped<Rcl.Essentials.IScreenshot, Essentials.Screenshot>();
            services.AddScoped<IStorageSpace, Services.StorageSpace>();
            services.AddScoped<IVersionUpdataManager, VersionUpdataManager>();

            return services;
        }
    }
}
