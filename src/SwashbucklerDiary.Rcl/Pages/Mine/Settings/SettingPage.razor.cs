using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Components;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class SettingPage : ImportantComponentBase
    {
        private bool privacy;

        private string? privatePassword;

        private bool showPPSet;

        private bool showPPInput;

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

            UpdatePrivatePassword();
            privacy = SettingService.Get<bool>(Setting.PrivacyMode);
            showStatisticsCard = SettingService.Get<bool>(Setting.StatisticsCard);
        }

        private async Task PrivacyChange(bool value)
        {
            privacy = value;
            await SettingService.Set(Setting.PrivacyMode, value);
            if (!value)
            {
                await AlertService.Success(I18n.T("Setting.Safe.CamouflageSuccess"));
            }
        }

        private string? GetDisplayPrivacy()
        {
            return privacy ? I18n.T("Setting.Safe.DisplayPrivacy.Name") : I18n.T("Setting.Safe.Mask.Name");
        }

        private async Task SetPassword(string value)
        {
            showPPSet = false;
            privatePassword = value;
            await SettingService.Set(Setting.PrivatePassword, value.MD5Encrytp32());
            await AlertService.Success(I18n.T("Setting.Safe.PrivatePasswordSetSuccess"));
        }

        private async Task InputPassword(string value)
        {
            showPPInput = false;
            if (string.IsNullOrWhiteSpace(value))
            {
                return;
            }

            UpdatePrivatePassword();
            if (privatePassword != value.MD5Encrytp32())
            {
                await AlertService.Error(I18n.T("Setting.Safe.PasswordError"));
                return;
            }

            await PrivacyChange(true);
        }

        private async Task PrivacyClick()
        {
            UpdatePrivatePassword();
            if (!string.IsNullOrEmpty(privatePassword) && !privacy)
            {
                showPPInput = true;
                return;
            }

            await PrivacyChange(!privacy);
        }

        private void UpdatePrivatePassword()
        {
            privatePassword = SettingService.Get<string>(Setting.PrivatePassword);
        }

        private string? GetPrivatePasswordSetState()
            => string.IsNullOrEmpty(privatePassword) ? I18n.T("Setting.Safe.NotSetPassword") : null;

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
    }
}
