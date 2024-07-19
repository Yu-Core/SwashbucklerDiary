using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Components;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class SettingPage : ImportantComponentBase
    {
        private bool showClearCache;

        private bool showStatisticsCard;

        private string? cacheSize;

        [Inject]
        private IStorageSpace StorageSpace { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            UpdateCacheSize();
        }

        protected override void ReadSettings()
        {
            base.ReadSettings();

            showStatisticsCard = SettingService.Get<bool>(Setting.StatisticsCard);
        }

        private void UpdateCacheSize()
        {
            cacheSize = StorageSpace.GetCacheSize();
        }

        private async Task ClearCache()
        {
            showClearCache = false;
            StorageSpace.ClearCache();
            UpdateCacheSize();
            await PopupServiceHelper.Success(I18n.T("Storage.ClearSuccess"));
        }
    }
}
