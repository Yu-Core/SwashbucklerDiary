using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SwashbucklerDiary.Components;
using SwashbucklerDiary.Extend;
using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Models;
using SwashbucklerDiary.Services;

namespace SwashbucklerDiary.Pages
{
    public partial class ReadPage : PageComponentBase, IAsyncDisposable
    {
        private DiaryModel Diary = new();
        private bool ShowDelete;
        private bool ShowMenu;
        private bool ShowShare;
        private bool ShowExport;
        private bool showLoading;
        private IJSObjectReference? module;
        private bool Markdown;
        private List<ListItemModel> ListItemModels = new();
        private List<ListItemModel> ShareItems = new();
        private List<DiaryModel> ExportDiaries = new();

        [Inject]
        public IDiaryService DiaryService { get; set; } = default!;
        [Inject]
        public IconService IconService { get; set; } = default!;

        [Parameter]
        public Guid Id { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await LoadSettings();
            await LoadView();
            await UpdateDiary();
            await base.OnInitializedAsync();
        }

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                module = await JS.InvokeAsync<IJSObjectReference>("import", "./js/screenshot.js");
            }
        }

        private List<TagModel> Tags => Diary.Tags ?? new();
        private bool IsTop => Diary.Top;
        private bool IsPrivate => Diary.Private;
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

        private string TopText() => IsTop ? "Diary.CancelTop" : "Diary.Top";
        private string MarkdownText() => Markdown ? "Diary.Text" : "Diary.Markdown";
        private string MarkdownIcon() => Markdown ? "mdi-format-text" : "mdi-language-markdown-outline";
        private string PrivateText() => IsPrivate ? "Read.ClosePrivacy" : "Read.OpenPrivacy";
        private string PrivateIcon() => IsPrivate ? "mdi-lock-open-variant-outline" : "mdi-lock-outline";

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
            Markdown = await SettingsService.Get(SettingType.Markdown);
        }

        async Task LoadView()
        {
            ListItemModels = new List<ListItemModel>()
            {
                new("Share.Copy","mdi-content-copy",OnCopy),
                new(TopText,"mdi-format-vertical-align-top",OnTopping),
                new("Diary.Export","mdi-export",OpenExportDialog),
                new(MarkdownText,MarkdownIcon,MarkdownChanged),
            };
            bool privacy = await SettingsService.Get(SettingType.Privacy);
            if (privacy)
            {
                ListItemModels.Add(new(PrivateText, PrivateIcon, DiaryPrivacyChanged));
            }

            ShareItems = new()
            {
                new("Share.TextShare","mdi-format-text",ShareText),
                new("Share.ImageShare","mdi-image-outline",ShareImage),
            };
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
            ShowDelete = true;
            StateHasChanged();
        }

        private async Task HandleDelete()
        {
            ShowDelete = false;
            bool flag = await DiaryService.DeleteAsync(Diary);
            if (flag)
            {
                await AlertService.Success(I18n.T("Share.DeleteSuccess"));
            }
            else
            {
                await AlertService.Error(I18n.T("Share.DeleteFail"));
            }
            NavigateToBack();
        }

        private void OnEdit()
        {
            NavigateService.NavigateTo($"/write?DiaryId={Id}");
        }

        private async Task OnTopping()
        {
            Diary.Top = !Diary.Top;
            await DiaryService.UpdateAsync(Diary);
            StateHasChanged();
        }

        private async Task OnCopy()
        {
            await SystemService.SetClipboard(DiaryCopyContent);

            await AlertService.Success(I18n.T("Share.CopySuccess"));
        }

        private async Task ShareText()
        {
            ShowShare = false;
            StateHasChanged();
            await SystemService.ShareText(I18n.T("Share.Share")!, DiaryCopyContent);
            await HandleAchievements(AchievementType.Share);
        }

        private async Task ShareImage()
        {
            showLoading = true;
            ShowShare = false;
            StateHasChanged();
            await Task.Delay(1000);

            var base64 = await module!.InvokeAsync<string>("getScreenshotBase64", new object[1] { "#screenshot" });
            base64 = base64.Substring(base64.IndexOf(",") + 1);

            string fn = "Screenshot.png";
            string file = Path.Combine(FileSystem.CacheDirectory, fn);

            await File.WriteAllBytesAsync(file, Convert.FromBase64String(base64));
            showLoading = false;
            StateHasChanged();

            await SystemService.ShareFile(I18n.T("Share.Share")!, file);
            await HandleAchievements(AchievementType.Share);
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
            await SettingsService.Save(SettingType.Markdown, Markdown);
            StateHasChanged();
        }

        private async Task DiaryPrivacyChanged()
        {
            Diary.Private = !Diary.Private;
            await DiaryService.UpdateAsync(Diary);
        }

        private string CounterValue()
        {
            string? value = Diary.Content;
            int len = 0;
            if (string.IsNullOrWhiteSpace(value))
            {
                return len + " " + I18n.T("Write.CountUnit");
            }

            value = value.Trim();
            if (I18n.T("Write.WordCountType") == WordCountType.Word.ToString())
            {
                len = value.WordCount();
            }

            if (I18n.T("Write.WordCountType") == WordCountType.Character.ToString())
            {
                len = value.CharacterCount();
            }

            return len + " " + I18n.T("Write.CountUnit");
        }

        private void OpenExportDialog()
        {
            var diary = Diary;
            ExportDiaries = new() { diary };
            ShowExport = true;
            StateHasChanged();
        }
    }
}
