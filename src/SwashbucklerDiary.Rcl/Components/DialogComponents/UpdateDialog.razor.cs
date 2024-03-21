using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class UpdateDialog : DialogComponentBase
    {
        private bool notPrompt;

        [Inject]
        private IVersionUpdataManager VersionUpdataManager { get; set; } = default!;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            UpdateSettings();
        }

        private void UpdateSettings()
        {
            notPrompt = SettingService.Get<bool>(Setting.UpdateNotPrompt);
        }

        private async Task ToUpdate()
        {
            await InternalVisibleChanged(false);
            await VersionUpdataManager.ToUpdate();
        }

        private async Task ChangeUpdateNotPrompt(bool value)
        {
            await SettingService.Set(Setting.UpdateNotPrompt, value);
        }
    }
}
