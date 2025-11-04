using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Services;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class UpdateDialog : DialogComponentBase
    {
        private bool notPrompt;

        [Inject]
        private IVersionUpdataManager VersionUpdataManager { get; set; } = default!;

        [Parameter]
        public Release? Value { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            UpdateSettings();
        }

        private void UpdateSettings()
        {
            notPrompt = SettingService.Get(s => s.UpdateNotPrompt);
        }

        private async Task ToUpdate()
        {
            await InternalVisibleChanged(false);
            await VersionUpdataManager.ToUpdate();
        }

        private async Task ChangeUpdateNotPrompt(bool value)
        {
            await SettingService.SetAsync(s => s.UpdateNotPrompt, value);
        }
    }
}
