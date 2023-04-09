using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Components;
using SwashbucklerDiary.Extend;
using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Models;
using SwashbucklerDiary.Services;

namespace SwashbucklerDiary.Pages
{
    public partial class WritePage : PageComponentBase, IAsyncDisposable
    {
        private bool ShowTitle;
        private bool ShowMenu;
        private bool ShowSelectTag;
        private bool ShowWeather;
        private bool ShowMood;
        private bool ShowLocation;
        private bool Markdown = true;
        private bool IsSaved;
        private List<ViewListItem> ViewListItems = new();
        private DiaryModel Diary = new()
        {
            Tags = new(),
            CreateTime = DateTime.Now
        };
        private List<TagModel> Tags = new();

        [Inject]
        public MasaBlazor MasaBlazor { get; set; } = default!;
        [Inject]
        public IDiaryService DiaryService { get; set; } = default!;
        [Inject]
        public ITagService TagService { get; set; } = default!;
        [Inject]
        public IconService IconService { get; set; } = default!;

        [Parameter]
        [SupplyParameterFromQuery]
        public Guid? TagId { get; set; }
        [Parameter]
        [SupplyParameterFromQuery]
        public Guid? DiaryId { get; set; }

        public async ValueTask DisposeAsync()
        {
            MasaBlazor.Breakpoint.OnUpdate -= InvokeStateHasChangedAsync;
            NavigateService.Action -= NavigateToBack;
            if(!IsSaved)
            {
                await SaveDiary();
            }
            GC.SuppressFinalize(this);
        }

        protected override async Task OnInitializedAsync()
        {
            MasaBlazor.Breakpoint.OnUpdate += InvokeStateHasChangedAsync;
            NavigateService.Action += NavigateToBack;
            LoadView();
            await LoadSettings();
            await UpdateTags();
            await SetTag();
            await SetDiary();
        }

        protected override async void NavigateToBack()
        {
            if (string.IsNullOrWhiteSpace(Diary.Content))
            {
                base.NavigateToBack();
                return;
            }

            await SaveDiaryAndToBack();
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
        }

        private void LoadView()
        {
             ViewListItems = new()
            {
                new(ShowTitleText,"mdi-format-title",ShowTitleChanged),
                new(MarkdownText,MarkdownIcon,MarkdownChanged)
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

        private Task OnSave()
        {
            return SaveDiaryAndToBack();
        }

        private void OnClear()
        {
            Diary.Content = string.Empty;
            this.StateHasChanged();
        }

        private async Task SaveDiary()
        {
            if (string.IsNullOrWhiteSpace(Diary.Content))
            {
                return;
            }

            if (DiaryId == null)
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

            IsSaved = true;
        }

        private async Task SaveDiaryAndToBack()
        {
            await SaveDiary();
            base.NavigateToBack();
        }

        private async Task InvokeStateHasChangedAsync()
        {
            await InvokeAsync(StateHasChanged);
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
