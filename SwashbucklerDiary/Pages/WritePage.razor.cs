using BlazorComponent;
using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.Maui.Controls;
using SwashbucklerDiary.Components;
using SwashbucklerDiary.Extensions;
using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Models;
using SwashbucklerDiary.Services;
using SwashbucklerDiary.Utilities;
using System.Text.RegularExpressions;

namespace SwashbucklerDiary.Pages
{
    public partial class WritePage : ImportantComponentBase, IDisposable
    {
        private bool ShowMenu;
        private bool ShowSelectTag;
        private bool ShowWeather;
        private bool ShowMood;
        private bool ShowLocation;
        private bool ShowCreateTime;
        private bool EnableTitle;
        private bool EnableMarkdown = true;
        private bool EnableEditCreateTime;
        private bool CreateMode;
        private bool Overlay;
        private MyMarkdown? MyMarkdown;
        private MTextareaExtension? MTextareaExtension;
        private List<DynamicListItem> MenuItems = new();
        private DiaryModel Diary = new()
        {
            Tags = new(),
            Resources = new(),
            CreateTime = default
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

        protected override void OnInitialized()
        {
            MasaBlazor.BreakpointChanged += InvokeStateHasChanged;
            NavigateService.BeforePop += BeforePop;
            NavigateService.BeforePopToRoot += BeforePopToRoot;
            PlatformService.Stopped += LeaveAppSaveDiary;
            LoadView();

            base.OnInitialized();
        }

        protected override async Task OnInitializedAsync()
        {
            await LoadSettings();
            await UpdateTags();
            await SetTag();
            await SetDiary();
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
            PlatformService.Stopped -= LeaveAppSaveDiary;
            base.OnDispose();
        }

        private DateTime CreateTime
        {
            get => (CreateMode && Diary.CreateTime == default) ? DateTime.Now : Diary.CreateTime;
            set => Diary.CreateTime = value;
        }
        private List<TagModel> SelectedTags
        {
            get => Diary.Tags!;
            set => Diary.Tags = value;
        }
        private StringNumber? Weather
        {
            get => Diary.Weather;
            set => Diary.Weather = value?.ToString();
        }
        private StringNumber? Mood
        {
            get => Diary.Mood;
            set => Diary.Mood = value?.ToString();
        }
        private string? Location
        {
            get => Diary.Location;
            set => Diary.Location = value;
        }
        private DateOnly CreateDate
        {
            get => DateOnly.FromDateTime(CreateTime);
            set => CreateTime = value.ToDateTime(TimeOnly.FromDateTime(DateTime.Now));
        }
        private bool Desktop => MasaBlazor.Breakpoint.SmAndUp;
        private bool Mobile => !MasaBlazor.Breakpoint.SmAndUp;
        private static Dictionary<string, string> WeatherIcons => IconService.WeatherIcon;
        private static Dictionary<string, string> MoodIcons => IconService.MoodIcon;
        private string WeatherText =>
            string.IsNullOrEmpty(Diary.Weather) ? I18n.T("Write.Weather")! : I18n.T("Weather." + Diary.Weather)!;
        private string MoodText =>
            string.IsNullOrEmpty(Diary.Mood) ? I18n.T("Write.Mood")! : I18n.T("Mood." + Diary.Mood)!;

        private string SetTitleText() => EnableTitle ? "Write.CloseTitle" : "Write.EnableTitle";

        private string SetMarkdownText() => EnableMarkdown ? "Diary.Text" : "Diary.Markdown";

        private string SetMarkdownIcon() => EnableMarkdown ? "mdi-format-text" : "mdi-language-markdown-outline";

        private string SetEditCreateTimeText() => EnableEditCreateTime ? "Write.CloseEditCreateTime" : "Write.EnableEditCreateTime";

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
                CreateMode = true;
                return;
            }

            var diary = await DiaryService.FindAsync((Guid)DiaryId);
            if (diary == null)
            {
                return;
            }

            Diary = diary;
            Diary.Content = StaticCustomScheme.CustomSchemeRender(Diary.Content);
            EnableTitle = !string.IsNullOrEmpty(diary.Title);
        }

