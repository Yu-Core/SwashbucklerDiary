using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Components;
using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Pages
{
    public partial class UserPage : PageComponentBase,ITempCustomSchemeAssist
    {
        private string? UserName;
        private string? Sign;
        private string? Avatar;
        private bool ShowAvatar;
        private bool showLoading;

        [Inject]
        private IAppDataService AppDataService { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await LoadSettings();
            await SetAvatar();
        }

        private bool ShowUserName { get; set; }
        private bool ShowSign { get; set; }

        private async Task LoadSettings()
        {
            UserName = await SettingsService.Get(SettingType.UserName, I18n.T("AppName"));
            Sign = await SettingsService.Get(SettingType.Sign, I18n.T("Mine.Sign"));
        }

        private async Task SaveSign(string tagName)
        {
            ShowSign = false;
            if (!string.IsNullOrWhiteSpace(tagName) && tagName != Sign)
            {
                Sign = tagName;
                await SettingsService.Save(SettingType.Sign, Sign);
            }
            await HandleAchievements(AchievementType.Sign);
        }

        private async Task SaveUserName(string tagName)
        {
            ShowUserName = false;
            if (!string.IsNullOrWhiteSpace(tagName) && tagName != UserName)
            {
                UserName = tagName;
                await SettingsService.Save(SettingType.UserName, UserName);
            }
            await HandleAchievements(AchievementType.NickName);
        }

        private async Task PickPhoto()
        {
            ShowAvatar = false;
            string? photoPath = await PlatformService.PickPhotoAsync();
            await SavePhoto(photoPath);
        }

        private async Task OnCapture()
        {
            ShowAvatar = false;
            if (!PlatformService.IsCaptureSupported())
            {
                await AlertService.Error(I18n.T("User.NoCapture"));
                return;
            }

            var cameraPermission = await PlatformService.TryCameraPermission();
            if (!cameraPermission)
            {
                await AlertService.Error(I18n.T("Permission.OpenCamera"));
                return;
            }

            var writePermission = await PlatformService.TryStorageWritePermission();
            if (!writePermission)
            {
                await AlertService.Error(I18n.T("Permission.OpenStorageWrite"));
                return;
            }

            string? photoPath = await PlatformService.CapturePhotoAsync();
            await SavePhoto(photoPath);
        }

        private async Task SavePhoto(string? sourcFilePath)
        {
            if (string.IsNullOrWhiteSpace(sourcFilePath))
            {
                return;
            }

            showLoading = true;
            StateHasChanged();

            string oldUri = await SettingsService.Get(SettingType.Avatar);

            string dir = "Avatar/";
            string uri = await AppDataService.CreateAppDataFileAsync(dir, sourcFilePath);
            await SettingsService.Save(SettingType.Avatar, uri);
            await SetAvatar(uri);

            await AppDataService.DeleteAppDataFileByCustomSchemeAsync(oldUri);

            showLoading = false;
            StateHasChanged();
            await AlertService.Success(I18n.T("Share.EditSuccess"));
            await HandleAchievements(AchievementType.Avatar);
        }

        private async Task SetAvatar(string? uri = null)
        {
            uri ??= await SettingsService.Get(SettingType.Avatar);
            Avatar = this.ImageRender(uri);
        }
    }
}
