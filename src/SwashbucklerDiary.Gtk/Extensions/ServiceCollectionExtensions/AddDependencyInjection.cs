using Microsoft.Extensions.DependencyInjection;
using SwashbucklerDiary.Gtk.Essentials;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Extensions;
using SwashbucklerDiary.Rcl.Repository;
using SwashbucklerDiary.Rcl.Services;

namespace SwashbucklerDiary.Gtk.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDependencyInjection(this IServiceCollection services)
        {
            services.AddSingleton<GtkSystemThemeManager>();

            services.AddSingleton<IStaticWebAssets, Essentials.StaticWebAssets>();
            services.AddSingleton<ISettingService, Services.SettingService>();
            services.AddSingleton<INavigateController, Essentials.NavigateController>();
            services.AddSingleton<IGlobalConfiguration, GlobalConfiguration>();
            services.AddSingleton<IPopupServiceHelper, PopupServiceHelper>();
            services.AddSingleton<II18nService, I18nService>();
            services.AddSingleton<Rcl.Essentials.IVersionTracking, Essentials.VersionTracking>();
            services.AddSingleton<IPlatformIntegration, PlatformIntegration>();
            services.AddSingleton<IAppFileSystem, Essentials.AppFileSystem>();
            services.AddSingleton<IMediaResourceManager, Services.MediaResourceManager>();
            services.AddSingleton<MasaBlazorHelper>();

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

            services.AddSingleton<IAppLifecycle, AppLifecycle>();
            services.AddSingleton<IThemeService, ThemeService>();
            services.AddSingleton<IAccessExternal, Services.AccessExternal>();
            services.AddSingleton<IDiaryFileManager, Services.DiaryFileManager>();
            services.AddSingleton<IAvatarService, Services.AvatarService>();
            services.AddScoped<ScreenshotJSModule>();
            services.AddScoped<Rcl.Essentials.IScreenshot, Essentials.Screenshot>();
            services.AddSingleton<IVersionUpdataManager, Services.VersionUpdataManager>();
            services.AddSingleton<IWebDAV, WebDAV>();
            services.AddSingleton<ILANSenderService, LANSenderService>();
            services.AddSingleton<ILANReceiverService, LANReceiverService>();

            return services;
        }
    }
}
