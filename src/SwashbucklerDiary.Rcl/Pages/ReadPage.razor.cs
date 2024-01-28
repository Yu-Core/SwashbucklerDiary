using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Components;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Models;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class ReadPage : ImportantComponentBase
    {
        private bool showDelete;

        private bool showMenu;

        private bool showShare;

        private bool showExport;

        private bool enableMarkdown;

        private bool enablePrivacy;

        private DiaryModel diary = new();

        private List<DynamicListItem> menuItems = [];

        private List<DynamicListItem> shareItems = [];

        private List<DiaryModel> exportDiaries = [];

        [Inject]
        private IDiaryService DiaryService { get; set; } = default!;

        [Inject]
        private IIconService IconService { get; set; } = default!;

        [Inject]
        private IScreenshot ScreenshotService { get; set; } = default!;

        [Parameter]
        [SupplyParameterFromQuery]
        public Guid Id { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            LoadView();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if(firstRender)
            {
                await UpdateData();
                StateHasChanged();
            }
        }

        private List<TagModel> Tags => diary.Tags ?? new();

        private bool IsTop => diary.Top;

        private bool IsPrivate => diary.Private;

        private bool ShowTitle => !string.IsNullOrEmpty(diary.Title);

        private bool ShowWeather => !string.IsNullOrEmpty(diary.Weather);

        private bool ShowMood => !string.IsNullOrEmpty(diary.Mood);

        private bool ShowLocation => !string.IsNullOrEmpty(diary.Location);

        private string TopText() => IsTop ? "Diary.CancelTop" : "Diary.Top";

        private string MarkdownText() => enableMarkdown ? "Diary.Text" : "Diary.Markdown";

        private string MarkdownIcon() => enableMarkdown ? "mdi-format-text" : "mdi-language-markdown-outline";

        private string PrivateText() => IsPrivate ? "Read.ClosePrivacy" : "Read.OpenPrivacy";

        private string PrivateIcon() => IsPrivate ? "mdi-lock-open-variant-outline" : "mdi-lock-outline";

        private async Task UpdateDiary()
        {
            var diary = await DiaryService.FindAsync(Id);
            if (diary == null)
            {
                await NavigateToBack();
                return;
            }

            this.diary = diary;
        }

        private async Task UpdateSettings()
        {
            var markdownTask = Preferences.Get<bool>(Setting.Markdown);
            var privacyTask = Preferences.Get<bool>(Setting.PrivacyMode);
            await Task.WhenAll(markdownTask, privacyTask);
            enableMarkdown = markdownTask.Result;
            enablePrivacy = privacyTask.Result;
        }

        private void LoadView()
        {
            menuItems = new List<DynamicListItem>()
            {
                new(this, "Share.Copy","mdi-content-copy", OnCopy),
                new(this, TopText,"mdi-format-vertical-align-top", OnTopping),
                new(this, "Diary.Export","mdi-export", OpenExportDialog),
                new(this, MarkdownText,MarkdownIcon, MarkdownChanged),
                new(this, PrivateText, PrivateIcon, DiaryPrivacyChanged,()=>enablePrivacy)
            };

            shareItems = new()
            {
                new(this, "Share.TextShare","mdi-format-text", ShareText),
                new(this, "Share.ImageShare","mdi-image-outline", ShareImage),
            };
        }

        private void OpenDeleteDialog()
        {
            showDelete = true;
            StateHasChanged();
        }

        private async Task HandleDelete()
        {
            showDelete = false;
            bool flag = await DiaryService.DeleteAsync(diary);
            if (flag)
            {
                await AlertService.Success(I18n.T("Share.DeleteSuccess"));
                await NavigateToBack();
            }
            else
            {
                await AlertService.Error(I18n.T("Share.DeleteFail"));
            }
        }

        private Task OnEdit()
        {
            return NavigateService.PushAsync($"/write?DiaryId={Id}", false);
        }

        private async Task OnTopping()
        {
            diary.Top = !diary.Top;
            await DiaryService.UpdateAsync(diary);
            StateHasChanged();
        }

        private async Task OnCopy()
        {
            var content = GetDiaryCopyContent();
            await PlatformIntegration.SetClipboard(content);
            await AlertService.Success(I18n.T("Share.CopySuccess"));
        }

        private async Task ShareText()
        {
            var content = GetDiaryCopyContent();
            await PlatformIntegration.ShareTextAsync(I18n.T("Share.Share"), content);
            await HandleAchievements(Achievement.Share);
        }

        private async void ShareImage()
        {
            await AlertService.StartLoading();

            var filePath = await ScreenshotService.CaptureAsync("#screenshot");
            await PlatformIntegration.ShareFileAsync(I18n.T("Share.Share"), filePath);

            await AlertService.StopLoading();
            await InvokeAsync(StateHasChanged);

            await HandleAchievements(Achievement.Share);
        }

        private string GetWeatherIcon()
        {
            if (string.IsNullOrWhiteSpace(diary.Weather))
            {
                return "mdi-weather-cloudy";
            }

            return IconService.GetWeatherIcon(diary.Weather);
        }

        private string GetMoodIcon()
        {
            if (string.IsNullOrWhiteSpace(diary.Mood))
            {
                return "mdi-emoticon-outline";
            }

            return IconService.GetMoodIcon(diary.Mood);
        }

        private async Task MarkdownChanged()
        {
            enableMarkdown = !enableMarkdown;
            await Preferences.Set(Setting.Markdown, enableMarkdown);
            StateHasChanged();
        }

        private async Task DiaryPrivacyChanged()
        {

            diary.Private = !diary.Private;
            diary.UpdateTime = DateTime.Now;
            await DiaryService.UpdateAsync(diary);
            await AlertService.Success(I18n.T("Read.PrivacyAlert"));
        }

        private string CounterValue()
        {
            string? value = diary.Content;
            int len = 0;
            if (string.IsNullOrWhiteSpace(value))
            {
                return len + " " + I18n.T("Write.CountUnit");
            }

            value = value.Trim();
            if (I18n.T("Write.WordCountType") == WordCountStatistics.Word.ToString())
            {
                len = value.WordCount();
            }

            if (I18n.T("Write.WordCountType") == WordCountStatistics.Character.ToString())
            {
                len = value.CharacterCount();
            }

            return len + " " + I18n.T("Write.CountUnit");
        }

        private async Task OpenExportDialog()
        {
            var diary = await DiaryService.FindAsync(this.diary.Id); ;
            exportDiaries = [diary];
            showExport = true;
            StateHasChanged();
        }

        private string GetDiaryCopyContent()
        {
            var content = diary.Content!;
            if (string.IsNullOrEmpty(diary.Title))
            {
                return content;
            }

            return diary.Title + "\n" + content;
        }

        private Task UpdateData()
        {
            return Task.WhenAll(
                UpdateSettings(),
                UpdateDiary());
        }
    }
}
