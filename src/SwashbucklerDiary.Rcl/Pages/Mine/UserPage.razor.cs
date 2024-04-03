using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Components;
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
        }

        protected override void UpdateSettings()
        {
            base.UpdateSettings();

            userName = SettingService.Get<string>(Setting.UserName, default!);
            sign = SettingService.Get<string>(Setting.Sign, default!);
            avatar = SettingService.Get<string>(Setting.Avatar);
        }

        private string? UserName => userName ?? I18n.T("AppName");

        private string? Sign => sign ?? I18n.T("Mine.Sign");

        private void LoadView()
        {
            editAvatarMethods =
            [
                new(this, "User.Photos","mdi-image-outline",PickPhoto),
                new(this, "User.Capture","mdi-camera-outline",OnCapture),
            ];
        }

        private async Task SaveSign(string tagName)
        {
            showEditSign = false;
            if (!string.IsNullOrWhiteSpace(tagName) && tagName != sign)
            {
                sign = tagName;
                await SettingService.Set(Setting.Sign, sign);
            }
            await HandleAchievements(Achievement.Sign);
        }

        private async Task SaveUserName(string tagName)
        {
            showEditUserName = false;
            if (!string.IsNullOrWhiteSpace(tagName) && tagName != userName)
            {
                userName = tagName;
                await SettingService.Set(Setting.UserName, userName);
            }
            await HandleAchievements(Achievement.NickName);
        }

        private Task PickPhoto()
            => SetAvatar(AvatarService.SetAvatarByPickPhoto);

        private Task OnCapture()
            => SetAvatar(AvatarService.SetAvatarByCapture);

        private async Task SetAvatar(Func<Task<string>> func)
        {
            showEditAvatar = false;
            await AlertService.StartLoading();
            StateHasChanged();
            string? photoPath = await func.Invoke();
            await AlertService.StopLoading();
            if (string.IsNullOrEmpty(photoPath))
            {
                return;
            }

            avatar = photoPath;
            StateHasChanged();
            await AlertService.Success(I18n.T("Share.EditSuccess"));
            await HandleAchievements(Achievement.Avatar);
        }
    }
}
