using BlazorComponent;
using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using NoDecentDiary.Extend;
using NoDecentDiary.IServices;
using NoDecentDiary.Models;
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

        [Parameter]
        public int Id { get; set; }
        private DiaryModel _diary = new DiaryModel();
        private bool showMenu;
        private bool showShare;
        private bool showLoading;
        private bool ShowTitle => !string.IsNullOrEmpty(_diary.Title);
        private bool ShowWeather => !string.IsNullOrEmpty(_diary.Weather);
        private bool IsDesktop => MasaBlazor!.Breakpoint.SmAndUp;
        private string DiaryContent => _diary.Title + "\n" + _diary.Content;
        private List<TagModel> SelectedTags = new List<TagModel>();
        private bool Top => _diary.Top;
        private IJSObjectReference? module;

        protected override async Task OnInitializedAsync()
        {
            await UpdateDiary();
            await UpdateTag();
            MasaBlazor!.Breakpoint.OnUpdate += InvokeStateHasChangedAsync;
        }
        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            module = await JS!.InvokeAsync<IJSObjectReference>("import", "./js/screenshot.js");
        }
        private async Task UpdateDiary()
        {
            var diaryModel = await DiaryService!.FindAsync(Id);
            if (diaryModel == null)
            {
                NavigateToBack();
                return;
            }
            _diary = diaryModel;
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

            GC.SuppressFinalize(this);
        }
        public async Task HandOnDelete()
        {
            var confirmed = await PopupService!.ConfirmAsync(param =>
            {
                param.Title = "删除日记";
                param.TitleStyle = "font-weight:700;";
                param.Content = "请慎重删除，每一篇日记都是珍贵的回忆。";
                param.IconColor = "red";
                param.ActionsStyle = "justify-content: flex-end;";
            });
            if (!confirmed)
            {
                return;
            }
            bool flag = await DiaryService!.DeleteAsync(Id);
            if (flag)
            {
                await PopupService!.AlertAsync("删除成功", AlertTypes.Success);
            }
            else
            {
                await PopupService!.AlertAsync("删除失败", AlertTypes.Error);
            }
            NavigateToBack();
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
        private async void Copy(string text)
        {
            await Clipboard.Default.SetTextAsync(text);

            await PopupService!.AlertAsync(param =>
            {
                param.Content = "复制成功";
                param.Rounded = true;
                param.Type = AlertTypes.Success;
            });
        }
        private async Task TextShare(string text)
        {
            showShare = false;
            await Share.Default.RequestAsync(new ShareTextRequest
            {
                Text = text,
                Title = "分享"
            });
        }
        private async Task ImageShare()
        {
            showShare = false;
            showLoading = true;

            var base64 = await module!.InvokeAsync<string>("getScreenshotBase64", new object[1] { "#screenshot" });
            base64 = base64.Substring(base64.IndexOf(",") + 1);

            string fn = "Screenshot.png";
            string file = Path.Combine(FileSystem.CacheDirectory, fn);

            await File.WriteAllBytesAsync(file, Convert.FromBase64String(base64));
            showLoading = false;

            await Share.Default.RequestAsync(new ShareFileRequest
            {
                Title = "分享",
                File = new ShareFile(file)
            });
        }
    }
}
