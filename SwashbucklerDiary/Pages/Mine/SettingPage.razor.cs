using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Components;
using SwashbucklerDiary.Extend;
using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Pages
{
    public partial class SettingPage : PageComponentBase
    {
        private bool Privacy;
        private string? PrivatePassword;
        private bool ShowPPSet;
        private bool ShowPPInput;
        private bool ShowClearCache;
        private string? CacheSize;

        [Inject]
        private IAppDataService AppDataService { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            await LoadSettings();
            await UpdatePrivatePassword();
            UpdateCacheSize();
            await base.OnInitializedAsync();
        }

        private async Task LoadSettings()
        {
            Privacy = await SettingsService.Get(SettingType.PrivacyMode);
        }

        private async Task PrivacyChange(bool value)
        {
            Privacy = value;
            await SettingsService.Save(SettingType.PrivacyMode, value);
            if (!value)
            {
                await AlertService.Success(I18n.T("Setting.Safe.CamouflageSuccess"));
            }
        }

        private string? GetDisplayPrivacy()
        {
            return Privacy ? I18n.T("Setting.Safe.DisplayPrivacy.Name") : I18n.T("Setting.Safe.Mask.Name");
        }

        private async Task SetPassword(string value)
        {
            ShowPPSet = false;
            PrivatePassword = value;
            await SettingsService.Save(SettingType.PrivatePassword, value.MD5Encrytp32());
            await AlertService.Success(I18n.T("Setting.Safe.PrivatePasswordSetSuccess"));
        }

        private async Task InputPassword(string value)
        {
            ShowPPInput = false;
            if (string.IsNullOrWhiteSpace(value))
            {
                return;
            }
            await UpdatePrivatePassword();
            if (PrivatePassword != value.MD5Encrytp32())
            {
                await AlertService.Error(I18n.T("Setting.Safe.PasswordError"));
                return;
            }
            await PrivacyChange(true);
        }

        private async Task PrivacyClick()
        {
            await UpdatePrivatePassword();
            if (!string.IsNullOrEmpty(PrivatePassword) && !Privacy)
            {
                ShowPPInput = true;
                return;
            }

            await PrivacyChange(!Privacy);
        }

        private async Task UpdatePrivatePassword()
        {
            PrivatePassword = await SettingsService.Get(SettingType.PrivatePassword);
        }

        private string? GetPrivatePasswordSetState()
            => string.IsNullOrEmpty(PrivatePassword) ? I18n.T("Setting.Safe.NotSetPassword") : null;

        private void UpdateCacheSize()
        {
            CacheSize = AppDataService.GetCacheSize();
        }

        private async Task ClearCache()
        {
            ShowClearCache = false;
            AppDataService.ClearCache();
            UpdateCacheSize(); 
            await AlertService.Success(I18n.T("Storage.ClearSuccess"));
        }
    }
}
