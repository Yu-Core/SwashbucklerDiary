using BlazorComponent;
using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Components;
using SwashbucklerDiary.Rcl.Essentials;
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

        private List<TagModel> Tags = [];

        private Dictionary<string, string> WeatherIcons = [];

        private Dictionary<string, string> MoodIcons = [];

        [Inject]
        private MasaBlazor MasaBlazor { get; set; } = default!;

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
            MasaBlazor.BreakpointChanged += InvokeStateHasChanged;
            NavigateService.BeforePop += BeforePop;
            NavigateService.BeforePopToRoot += BeforePop;
            AppLifecycle.Stopped += LeaveAppSaveDiary;
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
            MasaBlazor.BreakpointChanged -= InvokeStateHasChanged;
            NavigateService.BeforePop -= BeforePop;
            NavigateService.BeforePopToRoot -= BeforePop;
            AppLifecycle.Stopped -= LeaveAppSaveDiary;
            timer?.Dispose();
            base.OnDispose();
        }

        protected override void UpdateSettings()
        {
            base.UpdateSettings();

            enableTitle = SettingService.Get<bool>(Setting.Title);
            enableMarkdown = SettingService.Get<bool>(Setting.Markdown);
            editAutoSave = SettingService.Get<int>(Setting.EditAutoSave);
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

        private bool Desktop => MasaBlazor.Breakpoint.SmAndUp;

        private bool Mobile => !MasaBlazor.Breakpoint.SmAndUp;

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
            Tags = await TagService.QueryAsync();

            if (DiaryId is null && TagId is not null)
            {
                var tag = Tags.Find(it => it.Id == TagId);
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
            menuItems = new()
            {
                new(this, TitleSwitchText, "mdi-format-title", ()=> SettingChange(Setting.Title, ref enableTitle)),
                new(this, MarkdownSwitchText, MarkdownSwitchIcon, ()=> SettingChange(Setting.Markdown, ref enableMarkdown)),
            };
            WeatherIcons = IconService.GetWeatherIcons();
            MoodIcons = IconService.GetMoodIcons();
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
            await SaveDiaryAsync(true);
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
            if (createMode)
            {
                createMode = false;
                bool flag = await DiaryService.AddAsync(diary);

                if (!background)
                {
                    if (flag)
                    {
                        await AlertService.Success(I18n.T("Share.AddSuccess"));
                    }
                    else
                    {
                        await AlertService.Error(I18n.T("Share.AddFail"));
                    }
                }

                if (flag)
                {
                    _ = HandleAchievements(background);
                }
            }
            else
            {
                bool flag = await DiaryService.UpdateIncludesAsync(diary);
                if (!background)
                {
                    if (flag)
                    {
                        await AlertService.Success(I18n.T("Share.EditSuccess"));
                    }
                    else
                    {
                        await AlertService.Error(I18n.T("Share.EditFail"));
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

        private async Task HandleAchievements(bool background = false)
        {
            var messages = await AchievementService.UpdateUserState(Achievement.Diary);
            var wordCount = await DiaryService.GetWordCount(WordCountType);
            var messages2 = await AchievementService.UpdateUserState(Achievement.Word, wordCount);
            if (!background)
            {
                messages.AddRange(messages2);
                await AlertAchievements(messages);
            }
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

        private async Task BeforePop(PopEventArgs e)
        {
            if (!IsCurrentPage)
            {
                return;
            }

            timer?.Dispose();
            await SaveDiaryAsync();
        }

        private async Task CreateTimer()
        {
            timer = new PeriodicTimer(TimeSpan.FromSeconds(editAutoSave));
            while (await timer.WaitForNextTickAsync())
            {
                await SaveDiaryAsync(true);
            }
        }
    }
}
