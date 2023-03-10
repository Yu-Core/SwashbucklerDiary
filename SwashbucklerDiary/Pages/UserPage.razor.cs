using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Components;
using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Pages
{
    public partial class UserPage : PageComponentBase
    {
        private string? UserName;
        private string? Sign;
        private string? Avatar;
        private bool ShowAvatar;

        [Inject]
        private ILocalImageService LocalImageService { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            await SetAvatar();
            await LoadSettings();
            await base.OnInitializedAsync();
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
            string? photoPath = await SystemService.PickPhotoAsync();
            await SavePhoto(photoPath);
        }

        private async Task OnCapture()
        {
            ShowAvatar = false;
            if (!SystemService.IsCaptureSupported())
            {
                await AlertService.Error(I18n.T("User.NoCapture"));
                return;
            }

            var cameraPermission = await SystemService.CheckCameraPermission();
            if (!cameraPermission)
            {
                await AlertService.Error(I18n.T("Permission.OpenCamera"));
                return;
            }

            var writePermission = await SystemService.CheckStorageWritePermission();
            if (!writePermission)
            {
                await AlertService.Error(I18n.T("Permission.OpenStorageWrite"));
                return;
            }

            string? photoPath = await SystemService.CapturePhotoAsync();
            await SavePhoto(photoPath);
        }

        private async Task SavePhoto(string? filePath)
        {
            if (File.Exists(filePath))
            {
                // save the file into local storage
                string localFilePath = Path.Combine(FileSystem.AppDataDirectory, nameof(Avatar) + Path.GetExtension(filePath));

                await SystemService.FileCopy(filePath, localFilePath);

                localFilePath = await LocalImageService.AddFlag(filePath);

                await SettingsService.Save(SettingType.Avatar, localFilePath);
                var oldAvatar = Avatar;
                await SetAvatar();
                await LocalImageService.RevokeUrl(oldAvatar!);
                await AlertService.Success(I18n.T("Share.EditSuccess"));
                await HandleAchievements(AchievementType.Avatar);
            }
        }

        private async Task SetAvatar()
        {
            string avatar = await SettingsService.Get(SettingType.Avatar);
            Avatar = await LocalImageService.ToUrl(avatar);
        }
    }
}
