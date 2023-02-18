using Microsoft.JSInterop;
using SwashbucklerDiary.Components;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Pages
{
    public partial class UserPage : PageComponentBase, IAsyncDisposable
    {
        private IJSObjectReference? module;
        private const string DefaultAvatar = "./logo/logo.svg";
        private string? UserName;
        private string? Sign;
        private string? Avatar;
        private bool ShowAvatar;

        protected override async Task OnInitializedAsync()
        {
            await LoadSettings();
            await base.OnInitializedAsync();
        }
        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                module = await JS!.InvokeAsync<IJSObjectReference>("import", "./js/getNativeImage.js");
                bool flag = await SettingsService!.ContainsKey(nameof(Avatar));
                if (!flag)
                {
                    Avatar = DefaultAvatar;
                }
                else
                {
                    var avatar = await SettingsService!.Get(nameof(Avatar), DefaultAvatar);
                    await SetAvatar(avatar);
                }
                StateHasChanged();
            }
        }

        private bool ShowUserName { get; set; }
        private bool ShowSign { get; set; }

        private async Task LoadSettings()
        {
            UserName = await SettingsService!.Get<string?>(nameof(UserName), I18n!.T("AppName"));
            Sign = await SettingsService!.Get<string?>(nameof(Sign), I18n!.T("Mine.Sign"));
        }

        private async Task SaveSign(string tagName)
        {
            ShowSign = false;
            if (!string.IsNullOrWhiteSpace(tagName) && tagName != Sign)
            {
                Sign = tagName;
                await SettingsService!.Save(nameof(Sign), Sign);
            }
        }

        private async Task SaveUserName(string tagName)
        {
            ShowUserName = false;
            if (!string.IsNullOrWhiteSpace(tagName) && tagName != UserName)
            {
                UserName = tagName;
                await SettingsService!.Save(nameof(UserName), UserName);
            }
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
                await PopupService.ToastErrorAsync(I18n!.T("User.NoCapture"));
                return;
            }

            var cameraPermission = await SystemService.CheckCameraPermission();
            if (!cameraPermission)
            {
                await PopupService.ToastErrorAsync(I18n!.T("Permission.OpenCamera"));
                return;
            }

            var writePermission = await SystemService.CheckStorageWritePermission();
            if (!writePermission)
            {
                await PopupService.ToastErrorAsync(I18n!.T("Permission.OpenStorageWrite"));
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

                await SettingsService!.Save(nameof(Avatar), localFilePath);
                await SetAvatar(localFilePath);
                await PopupService.ToastSuccessAsync(I18n!.T("Share.EditSuccess"));
            }
        }

        private async Task SetAvatar(string path)
        {
            //Here is a provisional approach.Because https://github.com/dotnet/maui/issues/2907
            using var imageStream = File.OpenRead(path);
            var dotnetImageStream = new DotNetStreamReference(imageStream);
            Avatar = await module!.InvokeAsync<string>("streamToUrl", new object[1] { dotnetImageStream });
        }

        async ValueTask IAsyncDisposable.DisposeAsync()
        {
            if (!string.IsNullOrEmpty(Avatar) && Avatar != DefaultAvatar)
            {
                await module!.InvokeVoidAsync("revokeUrl", new object[1] { Avatar });
            }

            if (module is not null)
            {
                await module.DisposeAsync();
            }
            GC.SuppressFinalize(this);
        }
    }
}
