using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Components;
using SwashbucklerDiary.Rcl.Essentials;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class SettingPage : ImportantComponentBase
    {
        private bool showClearCache;

        private bool showStatisticsCard;

        private string? cacheSize;

        [Inject]
        private IAppFileSystem AppFileSystem { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            UpdateCacheSize();
        }

        protected override void ReadSettings()
        {
            base.ReadSettings();

            showStatisticsCard = SettingService.Get(s => s.StatisticsCard);
        }

        private void UpdateCacheSize()
        {
            cacheSize = AppFileSystem.GetCacheSize();
        }

        private async Task ClearCache()
        {
            showClearCache = false;
            AppFileSystem.ClearCache();
            UpdateCacheSize();
            await PopupServiceHelper.Success(I18n.T("Clean up successfully"));
        }
    }
}
