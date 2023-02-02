using BlazorComponent;
using BlazorComponent.I18n;
using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using NoDecentDiary.Extend;
using NoDecentDiary.IServices;
using NoDecentDiary.Models;
using NoDecentDiary.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoDecentDiary.Pages
{
    public partial class PageRead : IAsyncDisposable
    {
        [Inject]
        public MasaBlazor? MasaBlazor { get; set; }
        [Inject]
        public IDiaryService? DiaryService { get; set; }
        [Inject]
        public IPopupService? PopupService { get; set; }
        [Inject]
        public IDiaryTagService? DiaryTagService { get; set; }
        [Inject]
        public ITagService? TagService { get; set; }
        [Inject]
        public INavigateService? NavigateService { get; set; }
        [Inject]
        public IJSRuntime? JS { get; set; }
        [Inject]
        public IconService? IconService { get; set; }
        [Inject]
        private I18n? I18n { get; set; }

        [Parameter]
        public int Id { get; set; }
        private DiaryModel Diary = new DiaryModel();
        private bool _showDelete;
        private bool ShowDelete
        {
            get => _showDelete;
            set
            {
                _showDelete = value;
                if (!value)
                {
                    HandOnOKDelete = null;
                }
            }
        }
        private bool _showMenu;
        private bool ShowMenu
        {
            get => _showMenu;
            set
            {
                SetShowMenu(value);
            }
        }
        private bool _showShare;
        private bool ShowShare
        {
            get => _showShare;
            set
            {
                SetShowShare(value);
            }
        }
        private bool showLoading;
        private bool ShowTitle => !string.IsNullOrEmpty(Diary.Title);
        private bool ShowWeather => !string.IsNullOrEmpty(Diary.Weather);
        private bool ShowMood => !string.IsNullOrEmpty(Diary.Mood);
        private bool ShowLocation => !string.IsNullOrEmpty(Diary.Location);
        private bool Desktop => MasaBlazor!.Breakpoint.SmAndUp;
        private bool Mobile => !MasaBlazor!.Breakpoint.SmAndUp;
        private string DiaryCopyContent
        {
            get
            {
                if (string.IsNullOrEmpty(Diary.Title))
                {
                    return Diary.Content!;
                }
                return Diary.Title + "\n" + Diary.Content;
            }
        }
        private List<TagModel> SelectedTags = new List<TagModel>();
        private bool Top => Diary.Top;
        private IJSObjectReference? module;
        private Action? HandOnOKDelete;

        protected override async Task OnInitializedAsync()
        {
            await UpdateDiary();
            await UpdateTag();
            MasaBlazor!.Breakpoint.OnUpdate += InvokeStateHasChangedAsync;
        }
        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if(firstRender)
            {
                module = await JS!.InvokeAsync<IJSObjectReference>("import", "./js/screenshot.js");
            }
        }
        private async Task UpdateDiary()
        {
            var diaryModel = await DiaryService!.FindAsync(Id);
            if (diaryModel == null)
            {
                NavigateToBack();
                return;
            }
            Diary = diaryModel;
        }
        private async Task UpdateTag()
        {
            SelectedTags = await TagService!.GetDiaryTagsAsync(Id);
        }
        private void HandOnBack()
        {
            NavigateToBack();
        }
        public void NavigateToBack()
        {
            NavigateService!.NavigateToBack();
        }
        private async Task InvokeStateHasChangedAsync()
        {
            await InvokeAsync(StateHasChanged);
        }
        async ValueTask IAsyncDisposable.DisposeAsync()
        {
            MasaBlazor!.Breakpoint.OnUpdate -= InvokeStateHasChangedAsync;
            if (module is not null)
            {
                await module.DisposeAsync();
            }
            if(ShowMenu)
            {
                NavigateService!.Action -= CloseMenu;
            }
            if(ShowShare)
            {
                NavigateService!.Action -= CloseShare;
            }
            GC.SuppressFinalize(this);
        }
        public void HandOnDelete()
        {
            HandOnOKDelete += async () =>
            {
                ShowDelete = false;
                bool flag = await DiaryService!.DeleteAsync(Id);
                if (flag)
                {
                    await PopupService!.ToastAsync(it => 
                    { 
                        it.Type = AlertTypes.Success;
                        it.Title = I18n!.T("Share.DeleteSuccess"); 
                    });
                }
                else
                {
                    await PopupService!.ToastAsync(it => 
                    { 
                        it.Type = AlertTypes.Error; 
                        it.Title = I18n!.T("Share.DeleteFail");
                    });
                }
                NavigateToBack();
            };
            ShowDelete = true;
            StateHasChanged();
        }
        public void HandOnEdit()
        {
            NavigateService!.NavigateTo($"/Write?DiaryId={Id}");
        }
        private async Task Topping(DiaryModel diaryModel)
        {
            diaryModel.Top = !diaryModel.Top;
            await DiaryService!.UpdateAsync(diaryModel);

        }
        private async void Copy()
        {
            await Clipboard.Default.SetTextAsync(DiaryCopyContent);

            await PopupService!.ToastAsync(it =>
            {
                it.Type = AlertTypes.Success;
                it.Title = I18n!.T("Share.CopySuccess");
            });
        }
        private async Task TextShare()
        {
            ShowShare = false;
            await Share.Default.RequestAsync(new ShareTextRequest
            {
                Text = DiaryCopyContent,
                Title = I18n!.T("Read.Share")
        });
        }
        private async Task ImageShare()
        {
            ShowShare = false;
            showLoading = true;

            var base64 = await module!.InvokeAsync<string>("getScreenshotBase64", new object[1] { "#screenshot" });
            base64 = base64.Substring(base64.IndexOf(",") + 1);

            string fn = "Screenshot.png";
            string file = Path.Combine(FileSystem.CacheDirectory, fn);

            await File.WriteAllBytesAsync(file, Convert.FromBase64String(base64));
            showLoading = false;

            await Share.Default.RequestAsync(new ShareFileRequest
            {
                Title = I18n!.T("Read.Share"),
                File = new ShareFile(file)
            });
        }
        private string GetWeatherIcon(string? key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return "mdi-weather-cloudy";
            }
            return IconService!.GetWeatherIcon(key);
        }
        private string GetMoodIcon(string? key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return "mdi-emoticon-outline";
            }
            return IconService!.GetMoodIcon(key);
        }
        private void SetShowMenu(bool value)
        {
            if (_showMenu != value)
            {
                _showMenu = value;
                if (value)
                {
                    NavigateService!.Action += CloseMenu;
                }
                else
                {
                    NavigateService!.Action -= CloseMenu;
                }
            }
        }
        private void CloseMenu()
        {
            ShowMenu = false;
            StateHasChanged();
        }
        private void SetShowShare(bool value)
        {
            if (_showShare != value)
            {
                _showShare = value;
                if (value)
                {
                    NavigateService!.Action += CloseShare;
                }
                else
                {
                    NavigateService!.Action -= CloseShare;
                }
            }
        }
        private void CloseShare()
        {
            ShowShare = false;
            StateHasChanged();
        }
    }
}
