using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Components;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Extensions;
using SwashbucklerDiary.Rcl.Models;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class WritePage : ImportantComponentBase
    {
        private readonly static string cssHref = $"_content/{Rcl.Essentials.StaticWebAssets.RclAssemblyName}/css/extend/masa-blazor-extend-enqueued-snackbars-write.css";

        private bool showMenu;

        private bool showSelectTag;

        private bool showWeather;

        private bool showMood;

        private bool showLocation;

        private bool showCreateTime;

        private bool enableTitle;

        private bool enableMarkdown = true;

        private bool overlay;

        private bool autofocus = true;

        private bool privacyMode;

        private bool launchActivation;

        private bool showIconText;

        private bool showOtherInfo;

        private int editAutoSave;

        private PeriodicTimer? timer;

        private MarkdownEdit markdownEdit = default!;

        private TextareaEdit textareaEdit = default!;

        private List<DynamicListItem> menuItems = [];

        private DiaryEditMode diaryEditMode = DiaryEditMode.Add;

        private DiaryModel diary = new()
        {
            Tags = [],
            Resources = []
        };

        private List<TagModel> tags = [];

        [Inject]
        private IDiaryService DiaryService { get; set; } = default!;

        [Inject]
        private ITagService TagService { get; set; } = default!;

        [Inject]
        private IGlobalConfiguration GlobalConfiguration { get; set; } = default!;

        [Inject]
        private IResourceService ResourceService { get; set; } = default!;

        [Inject]
        private IAppLifecycle AppLifecycle { get; set; } = default!;

        [Inject]
        private IMediaResourceManager MediaResourceManager { get; set; } = default!;

        [Parameter]
        [SupplyParameterFromQuery]
        public Guid? TagId { get; set; }

        [Parameter]
        [SupplyParameterFromQuery]
        public Guid? DiaryId { get; set; }

        [Parameter]
        [SupplyParameterFromQuery]
        public DateOnly? CreateDate { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            LoadView();
            AppLifecycle.OnStopped += LeaveAppSaveDiary;
            AppLifecycle.OnResumed += ResumeApp;
            AppLifecycle.OnActivated += HandleActivated;
            NavigateController.AddHistoryAction(LeavePageSaveDiary, false);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                await Task.WhenAll(
                    InitDiary(),
                    InitTags());
                InitCreateTime();
                StateHasChanged();
                _ = CreateTimer();
            }
        }

        protected override async ValueTask DisposeAsyncCore()
        {
            await base.DisposeAsyncCore();

            AppLifecycle.OnStopped -= LeaveAppSaveDiary;
            AppLifecycle.OnResumed -= ResumeApp;
            AppLifecycle.OnActivated -= HandleActivated;
            timer?.Dispose();
        }

        protected override void ReadSettings()
        {
            base.ReadSettings();

            enableTitle = SettingService.Get(s => s.Title);
            enableMarkdown = SettingService.Get(s => s.Markdown);
            editAutoSave = SettingService.Get(s => s.EditAutoSave);
            showIconText = SettingService.Get(s => s.DiaryIconText);
            showOtherInfo = SettingService.Get(s => s.OtherInfo);
            privacyMode = SettingService.GetTemp(s => s.PrivacyMode);
        }

        private List<TagModel> SelectedTags
        {
            get => diary.Tags!;
            set => diary.Tags = value;
        }

        private StringNumber? SelectedWeather
        {
            get => diary.Weather;
            set => diary.Weather = value?.ToString();
        }

        private StringNumber? SelectedMood
        {
            get => diary.Mood;
            set => diary.Mood = value?.ToString();
        }

        private string? SelectedLocation
        {
            get => diary.Location;
            set => diary.Location = value;
        }

        private DateOnly SelectedDate
        {
            get => DateOnly.FromDateTime(diary.CreateTime);
            set => diary.CreateTime = value.ToDateTime();
        }

        private string WeatherIcon => GetWeatherIcon(diary.Weather);

        private string MoodIcon => GetMoodIcon(diary.Mood);

        private string WeatherText =>
            string.IsNullOrEmpty(diary.Weather) ? I18n.T("Write.Weather")! : I18n.T("Weather." + diary.Weather)!;

        private string MoodText =>
                   string.IsNullOrEmpty(diary.Mood) ? I18n.T("Write.Mood")! : I18n.T("Mood." + diary.Mood)!;

        private string LocationText =>
            string.IsNullOrEmpty(SelectedLocation) ? I18n.T("Write.Location") : SelectedLocation;

        private WordCountStatistics WordCountType
            => (WordCountStatistics)Enum.Parse(typeof(WordCountStatistics), I18n.T("Write.WordCountType")!);

        private string TitleSwitchText() => enableTitle ? "Write.CloseTitle" : "Write.EnableTitle";

        private string MarkdownSwitchText() => enableMarkdown ? "Diary.Text" : "Diary.Markdown";

        private string MarkdownSwitchIcon() => enableMarkdown ? "mdi-format-text" : "mdi-language-markdown-outline";

        private string OtherInfoSwitchText() => showOtherInfo ? "Write.HideOtherInfo" : "Write.DisplayOtherInfo";

        private async Task InitTags()
        {
            tags = await TagService.QueryAsync();

            if (DiaryId is not null || TagId is not Guid tagId)
            {
                return;
            }

            var tag = tags.Find(it => it.Id == tagId);
            if (tag is null)
            {
                return;
            }

            SelectedTags.Add(tag);
        }

        private async Task InitDiary()
        {
            if (DiaryId is not Guid diaryId)
            {
                return;
            }

            diaryEditMode = DiaryEditMode.Update;
            var diary = await DiaryService.FindAsync(diaryId);
            if (diary is null)
            {
                return;
            }

            this.diary = diary;
        }

        private void InitCreateTime()
        {
            if (CreateDate is null)
            {
                return;
            }

            SelectedDate = (DateOnly)CreateDate;
        }

        private void LoadView()
        {
            menuItems =
            [
                new(this, TitleSwitchText, "mdi-format-title", ()=> SettingChange(nameof(Setting.Title), ref enableTitle)),
                new(this, MarkdownSwitchText, MarkdownSwitchIcon, ()=> SettingChange(nameof(Setting.Markdown), ref enableMarkdown)),
                new(this, OtherInfoSwitchText, "mdi-information-outline", ()=> SettingChange(nameof(Setting.OtherInfo), ref showOtherInfo)),
            ];
        }

        private void RemoveSelectedTag(TagModel tag)
        {
            int index = SelectedTags.IndexOf(tag);
            if (index > -1)
            {
                SelectedTags.RemoveAt(index);
            }
        }

        private async void LeaveAppSaveDiary()
        {
            timer?.Dispose();
            await SaveDiaryAsync(true);
        }

        private async Task LeavePageSaveDiary()
        {
            NavigateController.RemoveHistoryAction(LeavePageSaveDiary);
            timer?.Dispose();
            await SaveDiaryAsync();
        }

        private void ResumeApp()
        {
            _ = CreateTimer();
        }

        private async Task SaveDiaryAsync(bool background = false)
        {
            if (enableMarkdown)
            {
                if (!background)
                {
                    overlay = true;
                    await InvokeAsync(StateHasChanged);
                }

                // https://github.com/Vanessa219/vditor/issues/1307
                // https://github.com/Vanessa219/vditor/issues/574
                await Task.Delay(120);
                diary.Content = await markdownEdit.GetValueAsync();

                if (!background)
                {
                    overlay = false;
                    await InvokeAsync(StateHasChanged);
                }
            }

            if (string.IsNullOrWhiteSpace(diary.Content))
            {
                if (diaryEditMode == DiaryEditMode.Update)
                {
                    diaryEditMode = DiaryEditMode.Add;
                    await DiaryService.DeleteAsync(diary);
                }

                return;
            }

            diary.Resources = MediaResourceManager.GetDiaryResources(diary.Content);
            diary.UpdateTime = DateTime.Now;
            if (diaryEditMode == DiaryEditMode.Add)
            {
                diaryEditMode = DiaryEditMode.Update;
                bool isSuccess = await DiaryService.AddAsync(diary);

                if (!isSuccess)
                {
                    await PopupServiceHelper.Error(I18n.T("Share.AddFail"));
                }
                else
                {
                    _ = HandleAchievements(background, DiaryEditMode.Add);
                }
            }
            else
            {
                bool isSuccess = await DiaryService.UpdateIncludesAsync(diary);
                if (!isSuccess)
                {
                    await PopupServiceHelper.Error(I18n.T("Share.EditFail"));
                }
                else
                {
                    _ = HandleAchievements(background, DiaryEditMode.Update);
                }
            }
        }

        private void HandleBreakpointChange(object? sender, BreakpointChangedEventArgs e)
        {
            StateHasChanged();
        }

        private string GetWeatherIcon(string? key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return "mdi-weather-sunny";
            }

            return GlobalConfiguration.GetWeatherIcon(key);
        }

        private string GetMoodIcon(string? key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return "mdi-emoticon-happy-outline";
            }

            return GlobalConfiguration.GetMoodIcon(key);
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
            len = WordCountType switch
            {
                WordCountStatistics.Word => value.WordCount(),
                WordCountStatistics.Character => value.CharacterCount(),
                _ => default
            };

            return len + " " + I18n.T("Write.CountUnit");
        }

        private async Task HandleAchievements(bool background, DiaryEditMode diaryEditMode)
        {
            List<string> messages = [];
            if (diaryEditMode == DiaryEditMode.Add)
            {
                var messages1 = await AchievementService.UpdateUserState(Achievement.Diary);
                messages.AddRange(messages1);
            }

            if (!background)
            {
                var alldiaries = await DiaryService.QueryAsync();
                var wordCount = alldiaries.GetWordCount(WordCountType);
                var messages2 = await AchievementService.UpdateUserState(Achievement.Word, wordCount);
                messages.AddRange(messages2);
                await AlertAchievements(messages);
            }
        }

        private Task SettingChange(string key, ref bool value)
        {
            value = !value;
            return SettingService.SetAsync(key, value);
        }

        private async Task InsertTimestamp()
        {
            string dateTimeNow = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            await InsertValueAsync(dateTimeNow);
        }

        private async Task InsertValueAsync(string value)
        {
            if (enableMarkdown)
            {
                await markdownEdit.InsertValueAsync(value);
            }
            else
            {
                await textareaEdit.InsertValueAsync(value);
            }
        }

        private async Task CreateTimer()
        {
            if (editAutoSave == 0)
            {
                return;
            }

            timer = new PeriodicTimer(TimeSpan.FromSeconds(editAutoSave));
            while (await timer.WaitForNextTickAsync())
            {
                await SaveDiaryAsync(true);
            }
        }

        private async Task HandleAppActivation()
        {
            if (launchActivation)
            {
                return;
            }

            launchActivation = true;
            var args = AppLifecycle.ActivationArguments;
            await HandleActivatedAsync(args);
            AppLifecycle.ActivationArguments = null;
        }

        private void HandleActivated(ActivationArguments? args)
        {
            _ = HandleActivatedAsync(args);
        }

        private async Task HandleActivatedAsync(ActivationArguments? args)
        {
            if (args is null || args.Data is null)
            {
                return;
            }

            if (args.Kind == AppActivationKind.Share)
            {
                await HandleShare((ShareActivationArguments)args.Data);
            }
        }

        private async Task HandleShare(ShareActivationArguments? args)
        {
            if (args is null || args.Data is null)
            {
                return;
            }

            string? insertContent = null;
            if (args.Kind == ShareActivationKind.Text)
            {
                insertContent = args.Data as string;
            }
            else if (args.Kind == ShareActivationKind.FilePaths)
            {
                var filePaths = (List<string?>)args.Data;
                insertContent = await MediaResourceManager.CreateMediaFilesInsertContentAsync(filePaths);
            }

            if (insertContent is null)
            {
                return;
            }

            await InsertValueAsync(insertContent);
        }
    }
}
