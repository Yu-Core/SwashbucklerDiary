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
    public partial class PageMine
    {
        [Inject]
        public INavigateService? NavigateService { get; set; }
        [Inject]
        private IDiaryService? DiaryService { get; set; }
        [Inject]
        private IPopupService? PopupService { get; set; }
        [Inject]
        private I18n? I18n { get; set; }

        private int DiaryCount { get; set; }
        private long WordCount { get; set; }
        private int ActiveDayCount { get; set; }
        private string SelectLanguage { get; set; } = Languages.First().Value;
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
        }

        private async Task SetCount()
        {
            DiaryCount = await DiaryService!.CountAsync();
            var diaries = await DiaryService!.QueryAsync();
            foreach (var item in diaries)
            {
                WordCount += item.Content?.Length ?? 0;
            }
            ActiveDayCount = diaries.Select(it => DateOnly.FromDateTime(it.CreateTime)).Distinct().Count();
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
        private void OnChangeLanguage(string value)
        {
            SelectLanguage = value;
            I18n!.SetCulture(new CultureInfo(value));
            ShowLanguage = false;
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
