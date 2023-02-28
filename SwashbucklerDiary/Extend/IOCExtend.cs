using CommunityToolkit.Maui.Storage;
using SwashbucklerDiary.IRepository;
using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Repository;
using SwashbucklerDiary.Services;

namespace SwashbucklerDiary.Extend
{
    public static class IOCExtend
    {
        public static IServiceCollection AddCustomIOC(this IServiceCollection services)
        {
            services.AddSingleton<IFolderPicker>(FolderPicker.Default);

            services.AddSingleton<IDiaryRepository,DiaryRepository>();
            services.AddSingleton<ITagRepository,TagRepository>();
            services.AddSingleton<IDiaryTagRepository,DiaryTagRepository>();
            services.AddSingleton<IUserAchievementRepository, UserAchievementRepository>();
            services.AddSingleton<IUserStateModelRepository, UserStateModelRepository>();

            services.AddSingleton<ITagService, TagService>();
            services.AddSingleton<IDiaryService, DiaryService>();
            services.AddSingleton<IDiaryTagService, DiaryTagService>();
            services.AddSingleton<INavigateService, NavigateService>();
            services.AddSingleton<IconService>();
            services.AddSingleton<ISettingsService, SettingsService>();
            services.AddSingleton<ISystemService, SystemService>();
            services.AddSingleton<IAchievementService, AchievementService>();
            return services;
        }
    }
}
