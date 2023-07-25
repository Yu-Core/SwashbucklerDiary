using SwashbucklerDiary.IRepository;
using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Repository;
using SwashbucklerDiary.Services;

namespace SwashbucklerDiary.Extend
{
    public static partial class ServiceCollectionExtend
    {
        public static IServiceCollection AddCustomIOC(this IServiceCollection services)
        {
            //仓储相关
            services.AddSingleton<IDiaryRepository,DiaryRepository>();
            services.AddSingleton<ITagRepository,TagRepository>();
            services.AddSingleton<IDiaryTagRepository,DiaryTagRepository>();
            services.AddSingleton<IUserAchievementRepository, UserAchievementRepository>();
            services.AddSingleton<IUserStateModelRepository, UserStateModelRepository>();
            services.AddSingleton<ILocationRepository, LocationRepository>();
            services.AddSingleton<ILogRepository, LogRepository>();
            //数据服务相关
            services.AddSingleton<ITagService, TagService>();
            services.AddSingleton<IDiaryService, DiaryService>();
            services.AddSingleton<IDiaryTagService, DiaryTagService>();
            services.AddSingleton<IAchievementService, AchievementService>();
            services.AddSingleton<ILocationService, LocationService>();
            services.AddSingleton<ILogService, LogService>();
            //功能服务相关
            services.AddSingleton<INavigateService, NavigateService>();
            services.AddSingleton<IconService>();
            services.AddSingleton<ISettingsService, SettingsService>();
            services.AddSingleton<IPlatformService, PlatformService>();
            services.AddSingleton<IAlertService, AlertService>();
            services.AddSingleton<II18nService, I18nService>();
            services.AddSingleton<IThemeService, ThemeService>();
            services.AddSingleton<IStateService, StateService>();
            services.AddSingleton<ILANService, LANService>();
            services.AddSingleton<IAppDataService, AppDataService>();
            return services;
        }
    }
}
