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
            await UpdateSettings();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                await UpdateSettings();
                StateHasChanged();
            }
        }

        private string? UserName => userName ?? I18n.T("AppName");

        private string? Sign => sign ?? I18n.T("Mine.Sign");

        private void LoadView()
        {
            editAvatarMethods = new()
            {
                new(this, "User.Photos","mdi-image-outline",PickPhoto),
                new(this, "User.Capture","mdi-camera-outline",OnCapture),
            };
        }

        private async Task UpdateSettings()
        {
            var userNameTask = SettingService.Get<string>(Setting.UserName, default!);
            var signTask = SettingService.Get<string>(Setting.Sign, default!);
            var avatarTask = SettingService.Get<string>(Setting.Avatar);

            await Task.WhenAll(
                userNameTask,
                signTask,
                avatarTask);

            userName = userNameTask.Result;
            sign = signTask.Result;
            avatar = avatarTask.Result;
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
