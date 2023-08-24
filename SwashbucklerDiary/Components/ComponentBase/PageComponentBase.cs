using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Components
{
    public class PageComponentBase : MyComponentBase, IDisposable
    {
        [Inject]
        protected ISettingsService SettingsService { get; set; } = default!;
        [Inject]
        protected IJSRuntime JS { get; set; } = default!;
        [Inject]
        protected IPlatformService PlatformService { get; set; } = default!;
        [Inject]
        protected IAchievementService AchievementService { get; set; } = default!;

        public void Dispose()
        {
            OnDispose();
            GC.SuppressFinalize(this);
        }

        protected virtual void NavigateToBack()
        {
            NavigateService.NavigateToBack();
        }

        protected virtual async Task HandleAchievements(AchievementType type)
        {
            var messages = await AchievementService.UpdateUserState(type);
            await AlertAchievements(messages);
        }

        protected Func<bool, Task> SettingChange(SettingType type)
        {
            return (bool value) => SettingsService.Save(type, value);
        }

        protected string? MSwitchTrackColor(bool value)
        {
            return value && Light ? "black" : null;
        }

        protected async Task AlertAchievements(List<string> messages)
        {
            bool achievementsAlert = await SettingsService.Get(SettingType.AchievementsAlert);
            if(!achievementsAlert)
            {
                return;
            }

            foreach (var item in messages)
            {
                await AlertService.Info(I18n.T("Achievement.AchieveAchievements"), I18n.T(item));
            }
        }

        protected virtual void OnDispose()
        {
        }
    }
}
