using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
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

        [Inject]
        private ILogger<UserPage> Logger { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            LoadView();
        }

        protected override void ReadSettings()
        {
            base.ReadSettings();

            userName = SettingService.Get(s => s.UserName, default!);
            sign = SettingService.Get(s => s.Sign, default!);
            avatar = SettingService.Get(s => s.Avatar);
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
                await SettingService.SetAsync(s => s.Sign, sign);
            }
            await HandleAchievements(Achievement.Sign);
        }

        private async Task SaveUserName(string tagName)
        {
            showEditUserName = false;
            if (!string.IsNullOrWhiteSpace(tagName) && tagName != userName)
            {
                userName = tagName;
                await SettingService.SetAsync(s => s.UserName, userName);
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
            await PopupServiceHelper.StartLoading();
            StateHasChanged();
            string? photoPath = null;
            try
            {
                photoPath = await func.Invoke();
            }
            catch (Exception e)
            {
                await PopupServiceHelper.Error(I18n.T("Share.EditFail"));
                Logger.LogError(e, I18n.T("Share.EditFail"));
            }
            finally
            {
                await PopupServiceHelper.StopLoading();
            }

            if (string.IsNullOrEmpty(photoPath))
            {
                await PopupServiceHelper.Error(I18n.T("Share.EditFail"));
                return;
            }

            avatar = photoPath;
            StateHasChanged();
            await HandleAchievements(Achievement.Avatar);
        }
    }
}
