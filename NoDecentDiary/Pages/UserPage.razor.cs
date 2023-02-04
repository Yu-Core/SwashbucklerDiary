using BlazorComponent.I18n;
using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using NoDecentDiary.Components;
using NoDecentDiary.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoDecentDiary.Pages
{
    public partial class UserPage : PageComponentBase, IAsyncDisposable
    {
        private IJSObjectReference? module;
        private const string DefaultAvatar = "./logo/logo.svg";
        private string? UserName;
        private string? Sign;
        private string? Avatar;
        private string? InputSign;
        private string? InputUserName;
        private bool _showAvatar;
        private bool _showUserName;
        private bool _showSign;

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

        private bool ShowAvatar
        {
            get => _showAvatar;
            set => SetShowAvatar(value);
        }
        
        private bool ShowUserName
        {
            get => _showUserName;
            set => SetShowUserName(value);
        }
        
        private bool ShowSign
        {
            get => _showSign;
            set => SetShowSign(value);
        }

        private async Task LoadSettings()
        {
            UserName = await SettingsService!.Get<string?>(nameof(UserName), I18n!.T("AppName"));
            Sign = await SettingsService!.Get<string?>(nameof(Sign), I18n!.T("Mine.Sign"));
        }

        private async Task SaveSign()
        {
            ShowSign = false;
            if (!string.IsNullOrWhiteSpace(InputSign) && InputSign != Sign)
            {
                Sign = InputSign;
                await SettingsService!.Save(nameof(Sign), Sign);
            }
        }

        private async Task SaveUserName()
        {
            ShowUserName = false;
            if (!string.IsNullOrWhiteSpace(InputUserName) && InputUserName != UserName)
            {
                UserName = InputUserName;
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
            if(!writePermission)
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
                string localFilePath = Path.Combine(FileSystem.Current.AppDataDirectory, nameof(Avatar) + Path.GetExtension(filePath));

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

        private void SetShowAvatar(bool value)
        {
            if (_showAvatar != value)
            {
                _showAvatar = value;
                if (value)
                {
                    NavigateService.Action += CloseAvatar;
                }
                else
                {
                    NavigateService.Action -= CloseAvatar;
                }
            }
        }

        private void CloseAvatar()
        {
            ShowAvatar = false;
            StateHasChanged();
        }

        private void SetShowUserName(bool value)
        {
            if (_showUserName != value)
            {
                if (value)
                {
                    InputUserName = UserName;
                    NavigateService.Action += CloseUserName;
                }
                else
                {
                    NavigateService.Action -= CloseUserName;
                }
                _showUserName = value;
            }
        }
        
        private void CloseUserName()
        {
            ShowUserName = false;
            StateHasChanged();
        }

        private void SetShowSign(bool value)
        {
            if (_showSign != value)
            {
                if (value)
                {
                    InputSign = Sign;
                    NavigateService.Action += CloseSign;
                }
                else
                {
                    NavigateService.Action -= CloseSign;
                }
                _showSign = value;
            }
        }

        private void CloseSign()
        {
            ShowSign = false;
            StateHasChanged();
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
            if (ShowAvatar)
            {
                NavigateService.Action -= CloseAvatar;
            }
            if (ShowUserName)
            {
                NavigateService.Action -= CloseUserName;
            }
            if (ShowSign)
            {
                NavigateService.Action -= CloseSign;
            }
            GC.SuppressFinalize(this);
        }
    }
}
