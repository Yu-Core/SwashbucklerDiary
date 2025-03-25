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

            userName = SettingService.Get(s => s.NickName, default!);
            sign = SettingService.Get(s => s.Sign, default!);
            avatar = SettingService.Get(s => s.Avatar);
        }

        private string? NickName => userName ?? I18n.T("Swashbuckler Diary");

        private string? Sign => sign ?? I18n.T("After the incident, brush off your clothes and hide deep in your name");

        private void LoadView()
        {
            editAvatarMethods =
            [
                new(this, "Photos","mdi-image-outline",PickPhoto),
                new(this, "Capture","mdi-camera-outline",OnCapture),
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
                await SettingService.SetAsync(s => s.NickName, userName);
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
                await PopupServiceHelper.Error(I18n.T("Change failed"));
                Logger.LogError(e, I18n.T("Change failed"));
            }
            finally
            {
                await PopupServiceHelper.StopLoading();
            }

            if (string.IsNullOrEmpty(photoPath))
            {
                await PopupServiceHelper.Error(I18n.T("Change failed"));
                return;
            }

            avatar = photoPath;
            StateHasChanged();
            await HandleAchievements(Achievement.Avatar);
        }
    }
}
