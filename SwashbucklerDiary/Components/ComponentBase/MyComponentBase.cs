using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Components
{
    public abstract class MyComponentBase : ComponentBase
    {
        [Inject]
        protected INavigateService NavigateService { get; set; } = default!;
        [Inject]
        protected II18nService I18n { get; set; } = default!;
        [Inject]
        protected IAlertService AlertService { get; set; } = default!;
        [Inject]
        protected IThemeService ThemeService { get; set; } = default!;
        [Inject]
        protected IAchievementService AchievementService { get; set; } = default!;
        [Inject]
        protected ISettingsService SettingsService { get; set; } = default!;

        protected NavigationManager Navigation => NavigateService.Navigation!;
        protected bool Dark => ThemeService.Dark;
        protected bool Light => ThemeService.Light;

        protected Task ToDo()
        {
            return AlertService.Info(I18n.T("ToDo.Title"), I18n.T("ToDo.Content"));
        }

        protected void To(string url)
        {
            NavigateService.NavigateTo(url);
        }

        protected virtual async Task HandleAchievements(AchievementType type)
        {
            var messages = await AchievementService.UpdateUserState(type);
            await AlertAchievements(messages);
        }

        protected async Task AlertAchievements(List<string> messages)
        {
            bool achievementsAlert = await SettingsService.Get(SettingType.AchievementsAlert);
            if (!achievementsAlert)
            {
                return;
            }

            foreach (var item in messages)
            {
                await AlertService.Info(I18n.T("Achievement.AchieveAchievements"), I18n.T(item));
            }
        }
    }
}
