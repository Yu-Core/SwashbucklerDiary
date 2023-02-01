using BlazorComponent;
using BlazorComponent.I18n;
using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using NoDecentDiary.IServices;
using NoDecentDiary.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoDecentDiary.Pages
{
    public partial class PageMine : IDisposable
    {
        [Inject]
        public INavigateService? NavigateService { get; set; }
        [Inject]
        private IDiaryService? DiaryService { get; set; }
        [Inject]
        private IPopupService? PopupService { get; set; }
        [Inject]
        private I18n? I18n { get; set; }
        [Inject]
        private ISettingsService? SettingsService { get; set; }

        private int DiaryCount { get; set; }
        private long WordCount { get; set; }
        private int ActiveDayCount { get; set; }
        private string? Language { get; set; } 
        private bool _showLanguage;
        private bool ShowLanguage
        {
            get => _showLanguage;
            set => SetShowLanguage(value);
        }
        private readonly static Dictionary<string, string> Languages = new()
        {
            {"中文","zh-CN" },
            {"English","en-US" }
        };

        protected override async Task OnInitializedAsync()
        {
            await SetCount();
            await LoadSettings();
        }

        private async Task SetCount()
        {
            DiaryCount = await DiaryService!.CountAsync();
            var diaries = await DiaryService!.QueryAsync();
            if (I18n!.T("Write.Word") == "1")
            {
                foreach (var item in diaries)
                {
                    WordCount += item.Content?.Split(' ').Length ?? 0;
                }
            }

            if (I18n!.T("Write.Character") == "1")
            {
                foreach (var item in diaries)
                {
                    WordCount += item.Content?.Length ?? 0;
                }
            }
            
            ActiveDayCount = diaries.Select(it => DateOnly.FromDateTime(it.CreateTime)).Distinct().Count();
        }
        private async Task LoadSettings()
        {
            Language = await SettingsService!.Get(nameof(Language), Languages.First().Value);
        }
        private Task ToDo()
        {
            return PopupService!.ToastAsync(it =>
            {
                it.Type = AlertTypes.Info;
                it.Title = I18n!.T("ToDo.Title");
                it.Content = I18n!.T("ToDo.Content");
            });
        }
        private void NavigateToSearch()
        {
            NavigateService!.NavigateTo("/Search");
        }
        private async Task OnChangeLanguage(string value)
        {
            ShowLanguage = false;
            Language = value;
            I18n!.SetCulture(new CultureInfo(value));
            await SettingsService!.Save(nameof(Language), Language);
        }
        private void SetShowLanguage(bool value)
        {
            if (_showLanguage != value)
            {
                _showLanguage = value;
                if (value)
                {
                    NavigateService!.Action += CloseLanguage;
                }
                else
                {
                    NavigateService!.Action -= CloseLanguage;
                }
            }
        }
        private void CloseLanguage()
        {
            ShowLanguage = false;
            StateHasChanged();
        }
        public void Dispose()
        {
            if (ShowLanguage)
            {
                NavigateService!.Action -= CloseLanguage;
            }
            GC.SuppressFinalize(this);
        }
    }
}
