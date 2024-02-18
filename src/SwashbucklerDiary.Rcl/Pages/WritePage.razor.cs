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
    public partial class WritePage : ImportantComponentBase, IDisposable
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

        private bool afterRender;

        private MarkdownEdit markdownEdit = default!;

        private TextareaEdit textareaEdit = default!;

        private List<DynamicListItem> menuItems = [];

        private DiaryModel diary = new()
        {
            Tags = [],
            Resources = [],
            CreateTime = default
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
            NavigateService.BeforePopToRoot += BeforePopToRoot;
            AppLifecycle.Stopped += LeaveAppSaveDiary;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                afterRender = true;
                await Task.WhenAll(
                    UpdateSettings(),
                    InitDiary(),
                    InitTags());
                InitCreateTime();
                StateHasChanged();
            }
        }

        private async Task BeforePop(PopEventArgs e)
        {
            if (!IsCurrentPage)
            {
                return;
            }

            await SaveDiaryAsync();
        }

        private async Task BeforePopToRoot(PopEventArgs e)
        {
            if (!IsCurrentPage)
            {
                return;
            }

            await SaveDiaryAsync();
        }

        protected override void OnDispose()
        {
            MasaBlazor.BreakpointChanged -= InvokeStateHasChanged;
            NavigateService.BeforePop -= BeforePop;
            NavigateService.BeforePopToRoot -= BeforePopToRoot;
            AppLifecycle.Stopped -= LeaveAppSaveDiary;
            base.OnDispose();
        }

        private DateTime DiaryCreateTime
        {
            get => (createMode && diary.CreateTime == default) ? DateTime.Now : diary.CreateTime;
            set => diary.CreateTime = value;
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
            get => DateOnly.FromDateTime(DiaryCreateTime);
            set => DiaryCreateTime = value.ToDateTime();
        }

        private bool Desktop => MasaBlazor.Breakpoint.SmAndUp;

        private bool Mobile => !MasaBlazor.Breakpoint.SmAndUp;

        private string WeatherIcon => GetWeatherIcon(diary.Weather);

        private string WeatherText =>
            string.IsNullOrEmpty(diary.Weather) ? I18n.T("Write.Weather")! : I18n.T("Weather." + diary.Weather)!;

        private string MoodIcon => GetMoodIcon(diary.Mood);

        private string MoodText =>
            string.IsNullOrEmpty(diary.Mood) ? I18n.T("Write.Mood")! : I18n.T("Mood." + diary.Mood)!;

        private WordCountStatistics WordCountType => (WordCountStatistics)Enum.Parse(typeof(WordCountStatistics), I18n.T("Write.WordCountType")!);

        private string SetTitleText() => enableTitle ? "Write.CloseTitle" : "Write.EnableTitle";

        private string SetMarkdownText() => enableMarkdown ? "Diary.Text" : "Diary.Markdown";

        private string SetMarkdownIcon() => enableMarkdown ? "mdi-format-text" : "mdi-language-markdown-outline";

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

        private async Task UpdateSettings()
        {
            enableTitle = await SettingService.Get<bool>(Setting.Title);
            enableMarkdown = await SettingService.Get<bool>(Setting.Markdown);
        }

        private void LoadView()
        {
            menuItems = new()
            {
                new(this, SetTitleText, "mdi-format-title", ()=> SettingChange(Setting.Title, ref enableTitle)),
                new(this, SetMarkdownText, SetMarkdownIcon, ()=> SettingChange(Setting.Markdown, ref enableMarkdown)),
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
            await SaveDiaryAsync(false);
        }

        private Task SaveDiaryAsync() => SaveDiaryAsync(true);

        private async Task SaveDiaryAsync(bool alert)
        {
            if (enableMarkdown)
            {
                // vditor 每次输入会触发渲染，所以有1秒左右的防抖
                // https://github.com/Vanessa219/vditor/issues/1307
                // https://github.com/Vanessa219/vditor/issues/574
                overlay = true;
                await InvokeAsync(StateHasChanged);

                await Task.Delay(800);

                overlay = false;
                await InvokeAsync(StateHasChanged);
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
                if (diary.CreateTime == default)
                {
                    diary.CreateTime = DateTime.Now;
                }
                else
                {
                    diary.CreateTime = DateOnly.FromDateTime(diary.CreateTime).ToDateTime();
                }

                bool flag = await DiaryService.AddAsync(diary);

                if (alert)
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
                    _ = HandleAchievements();
                }
            }
            else
            {
                bool flag = await DiaryService.UpdateIncludesAsync(diary);
                if (alert)
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

        protected async Task HandleAchievements()
        {
            var messages = await AchievementService.UpdateUserState(Achievement.Diary);
            var wordCount = await DiaryService.GetWordCount(WordCountType);
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
    }
}
