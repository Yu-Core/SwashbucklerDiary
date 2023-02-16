using NoDecentDiary.IServices;
using NoDecentDiary.Services;

namespace NoDecentDiary.Extend
{
    public static class IOCExtend
    {
        public static IServiceCollection AddCustomIOC(this IServiceCollection services)
        {
            services.AddSingleton<ITagService, TagService>();
            services.AddSingleton<IDiaryService, DiaryService>();
            services.AddSingleton<IDiaryTagService, DiaryTagService>();
            services.AddSingleton<INavigateService, NavigateService>();
            services.AddSingleton<IconService>();
            services.AddSingleton<ISettingsService, SettingsService>();
            services.AddSingleton<ISystemService, SystemService>();
            return services;
        }
    }
}
