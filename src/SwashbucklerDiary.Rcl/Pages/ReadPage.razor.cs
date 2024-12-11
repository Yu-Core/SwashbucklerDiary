using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Components;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Extensions;
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

        private bool showSetPrivacy;

        private bool showHighlightSearch;

        private bool privacyMode;

        private bool firstLineIndent;

        private bool taskListLineThrough;

        private bool outline;

        private bool rightOutline;

        private bool afterFirstQuery;

        private bool highlightSearchAutofocus = true;

        private readonly string scrollContainerId = $"scroll-container-{Guid.NewGuid():N}";

        private string? search;

        private string scrollContainerSelector = string.Empty;

        private DiaryModel diary = new();

        private MarkdownPreview markdownPreview = default!;

        private List<DynamicListItem> menuItems = [];

        private List<DynamicListItem> shareItems = [];

        private List<DiaryModel> exportDiaries = [];

        private readonly string highlightSearchContainerClass = $"search-{Guid.NewGuid():N}";

        [Inject]
        private IDiaryService DiaryService { get; set; } = default!;

        [Inject]
        private IGlobalConfiguration GlobalConfiguration { get; set; } = default!;

        [Inject]
        private IScreenshot ScreenshotService { get; set; } = default!;

        [Parameter]
        public Guid Id { get; set; }

        [SupplyParameterFromQuery]
        public string? Query { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            scrollContainerSelector = $"#{scrollContainerId}";
            LoadView();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                await UpdateDiary();
                StateHasChanged();
                if (!enableMarkdown)
                {
                    await HandleFirstQuery();
                }
            }
        }

        protected override void ReadSettings()
        {
            base.ReadSettings();

            enableMarkdown = SettingService.Get(s => s.Markdown);
            showSetPrivacy = SettingService.Get(s => s.SetPrivacyDiary);
            firstLineIndent = SettingService.Get(s => s.FirstLineIndent);
            taskListLineThrough = SettingService.Get(s => s.TaskListLineThrough);
            outline = SettingService.Get(s => s.Outline);
            rightOutline = SettingService.Get(s => s.RigthOutline);
            privacyMode = SettingService.GetTemp(s => s.PrivacyMode);
        }

        private List<TagModel> Tags => diary.Tags ?? [];

        private bool IsTop => diary.Top;

        private bool IsPrivate => privacyMode;

        private bool ShowTitle => !string.IsNullOrEmpty(diary.Title);

        private bool ShowWeather => !string.IsNullOrEmpty(diary.Weather);

        private bool ShowMood => !string.IsNullOrEmpty(diary.Mood);

        private bool ShowLocation => !string.IsNullOrEmpty(diary.Location);

        private bool ShowAppBar => !showHighlightSearch;

        private string? WeatherIcon => GlobalConfiguration.GetWeatherIcon(diary.Weather!);

        private string? MoodIcon => GlobalConfiguration.GetMoodIcon(diary.Mood!);

        private string? WeatherText => I18n.T("Weather." + diary.Weather);

        private string? MoodText => I18n.T("Mood." + diary.Mood);

        private string? LocationText => diary.Location;

        private string TopText() => IsTop ? "Diary.CancelTop" : "Diary.Top";

        private string MarkdownText() => enableMarkdown ? "Diary.Text" : "Diary.Markdown";

        private string MarkdownIcon() => enableMarkdown ? "mdi-format-text" : "mdi-language-markdown-outline";

        private string PrivateText() => IsPrivate ? "Read.ClosePrivacy" : "Read.OpenPrivacy";

        private string PrivateIcon() => IsPrivate ? "mdi-lock-open-variant-outline" : "mdi-lock-outline";

        private async Task UpdateDiary()
        {
            var diary = await DiaryService.FindAsync(Id);
            if (diary is null)
            {
                await NavigateToBack();
                return;
            }

            this.diary = diary;
        }

        private void LoadView()
        {
            menuItems =
            [
                new(this, "Share.Copy","mdi-content-copy", OnCopy),
                new(this, TopText,"mdi-format-vertical-align-top", OnTopping),
                new(this, "Diary.Export","mdi-export", OpenExportDialog),
                new(this, MarkdownText,MarkdownIcon, MarkdownChanged),
                new(this, "Read.CopyQuote", "mdi-format-quote-open-outline", CopyQuote),
                new(this, "Read.CopyLink", "mdi-link-variant", CopyLink),
                new(this, "Read.Search", "mdi-text-box-search-outline", OpenSearch),
                new(this, PrivateText, PrivateIcon, DiaryPrivacyChanged,()=>privacyMode || showSetPrivacy)
            ];

            shareItems =
            [
                new(this, "Share.TextShare","mdi-format-text", ShareText),
                new(this, "Share.ImageShare","mdi-image-outline", ShareImage),
            ];
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
                await PopupServiceHelper.Success(I18n.T("Share.DeleteSuccess"));
                await NavigateToBack();
            }
            else
            {
                await PopupServiceHelper.Error(I18n.T("Share.DeleteFail"));
            }
        }

        private void ToWrite()
        {
            To($"write?DiaryId={Id}", false);
        }

        private async Task OnTopping()
        {
            diary.Top = !diary.Top;
            diary.UpdateTime = DateTime.Now;
            await DiaryService.UpdateAsync(diary, it => new { it.Top, it.UpdateTime });
            StateHasChanged();
        }

        private async Task OnCopy()
        {
            var content = diary.CreateCopyContent();
            await PlatformIntegration.SetClipboard(content);
            await PopupServiceHelper.Success(I18n.T("Share.CopySuccess"));
        }

        private async Task ShareText()
        {
            var content = diary.CreateCopyContent();
            await PlatformIntegration.ShareTextAsync(I18n.T("Share.Share"), content);
            await HandleAchievements(Achievement.Share);
        }

        private async void ShareImage()
        {
            await PopupServiceHelper.StartLoading();

            if (enableMarkdown && markdownPreview is not null)
            {
                await markdownPreview.RenderLazyLoadingImage();
            }

            var filePath = await ScreenshotService.CaptureAsync("#screenshot");
            await PlatformIntegration.ShareFileAsync(I18n.T("Share.Share"), filePath);

            await PopupServiceHelper.StopLoading();
            await InvokeAsync(StateHasChanged);

            await HandleAchievements(Achievement.Share);
        }

        private async Task MarkdownChanged()
        {
            enableMarkdown = !enableMarkdown;
            await SettingService.SetAsync(s => s.Markdown, enableMarkdown);
            StateHasChanged();
        }

        private async Task DiaryPrivacyChanged()
        {
            await DiaryService.MovePrivacyDiaryAsync(diary, !privacyMode);
            if (privacyMode)
            {
                await PopupServiceHelper.Success(I18n.T("Read.Removed from privacy mode"));
            }
            else
            {
                await PopupServiceHelper.Success(I18n.T("Read.Moved to privacy mode"));
            }
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

        private async Task CopyQuote()
        {
            var text = $"[{I18n.T("Read.DiaryLink")}](read/{Id})";
            await PlatformIntegration.SetClipboard(text);
            await PopupServiceHelper.Success(I18n.T("Share.CopySuccess"));
        }

        private async Task CopyLink()
        {
            string text;
            if (PlatformIntegration.CurrentPlatform == AppDevicePlatform.Browser)
            {
                text = NavigationManager.ToAbsoluteUri($"read/{Id}").ToString();
            }
            else
            {
                text = $"{SchemeConstants.SwashbucklerDiary}://read/{Id}";
            }

            await PlatformIntegration.SetClipboard(text);
            await PopupServiceHelper.Success(I18n.T("Share.CopySuccess"));
        }

        private async Task HandleFirstQuery()
        {
            if (afterFirstQuery) return;
            afterFirstQuery = true;
            if (string.IsNullOrWhiteSpace(Query)) return;

            await Task.Delay(500);
            search = Query;
            highlightSearchAutofocus = false;
            showHighlightSearch = true;
            StateHasChanged();
        }

        private void OpenSearch()
        {
            highlightSearchAutofocus = true;
            showHighlightSearch = true;
        }
    }
}
