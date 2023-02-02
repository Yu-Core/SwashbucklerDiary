using BlazorComponent.I18n;
using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using NoDecentDiary.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoDecentDiary.Pages
{
    public partial class PageUser : IAsyncDisposable
    {
        [Inject]
        public INavigateService? NavigateService { get; set; }
        [Inject]
        private I18n? I18n { get; set; }
        [Inject]
        private ISettingsService? SettingsService { get; set; }
        [Inject]
        public IJSRuntime? JS { get; set; }
        [Inject]
        private IPopupService? PopupService { get; set; }

        private IJSObjectReference? module;
        private const string DefaultAvatar = "./logo/logo.svg";
        private string? UserName { get; set; }
        private string? Sign { get; set; }
        private string? Avatar { get; set; }
        private string? InputSign { get; set; }
        private string? InputUserName { get; set; }
        private bool _showAvatar;
        private bool ShowAvatar
        {
            get => _showAvatar;
            set => SetShowAvatar(value);
        }
        private bool _showUserName;
        private bool ShowUserName
        {
            get => _showUserName;
            set => SetShowUserName(value);
        }
        private bool _showSign;
        private bool ShowSign
        {
            get => _showSign;
            set => SetShowSign(value);
        }

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
        private async Task LoadSettings()
        {
            UserName = await SettingsService!.Get(nameof(UserName), I18n!.T("AppName"));
            Sign = await SettingsService!.Get(nameof(Sign), I18n!.T("Mine.Sign"));

        }
        private void HandOnBack()
        {
            NavigateService!.NavigateToBack();
        }
        private async Task OnSaveSign()
        {
            ShowSign = false;
            if (!string.IsNullOrWhiteSpace(InputSign) && InputSign != Sign)
            {
                Sign = InputSign;
                await SettingsService!.Save(nameof(Sign), Sign);
            }
        }
        private async Task OnSaveUserName()
        {
            ShowUserName = false;
            if (!string.IsNullOrWhiteSpace(InputUserName) && InputUserName != UserName)
            {
                UserName = InputUserName;
                await SettingsService!.Save(nameof(UserName), UserName);
            }
        }
        private async Task OnPickPhoto()
        {
            ShowAvatar = false;
            FileResult photo = await MediaPicker.Default.PickPhotoAsync();
            await SavePhoto(photo);
        }
        private async Task OnCapture()
        {
            ShowAvatar = false;
            if (MediaPicker.Default.IsCaptureSupported)
            {
                FileResult photo = await MediaPicker.Default.CapturePhotoAsync();
                await SavePhoto(photo);
            }
            else
            {
                await PopupService!.ToastErrorAsync(I18n!.T("User.NoCapture"));
            }
        }
        private async Task SavePhoto(FileResult photo)
        {
            if (photo != null)
            {
                // save the file into local storage
                string localFilePath = Path.Combine(FileSystem.Current.AppDataDirectory, nameof(Avatar) + Path.GetExtension(photo.FullPath));

                using (Stream sourceStream = await photo.OpenReadAsync())
                {
                    using(FileStream localFileStream = File.OpenWrite(localFilePath))
                    {
                        await sourceStream.CopyToAsync(localFileStream);
                    };
                };

                await SettingsService!.Save(nameof(Avatar), localFilePath);
                await SetAvatar(localFilePath);
                await PopupService!.ToastSuccessAsync(I18n!.T("Share.EditSuccess"));
            }
        }
        private async Task SetAvatar(string path)
        {
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
                    NavigateService!.Action += CloseAvatar;
                }
                else
                {
                    NavigateService!.Action -= CloseAvatar;
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
                    NavigateService!.Action += CloseUserName;
                }
                else
                {
                    NavigateService!.Action -= CloseUserName;
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
                    NavigateService!.Action += CloseSign;
                }
                else
                {
                    NavigateService!.Action -= CloseSign;
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
                NavigateService!.Action -= CloseAvatar;
            }
            if (ShowUserName)
            {
                NavigateService!.Action -= CloseUserName;
            }
            if (ShowSign)
            {
                NavigateService!.Action -= CloseSign;
            }
            GC.SuppressFinalize(this);
        }
    }
}
