using BlazorComponent;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SwashbucklerDiary.Components;
using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Models;
using SwashbucklerDiary.Services;

namespace SwashbucklerDiary.Pages
{
    public partial class ReadPage : PageComponentBase, IAsyncDisposable
    {
        private DiaryModel Diary = new();
        private bool _showDelete;
        private bool ShowMenu;
        private bool ShowShare;
        private bool showLoading;
        private IJSObjectReference? module;
        private Action? OnDelete;
        private bool Markdown;

        [Inject]
        public IDiaryService DiaryService { get; set; } = default!;
        [Inject]
        public IconService IconService { get; set; } = default!;

        [Parameter]
        public int Id { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await LoadSettings();
            await UpdateDiary();
        }

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                module = await JS.InvokeAsync<IJSObjectReference>("import", "./js/screenshot.js");
            }
        }

        private List<TagModel> Tags => Diary.Tags ?? new();
        private bool ShowDelete
        {
            get => _showDelete;
            set
            {
                _showDelete = value;
                if (!value)
                {
                    OnDelete = null;
                }
            }
        }
        private bool IsTop => Diary.Top;
        private bool ShowTitle => !string.IsNullOrEmpty(Diary.Title);
        private bool ShowWeather => !string.IsNullOrEmpty(Diary.Weather);
        private bool ShowMood => !string.IsNullOrEmpty(Diary.Mood);
        private bool ShowLocation => !string.IsNullOrEmpty(Diary.Location);
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

        private async Task UpdateDiary()
        {
            var diary = await DiaryService.FindIncludesAsync(Id);
            if (diary == null)
            {
                NavigateToBack();
                return;
            }
            Diary = diary;
        }

        private async Task LoadSettings()
        {
            Markdown = await SettingsService.Get(nameof(Markdown), false);
        }

        async ValueTask IAsyncDisposable.DisposeAsync()
        {
            if (module is not null)
            {
                await module.DisposeAsync();
            }
            GC.SuppressFinalize(this);
        }

        private void OpenDeleteDialog()
        {
            OnDelete += async () =>
            {
                ShowDelete = false;
                bool flag = await DiaryService.DeleteAsync(Diary);
                if (flag)
                {
                    await PopupService.ToastAsync(it =>
                    {
                        it.Type = AlertTypes.Success;
                        it.Title = I18n.T("Share.DeleteSuccess");
                    });
                }
                else
                {
                    await PopupService.ToastAsync(it =>
                    {
                        it.Type = AlertTypes.Error;
                        it.Title = I18n.T("Share.DeleteFail");
                    });
                }
                NavigateToBack();
            };
            ShowDelete = true;
            StateHasChanged();
        }

        private void OnEdit()
        {
            NavigateService.NavigateTo($"/write?DiaryId={Id}");
        }

        private async Task OnTopping(DiaryModel diaryModel)
        {
            diaryModel.Top = !diaryModel.Top;
            await DiaryService.UpdateAsync(diaryModel);
        }

        private async Task OnCopy()
        {
            await SystemService.SetClipboard(DiaryCopyContent);

            await PopupService.ToastAsync(it =>
            {
                it.Type = AlertTypes.Success;
                it.Title = I18n.T("Share.CopySuccess");
            });
        }

        private async Task ShareText()
        {
            ShowShare = false;
            await SystemService.ShareText(I18n.T("Read.Share"), DiaryCopyContent);
        }

        private async Task ShareImage()
        {
            ShowShare = false;
            showLoading = true;

            var base64 = await module!.InvokeAsync<string>("getScreenshotBase64", new object[1] { "#screenshot" });
            base64 = base64.Substring(base64.IndexOf(",") + 1);

            string fn = "Screenshot.png";
            string file = Path.Combine(FileSystem.CacheDirectory, fn);

            await File.WriteAllBytesAsync(file, Convert.FromBase64String(base64));
            showLoading = false;

            await SystemService.ShareFile(I18n.T("Read.Share"), file);
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

        private async Task MarkdownChanged()
        {
            Markdown = !Markdown;
            await SettingsService!.Save(nameof(Markdown), Markdown);
        }
    }
}
