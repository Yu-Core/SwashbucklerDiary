using SwashbucklerDiary.Rcl.Components;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Layout
{
    public partial class SponsorSupportSnackbar : MyComponentBase
    {
        private bool showSnackbar;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            AchievementService.UserStateChanged += OnUserStateChanged;
        }

        protected override async ValueTask DisposeAsyncCore()
        {
            await base.DisposeAsyncCore();

            AchievementService.UserStateChanged -= OnUserStateChanged;
        }

        protected async void OnUserStateChanged(UserStateModel userStateModel)
        {
            if (userStateModel.Type != Achievement.Diary)
            {
                return;
            }

            var flag = await SettingService.GetAsync("SponsorSupport", false);
            if (flag)
            {
                return;
            }

            if (userStateModel.Count == 100 || userStateModel.Count == 500 || userStateModel.Count == 1000)
            {
                await Task.Delay(1000);
                showSnackbar = true;
                await InvokeAsync(StateHasChanged);
                await SettingService.SetAsync("SponsorSupport", true);
            }
        }

        private void ToSupport()
        {
            showSnackbar = false;
            To("sponsor");
        }
    }
}