using BlazorComponent;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Components
{
    public class PageComponentBase : MyComponentBase
    {
        [Inject]
        protected ISettingsService SettingsService { get; set; } = default!;
        [Inject]
        protected IJSRuntime JS { get; set; } = default!;
        [Inject]
        protected ISystemService SystemService { get; set; } = default!;
        [Inject]
        protected IAchievementService AchievementService { get; set; } = default!;

        protected void NavigateToBack()
        {
            NavigateService.NavigateToBack();
        }

        protected virtual async Task HandleAchievements(AchievementType type)
        {
            var messages = await AchievementService.UpdateUserState(type);
            foreach (var item in messages)
            {
                await PopupService.ToastAsync(it =>
                {
                    it.Type = AlertTypes.Success;
                    it.Title = I18n.T("Achievement.AchieveAchievements");
                    it.Content = I18n.T(item);
                });
            }
        }
    }
}
