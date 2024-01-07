using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Components;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Models;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class UserPage : ImportantComponentBase
    {
        private string? userName;

        private string? sign;

        private string? avatar;

        private bool showEditAvatar;

        private bool showEditUserName;

        private bool showEditSign;

        private List<DynamicListItem> editAvatarMethods = [];

        [Inject]
        private IAvatarService AvatarService { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            LoadView();
            await SetAvatar();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                await LoadSettings();
                StateHasChanged();
            }
        }

        private void LoadView()
        {
            editAvatarMethods = new()
            {
                new(this, "User.Photos","mdi-image-outline",PickPhoto),
                new(this, "User.Capture","mdi-camera-outline",OnCapture),
            };
        }

        private async Task LoadSettings()
        {
            userName = await Preferences.Get(Setting.UserName, I18n.T("AppName"));
            sign = await Preferences.Get(Setting.Sign, I18n.T("Mine.Sign"));
        }

        private async Task SaveSign(string tagName)
        {
            showEditSign = false;
            if (!string.IsNullOrWhiteSpace(tagName) && tagName != sign)
            {
                sign = tagName;
                await Preferences.Set(Setting.Sign, sign);
            }
            await HandleAchievements(Achievement.Sign);
        }

        private async Task SaveUserName(string tagName)
        {
            showEditUserName = false;
            if (!string.IsNullOrWhiteSpace(tagName) && tagName != userName)
            {
                userName = tagName;
                await Preferences.Set(Setting.UserName, userName);
            }
            await HandleAchievements(Achievement.NickName);
        }

        private async Task PickPhoto()
        {
            showEditAvatar = false;
            string? photoPath = await PlatformIntegration.PickPhotoAsync();
            await SavePhoto(photoPath);
        }

        private async Task OnCapture()
        {
            showEditAvatar = false;

            var cameraPermission = await PlatformIntegration.TryCameraPermission();
            if (!cameraPermission)
            {
                return;
            }

            var writePermission = await PlatformIntegration.TryStorageWritePermission();
            if (!writePermission)
            {
                return;
            }

            string? photoPath = await PlatformIntegration.CapturePhotoAsync();
            await SavePhoto(photoPath);
        }

        private async Task SavePhoto(string? sourcFilePath)
        {
            if (string.IsNullOrWhiteSpace(sourcFilePath))
            {
                return;
            }

            await AlertService.StartLoading();
            await InvokeAsync(StateHasChanged);

            avatar = await AvatarService.SetAvatar(sourcFilePath);

            await AlertService.StopLoading();
            await InvokeAsync(StateHasChanged);
            await AlertService.Success(I18n.T("Share.EditSuccess"));
            await HandleAchievements(Achievement.Avatar);
        }

        private async Task SetAvatar()
        {
            avatar = await Preferences.Get<string>(Setting.Avatar);
        }
    }
}
