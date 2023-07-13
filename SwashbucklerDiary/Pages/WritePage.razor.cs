using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Components;
using SwashbucklerDiary.Extend;
using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Models;
using SwashbucklerDiary.Services;

namespace SwashbucklerDiary.Pages
{
    public partial class WritePage : PageComponentBase, IDisposable
    {
        private bool ShowTitle;
        private bool ShowMenu;
        private bool ShowSelectTag;
        private bool ShowWeather;
        private bool ShowMood;
        private bool ShowLocation;
        private bool ShowCreateTime;
        private bool Markdown = true;
        private bool EditCreateTime;
        private List<DynamicListItem> ListItemModels = new();
        private DiaryModel Diary = new()
        {
            Tags = new(),
            CreateTime = DateTime.Now
        };
        private List<TagModel> Tags = new();

        [Inject]
        private MasaBlazor MasaBlazor { get; set; } = default!;
        [Inject]
        private IDiaryService DiaryService { get; set; } = default!;
        [Inject]
        private ITagService TagService { get; set; } = default!;
        [Inject]
        private IconService IconService { get; set; } = default!;

        [Parameter]
        [SupplyParameterFromQuery]
        public Guid? TagId { get; set; }
        [Parameter]
        [SupplyParameterFromQuery]
        public Guid? DiaryId { get; set; }

        public void Dispose()
        {
            MasaBlazor.BreakpointChanged -= InvokeStateHasChanged;
            NavigateService.Action -= NavigateToBack;
            NavigateService.NavBtnAction -= SaveDiaryAsync;
            PlatformService.Stopped -= SaveDiary;
            GC.SuppressFinalize(this);
        }

        protected override async Task OnInitializedAsync()
        {
            MasaBlazor.BreakpointChanged += InvokeStateHasChanged;
            NavigateService.Action += NavigateToBack;
            NavigateService.NavBtnAction += SaveDiaryAsync;
            PlatformService.Stopped += SaveDiary;
            LoadView();
            await LoadSettings();
            await UpdateTags();
            await SetTag();
            await SetDiary();
        }

        protected override async void NavigateToBack()
        {
            if (!string.IsNullOrWhiteSpace(Diary.Content))
            {
                await SaveDiaryAsync();
            }

            base.NavigateToBack();
        }

        private List<TagModel> SelectedTags
        {
            get => Diary.Tags!;
            set => Diary.Tags = value;
        }
        private string? Weather
        {
            get => Diary.Weather;
            set => Diary.Weather = value;
        }
        private string? Mood
        {
            get => Diary.Mood;
            set => Diary.Mood = value;
        }
        private string? Location
        {
            get => Diary.Location;
            set => Diary.Location = value;
        }
        private DateOnly CreateTime
        {
            get => DateOnly.FromDateTime(Diary.CreateTime);
            set => Diary.CreateTime = value.ToDateTime(TimeOnly.FromDateTime(DateTime.Now));
        }
        private bool Desktop => MasaBlazor.Breakpoint.SmAndUp;
        private bool Mobile => !MasaBlazor.Breakpoint.SmAndUp;
        private Dictionary<string, string> WeatherIcons => IconService.WeatherIcon;
        private Dictionary<string, string> MoodIcons => IconService.MoodIcon;
        private string WeatherText =>
            string.IsNullOrEmpty(Diary.Weather) ? I18n.T("Write.Weather")! : I18n.T("Weather." + Diary.Weather)!;
        private string MoodText =>
            string.IsNullOrEmpty(Diary.Mood) ? I18n.T("Write.Mood")! : I18n.T("Mood." + Diary.Mood)!;

        private string ShowTitleText() => ShowTitle ? "Write.CloseTitle" : "Write.OpenTitle";

        private string MarkdownText() => Markdown ? "Diary.Text" : "Diary.Markdown";

        private string MarkdownIcon() => Markdown ? "mdi-format-text" : "mdi-language-markdown-outline";

        private async Task SetTag()
        {
            if (TagId != null)
            {
                var tag = await TagService.FindAsync((Guid)TagId);
                if (tag != null)
                {
                    SelectedTags.Add(tag);
                }
            }
        }

        private async Task SetDiary()
        {
            if (DiaryId == null)
            {
                return;
            }

            var diary = await DiaryService.FindIncludesAsync((Guid)DiaryId);
            if (diary == null)
            {
                return;
            }

            Diary = diary;
            ShowTitle = !string.IsNullOrEmpty(diary.Title);
        }

        private async Task LoadSettings()
        {
            ShowTitle = await SettingsService.Get(SettingType.Title);
            Markdown = await SettingsService.Get(SettingType.Markdown);
            EditCreateTime = await SettingsService.Get(SettingType.EditCreateTime);
        }

        private void LoadView()
        {
            ListItemModels = new()
            {
                new(this, ShowTitleText,"mdi-format-title",ShowTitleChanged),
                new(this, MarkdownText,MarkdownIcon,MarkdownChanged)
            };
        }

        private async Task UpdateTags()
        {
            Tags = await TagService.QueryAsync();
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
            ShowSelectTag = false;
        }

        private void OnSave()
        {
            if (string.IsNullOrWhiteSpace(Diary.Content))
            {
                return;
            }
            NavigateToBack();
        }

        private void OnClear()
        {
            Diary.Content = string.Empty;
            this.StateHasChanged();
        }

        private async void SaveDiary()
        {
            await SaveDiaryAsync();
        }

        private async Task SaveDiaryAsync()
        {
            if (string.IsNullOrWhiteSpace(Diary.Content))
            {
                return;
            }

            bool exist = await DiaryService.AnyAsync(it => it.Id == Diary.Id);
            if (!exist)
            {
                bool flag = await DiaryService.AddAsync(Diary);
                if (flag)
                {
                    await AlertService.Success(I18n.T("Share.AddSuccess"));
                    await HandleAchievements();
                }
                else
                {
                    await AlertService.Error(I18n.T("Share.AddFail"));
                }
            }
            else
            {
                bool flag = await DiaryService.UpdateIncludesAsync(Diary);
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

        private async Task MarkdownChanged()
        {
            Markdown = !Markdown;
            await SettingsService.Save(SettingType.Markdown, Markdown);
        }

        protected async Task HandleAchievements()
        {
            var messages = await AchievementService.UpdateUserState(AchievementType.Diary);
            var wordCountType = (WordCountType)Enum.Parse(typeof(WordCountType), I18n.T("Write.WordCountType")!);
            var wordCount = await DiaryService.GetWordCount(wordCountType);
            var messages2 = await AchievementService.UpdateUserState(AchievementType.Word, wordCount);
            messages.AddRange(messages2);
            foreach (var item in messages)
            {
                await AlertService.Success(I18n.T("Achievement.AchieveAchievements"), I18n.T(item));
            }
        }

        private async Task ShowTitleChanged()
        {
            ShowTitle = !ShowTitle;
            await SettingsService.Save(SettingType.Title, ShowTitle);
        }
    }
}
