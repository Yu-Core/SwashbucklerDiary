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

        private bool createMode;

        private bool overlay;

        private bool autofocus = true;

        private bool privacyMode;

        private bool launchActivation;

        private int editAutoSave;

        private PeriodicTimer? timer;

        private MarkdownEdit markdownEdit = default!;

        private TextareaEdit textareaEdit = default!;

        private List<DynamicListItem> menuItems = [];

        private DiaryModel diary = new()
        {
            Tags = [],
            Resources = []
        };

        private List<TagModel> tags = [];

        private Dictionary<string, string> weatherIcons = [];

        private Dictionary<string, string> moodIcons = [];

        [Inject]
        private IDiaryService DiaryService { get; set; } = default!;

        [Inject]
        private ITagService TagService { get; set; } = default!;

        [Inject]
        private IIconService IconService { get; set; } = default!;

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
            AppLifecycle.Stopped += LeaveAppSaveDiary;
            AppLifecycle.Resumed += ResumeApp;
            AppLifecycle.Activated += Activated;
            NavigateController.AddHistoryAction(LeaveThisPageSaveDiary, false);
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

        protected override void OnDispose()
        {
            AppLifecycle.Stopped -= LeaveAppSaveDiary;
            AppLifecycle.Resumed -= ResumeApp;
            AppLifecycle.Activated -= Activated;
            timer?.Dispose();
            base.OnDispose();
        }

        protected override void ReadSettings()
        {
            base.ReadSettings();

            enableTitle = SettingService.Get<bool>(Setting.Title);
            enableMarkdown = SettingService.Get<bool>(Setting.Markdown);
            editAutoSave = SettingService.Get<int>(Setting.EditAutoSave);
            privacyMode = SettingService.GetTemp<bool>(TempSetting.PrivacyMode);
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

        private string WeatherText =>
            string.IsNullOrEmpty(diary.Weather) ? I18n.T("Write.Weather")! : I18n.T("Weather." + diary.Weather)!;

        private string MoodIcon => GetMoodIcon(diary.Mood);

        private string MoodText =>
            string.IsNullOrEmpty(diary.Mood) ? I18n.T("Write.Mood")! : I18n.T("Mood." + diary.Mood)!;

        private string LocationText =>
            string.IsNullOrEmpty(SelectedLocation) ? I18n.T("Write.Location") : SelectedLocation;

        private WordCountStatistics WordCountType
            => (WordCountStatistics)Enum.Parse(typeof(WordCountStatistics), I18n.T("Write.WordCountType")!);

        private string TitleSwitchText() => enableTitle ? "Write.CloseTitle" : "Write.EnableTitle";

        private string MarkdownSwitchText() => enableMarkdown ? "Diary.Text" : "Diary.Markdown";

        private string MarkdownSwitchIcon() => enableMarkdown ? "mdi-format-text" : "mdi-language-markdown-outline";

        private async Task InitTags()
        {
            tags = await TagService.QueryAsync();

            if (DiaryId is null && TagId is not null)
            {
                var tag = tags.Find(it => it.Id == TagId);
                if (tag is not null)
                {
                    SelectedTags.Add(tag);
                }
            }
        }

        private async Task InitDiary()
        {
            if (DiaryId is null)
            {
                createMode = true;
                return;
            }

            var diary = await DiaryService.FindAsync((Guid)DiaryId);
            if (diary is null)
            {
                return;
            }

            this.diary = diary;
            enableTitle = !string.IsNullOrEmpty(diary.Title);
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
                new(this, TitleSwitchText, "mdi-format-title", ()=> SettingChange(Setting.Title, ref enableTitle)),
                new(this, MarkdownSwitchText, MarkdownSwitchIcon, ()=> SettingChange(Setting.Markdown, ref enableMarkdown)),
            ];
            weatherIcons = IconService.GetWeatherIcons();
            moodIcons = IconService.GetMoodIcons();
        }

        private void RemoveSelectedTag(TagModel tag)
        {
            int index = SelectedTags.IndexOf(tag);
            if (index > -1)
            {
                SelectedTags.RemoveAt(index);
            }
        }

        private void SaveSelectTags()
        {
            showSelectTag = false;
        }

        private async void LeaveAppSaveDiary()
        {
            timer?.Dispose();
            await SaveDiaryAsync(true);
        }

        private async Task LeaveThisPageSaveDiary()
        {
            NavigateController.RemoveHistoryAction(LeaveThisPageSaveDiary);
            timer?.Dispose();
            await SaveDiaryAsync(true);
        }

        private void ResumeApp()
        {
            _ = CreateTimer();
        }

        private Task SaveDiaryAsync() => SaveDiaryAsync(false);

        private async Task SaveDiaryAsync(bool background)
        {
            if (enableMarkdown)
            {
                // vditor 每次输入会触发渲染，所以有1秒左右的防抖
                // https://github.com/Vanessa219/vditor/issues/1307
                // https://github.com/Vanessa219/vditor/issues/574
                if (!background)
                {
                    overlay = true;
                    await InvokeAsync(StateHasChanged);
                }

                await Task.Delay(800);

                if (!background)
                {
                    overlay = false;
                    await InvokeAsync(StateHasChanged);
                }
            }

            if (string.IsNullOrWhiteSpace(diary.Content))
            {
                return;
            }

            diary.Resources = MediaResourceManager.GetDiaryResources(diary.Content);
            diary.UpdateTime = DateTime.Now;
            diary.Private = privacyMode;
            if (createMode)
            {
                createMode = false;
                bool isSuccess = await DiaryService.AddAsync(diary);

                if (!background)
                {
                    if (!isSuccess)
                    {
                        await AlertService.Error(I18n.T("Share.AddFail"));
                    }
                    else
                    {
                        _ = HandleAchievements();

                    }
                }
            }
            else
            {
                bool isSuccess = await DiaryService.UpdateIncludesAsync(diary);
                if (!background)
                {
                    if (!isSuccess)
                    {
                        await AlertService.Error(I18n.T("Share.EditFail"));
                    }
                    else
                    {
                        _ = HandleAchievements();
                    }
                }
            }
        }

        private void InvokeStateHasChanged(object? sender, BreakpointChangedEventArgs e)
        {
            StateHasChanged();
        }

        private string GetWeatherIcon(string? key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return "mdi-weather-cloudy";
            }

            return IconService.GetWeatherIcon(key);
        }

        private string GetMoodIcon(string? key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return "mdi-emoticon-outline";
            }

            return IconService.GetMoodIcon(key);
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

        private async Task HandleAchievements()
        {
            var messages = await AchievementService.UpdateUserState(Achievement.Diary);
            var alldiaries = await DiaryService.QueryAsync();
            var wordCount = alldiaries.GetWordCount(WordCountType);
            var messages2 = await AchievementService.UpdateUserState(Achievement.Word, wordCount);
            messages.AddRange(messages2);
            await AlertAchievements(messages);
        }

        private Task SettingChange(Setting setting, ref bool value)
        {
            value = !value;
            return SettingService.Set(setting, value);
        }

        private async Task InsertTimestamp()
        {
            string dateTimeNow = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            if (enableMarkdown)
            {
                await markdownEdit.InsertValueAsync(dateTimeNow);
            }
            else
            {
                await textareaEdit.InsertValueAsync(dateTimeNow);
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

        private void HandleLaunchActivation()
        {
            if (launchActivation)
            {
                return;
            }

            launchActivation = true;
            var args = AppLifecycle.ActivationArguments;
            Activated(args);
            AppLifecycle.ActivationArguments = null;
        }

        private void Activated(ActivationArguments? args)
        {
            if (args is null || args.Data is null)
            {
                return;
            }

            if (args.Kind == LaunchActivationKind.Share)
            {
                HandleShare((ShareActivationArguments)args.Data);
            }
        }

        private async void HandleShare(ShareActivationArguments? args)
        {
            if (args is null || args.Data is null)
            {
                return;
            }

            string? insertContent = null;
            if (args.Kind == ShareKind.Text)
            {
                insertContent = args.Data as string;
            }
            else if (args.Kind == ShareKind.FilePaths)
            {
                var filePaths = (List<string?>)args.Data;
                insertContent = await markdownEdit.CreateInsertContent(filePaths);
            }

            if (insertContent is null)
            {
                return;
            }

            if (enableMarkdown)
            {
                await markdownEdit.InsertValueAsync(insertContent);
            }
            else
            {
                await textareaEdit.InsertValueAsync(insertContent);
            }
        }
    }
}
