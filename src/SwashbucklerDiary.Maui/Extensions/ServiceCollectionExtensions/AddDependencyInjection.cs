using SwashbucklerDiary.Maui.Essentials;
using SwashbucklerDiary.Maui.Services;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Extensions;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Repository;

namespace SwashbucklerDiary.Maui.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDependencyInjection(this IServiceCollection services)
        {
            services.AddSingleton<IStaticWebAssets, Essentials.StaticWebAssets>();
            services.AddSingleton<Rcl.Essentials.IPreferences, Essentials.Preferences>();

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

            services.AddSingleton<INavigateService, NavigateService>();
            services.AddSingleton<IIconService, Services.IconService>();
            services.AddSingleton<ISettingService, Services.SettingService>();
            services.AddSingleton<IAlertService, AlertService>();
            services.AddSingleton<II18nService, Services.I18nService>();
            services.AddSingleton<Rcl.Essentials.IVersionTracking, Essentials.VersionTracking>();
            services.AddSingleton<IPlatformIntegration, PlatformIntegration>();

            services.AddSingleton<IAppLifecycle, AppLifecycle>();
            services.AddSingleton<IThemeService, ThemeService>();
            services.AddSingleton<IAppFileManager, Essentials.AppFileManager>();
            services.AddSingleton<IAccessExternal, Services.AccessExternal>();
            services.AddSingleton<IMediaResourceManager, Services.MediaResourceManager>();
            services.AddSingleton<IDiaryFileManager, Services.DiaryFileManager>();
            services.AddSingleton<IAvatarService, Services.AvatarService>();
            services.AddScoped<ScreenshotJSModule>();
            services.AddScoped<Rcl.Essentials.IScreenshot, Essentials.Screenshot>();
            services.AddSingleton<IStorageSpace, Services.StorageSpace>();
            services.AddSingleton<IVersionUpdataManager, Services.VersionUpdataManager>();
            services.AddSingleton<IWebDAV, Essentials.WebDAV>();
            services.AddSingleton<ILANSenderService, LANSenderService>();
            services.AddSingleton<ILANReceiverService, LANReceiverService>();

            return services;
        }
    }
}
