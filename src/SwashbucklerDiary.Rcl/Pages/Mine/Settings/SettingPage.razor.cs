using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Components;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class SettingPage : ImportantComponentBase
    {
        private bool privacyMode;

        private bool hidePrivacyModeEntrance;

        private bool showClearCache;

        private bool showStatisticsCard;

        private bool showprivacyModeEntrancePasswordDialog;

        private string? privacyModeEntrancePassword;

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

            privacyMode = SettingService.GetTemp<bool>(TempSetting.PrivacyMode);
            hidePrivacyModeEntrance = SettingService.Get<bool>(Setting.HidePrivacyModeEntrance);
            privacyModeEntrancePassword = SettingService.Get<string>(Setting.PrivacyModeEntrancePassword);
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
            await AlertService.Success(I18n.T("Storage.ClearSuccess"));
        }

        private void TryToPrivacyMode()
        {
            if (privacyMode || string.IsNullOrEmpty(privacyModeEntrancePassword))
            {
                ToPrivacyMode();
            }
            else
            {
                showprivacyModeEntrancePasswordDialog = true;
            }
        }

        private void ToPrivacyMode()
        {
            SettingService.SetTemp<bool>(TempSetting.AllowEnterPrivacyMode, true);
            To("privacyMode");
        }

        private async Task InputPassword(string value)
        {
            showprivacyModeEntrancePasswordDialog = false;
            if (string.IsNullOrWhiteSpace(value))
            {
                return;
            }

            string salt = Setting.PrivacyModeEntrancePassword.ToString();
            if (privacyModeEntrancePassword != (value + salt).MD5Encrytp32())
            {
                await AlertService.Error(I18n.T("PrivacyMode.PasswordError"));
                return;
            }

            ToPrivacyMode();
        }
    }
}
