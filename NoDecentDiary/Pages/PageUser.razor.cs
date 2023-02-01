using BlazorComponent.I18n;
using Microsoft.AspNetCore.Components;
using NoDecentDiary.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoDecentDiary.Pages
{
    public partial class PageUser : IDisposable
    {
        [Inject]
        public INavigateService? NavigateService { get; set; }
        [Inject]
        private I18n? I18n { get; set; }
        [Inject]
        private ISettingsService? SettingsService { get; set; }
        private string? UserName { get; set; }
        private string? Sign { get; set; }
        private string? Avatar { get; set; }
        private string? InputSign { get; set; }
        private string? InputUserName { get; set; }
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
        private async Task LoadSettings()
        {
            UserName = await SettingsService!.Get(nameof(UserName), I18n!.T("AppName"));
            Sign = await SettingsService!.Get(nameof(Sign), I18n!.T("Mine.Sign"));
            Avatar = await SettingsService!.Get(nameof(Avatar), "./logo/logo.svg");
        }
        private void HandOnBack()
        {
            NavigateService!.NavigateToBack();
        }
        private async Task OnSaveSign() 
        {
            ShowSign = false;
            if(!string.IsNullOrWhiteSpace(InputSign) && InputSign != Sign)
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
        public void Dispose()
        {
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