        private async Task LoadSettings()
        {
            EnableTitle = await SettingsService.Get(SettingType.Title);
            EnableMarkdown = await SettingsService.Get(SettingType.Markdown);
            EnableEditCreateTime = await SettingsService.Get(SettingType.EditCreateTime);
        }

        private void LoadView()
        {
            MenuItems = new()
            {
                new(this, SetTitleText,"mdi-format-title",()=> SettingChange(SettingType.Title,ref EnableTitle)),
                new(this, SetMarkdownText,SetMarkdownIcon,()=> SettingChange(SettingType.Markdown,ref EnableMarkdown)),
                new(this, SetEditCreateTimeText,"mdi-calendar-edit-outline",()=> SettingChange(SettingType.EditCreateTime,ref EnableEditCreateTime))
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

        private async void LeaveAppSaveDiary()
        {
            await SaveDiaryAsync(false);
        }

        private Task SaveDiaryAsync() => SaveDiaryAsync(true);

        private async Task SaveDiaryAsync(bool alert)
        {
            if (EnableMarkdown)
            {
                // vditor 每次输入会触发渲染，所以有1秒左右的防抖
                // https://github.com/Vanessa219/vditor/issues/1307
                // https://github.com/Vanessa219/vditor/issues/574
                Overlay = true;
                await InvokeAsync(StateHasChanged);

                await Task.Delay(800);

                Overlay = false;
                await InvokeAsync(StateHasChanged);
            }

            if (string.IsNullOrWhiteSpace(Diary.Content))
            {
                return;
            }

            Diary.Content = StaticCustomScheme.ReverseCustomSchemeRender(Diary.Content!);
            Diary.Resources = GetDiaryResources(Diary.Content);
            Diary.UpdateTime = DateTime.Now;
            if (CreateMode)
            {
                CreateMode = false;
                if (Diary.CreateTime == default)
                {
                    Diary.CreateTime = DateTime.Now;
                }

                bool flag = await DiaryService.AddAsync(Diary);

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
                    await HandleAchievements();
                }
            }
            else
            {
                bool flag = await DiaryService.UpdateIncludesAsync(Diary);
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

        protected async Task HandleAchievements()
        {
            var messages = await AchievementService.UpdateUserState(AchievementType.Diary);
            var wordCountType = (WordCountType)Enum.Parse(typeof(WordCountType), I18n.T("Write.WordCountType")!);
            var wordCount = await DiaryService.GetWordCount(wordCountType);
            var messages2 = await AchievementService.UpdateUserState(AchievementType.Word, wordCount);
            messages.AddRange(messages2);
            await AlertAchievements(messages);
        }

        private Task SettingChange(SettingType type, ref bool value)
        {
            value = !value;
            return SettingsService.Save(type, value);
        }

        private static List<ResourceModel> GetDiaryResources(string content)
        {
            var resources = new List<ResourceModel>();
            string pattern = @"(?<=\(|"")(appdata:///\S+?)(?=\)|"")"; ;

            MatchCollection matches = Regex.Matches(content, pattern);

            foreach (Match match in matches.Cast<Match>())
            {
                resources.Add(new()
                {
                    ResourceType = GetResourceType(match.Value),
                    ResourceUri = match.Value,
                });
            }

            return resources;
        }

        private static ResourceType GetResourceType(string uri)
        {
            var mime = StaticContentProvider.GetResponseContentTypeOrDefault(uri);
            var type = mime.Split('/')[0];

            return type switch
            {
                "image" => ResourceType.Image,
                "audio" => ResourceType.Audio,
                "video" => ResourceType.Video,
                _ => ResourceType.Unknown
            };
        }

        private async Task InsertTimestamp()
        {
            string dateTimeNow = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            if (EnableMarkdown)
            {
                await MyMarkdown!.InsertValueAsync(dateTimeNow);
            }
            else
            {
                await MTextareaExtension!.InsertValueAsync(dateTimeNow);
            }
        }
    }
}
