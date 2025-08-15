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
        private static readonly string cssHref = $"_content/{Rcl.Essentials.StaticWebAssets.RclAssemblyName}/css/extend/masa-blazor-extend-enqueued-snackbars-write.css";

        private bool showMenu;

        private bool showSelectTag;

        private bool showWeather;

        private bool showMood;

        private bool showLocation;

        private bool showDate;

        private bool showTime;

        private bool showTemplate;

        private bool enableTitle;

        private bool enableMarkdown = true;

        private bool overlay;

        private bool autofocus = true;

        private bool launchActivation;

        private bool showIconText;

        private bool showOtherInfo;

        private bool waitSelectTemplate = true;

        private bool outline;

        private bool? showMoblieOutline = false;

        private int editAutoSave;

        private UseTemplateKind useTemplateMethod;

        private bool selectTemplateWhenCreate;

        private bool showReference;

        private string? diaryTimeFormat;

        private Guid? defaultTemplateId;

        private string? diaryInsertTimeFormat;

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
        private IAppLifecycle AppLifecycle { get; set; } = default!;

        [Inject]
        private IMediaResourceManager MediaResourceManager { get; set; } = default!;

        [Inject]
        private BreakpointService BreakpointService { get; set; } = default!;

        [SupplyParameterFromQuery]
        private Guid? TagId { get; set; }

        [SupplyParameterFromQuery]
        private Guid? DiaryId { get; set; }

        [SupplyParameterFromQuery]
        private DateOnly? CreateDate { get; set; }

        [SupplyParameterFromQuery]
        private bool Template { get; set; }

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
                    InitTags(),
                    InitTemplate());

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
            useTemplateMethod = (UseTemplateKind)SettingService.Get(it => it.UseTemplateMethod);
            selectTemplateWhenCreate = SettingService.Get(it => it.SelectTemplateWhenCreate);
            outline = SettingService.Get(it => it.Outline);
            diaryTimeFormat = SettingService.Get(it => it.DiaryTimeFormat);
            string defaultTemplateIdString = SettingService.Get(s => s.DefaultTemplateId);
            if (Guid.TryParse(defaultTemplateIdString, out var defaultTemplateId))
            {
                this.defaultTemplateId = defaultTemplateId;
            }

            diaryInsertTimeFormat = SettingService.Get(it => it.DiaryInsertTimeFormat);
        }

        private List<TagModel> SelectedTags
        {
            get => diary.Tags!;
            set => diary.Tags = value;
        }

        private DateOnly SelectedDate
        {
            get => DateOnly.FromDateTime(diary.CreateTime);
            set => diary.CreateTime = value.ToDateTime(SelectedTime);
        }

        private TimeOnly SelectedTime
        {
            get => TimeOnly.FromDateTime(diary.CreateTime);
            set => diary.CreateTime = SelectedDate.ToDateTime(value);
        }

        private string WeatherIcon => GetWeatherIcon(diary.Weather);

        private string MoodIcon => GetMoodIcon(diary.Mood);

        private string WeatherText =>
            string.IsNullOrEmpty(diary.Weather) ? I18n.T("Weather")! : I18n.T(diary.Weather)!;

        private string MoodText =>
            string.IsNullOrEmpty(diary.Mood) ? I18n.T("Mood")! : I18n.T(diary.Mood)!;

        private string LocationText =>
            string.IsNullOrEmpty(diary.Location) ? I18n.T("Location") : diary.Location;

        private string TitleSwitchText() => enableTitle ? "Close title" : "Open title";

        private string MarkdownSwitchText() => enableMarkdown ? "Text mode" : "Markdown mode";

        private string MarkdownSwitchIcon() => enableMarkdown ? "description" : "markdown";

        private string OtherInfoSwitchText() => showOtherInfo ? "Hide other information" : "Display other information";

        private string TemplateSwitchText() => diary.Template ? "Switch to Diary" : "Switch to Template";

        private string TemplateSwitchIcon() => diary.Template ? "book" : "space_dashboard";

        private string OutlineText() => outline ? "Hide outline" : "Display outline";

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
            if (DiaryId is not null || CreateDate is not DateOnly createDate)
            {
                return;
            }

            SelectedDate = createDate;
        }

        private async Task InitTemplate()
        {
            waitSelectTemplate = false;

            if (DiaryId is not null)
            {
                return;
            }

            if (Template)
            {
                this.diary.Template = true;
                return;
            }

            if (this.defaultTemplateId is Guid defaultTemplateId)
            {
                var template = await DiaryService.FindAsync(defaultTemplateId);
                if (template is not null && template.Template)
                {
                    await UseTemplate(template, UseTemplateKind.Cover);
                    return;
                }
            }

            if (selectTemplateWhenCreate)
            {
                waitSelectTemplate = true;
                showTemplate = true;
                autofocus = false;
            }
        }

        private void LoadView()
        {
            menuItems =
            [
                new(this, TitleSwitchText, "title", ()=> SettingChange(nameof(Setting.Title), ref enableTitle)),
                new(this, MarkdownSwitchText, MarkdownSwitchIcon, ()=> SettingChange(nameof(Setting.Markdown), ref enableMarkdown)),
                new(this, OtherInfoSwitchText, "info", ()=> SettingChange(nameof(Setting.OtherInfo), ref showOtherInfo)),
                new(this, TemplateSwitchText, TemplateSwitchIcon, ()=> diary.Template = !diary.Template),
                new(this, "Reference diary", "format_quote", ()=> showReference = true),
                new(this, "Outline", "format_list_bulleted", ()=>showMoblieOutline=true, ()=>!BreakpointService.Breakpoint.MdAndUp && enableMarkdown),
                new(this, OutlineText, "format_list_bulleted", ()=> SettingChange(nameof(Setting.Outline), ref outline), ()=>BreakpointService.Breakpoint.MdAndUp),
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
                    await AlertService.ErrorAsync(I18n.T("Add failed"));
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
                    await AlertService.ErrorAsync(I18n.T("Change failed"));
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
                return "sunny";
            }

            return GlobalConfiguration.GetWeatherIcon(key);
        }

        private string GetMoodIcon(string? key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return "mdi:mdi-emoticon-happy-outline";
            }

            return GlobalConfiguration.GetMoodIcon(key);
        }

        private string CounterValue()
        {
            return $"{diary.GetWordCount()} {I18n.T("Word count unit")}";
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
                var alldiaries = await DiaryService.QueryDiariesAsync();
                var wordCount = alldiaries.GetWordCount();
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
            string dateTimeNow = DateTime.Now.ToString(diaryInsertTimeFormat);
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

            AlertService.StartLoading();

            try
            {
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

                if (insertContent is not null)
                {
                    await InsertValueAsync(insertContent);
                }
            }
            finally
            {
                AlertService.StopLoading();
            }
        }

        private Task UseTemplate(DiaryModel template)
            => UseTemplate(template, this.useTemplateMethod);

        private async Task UseTemplate(DiaryModel template, UseTemplateKind useTemplateMethod)
        {
            showTemplate = false;

            if (useTemplateMethod == UseTemplateKind.Cover)
            {
                diary.Title = template.Title;
                diary.Content = template.Content;
                diary.Weather = template.Weather;
                diary.Mood = template.Mood;
                diary.Location = template.Location;
                diary.Tags = [.. template.Tags ?? []];
            }
            else if (useTemplateMethod == UseTemplateKind.Insert)
            {
                if (!string.IsNullOrEmpty(template.Content))
                {
                    await InsertValueAsync(template.Content);
                }

                diary.Tags = (diary.Tags ?? []).Union(template.Tags ?? []).ToList();
            }
        }

        private async Task InsertReferenceAsync(DiaryModel value)
        {
            showReference = false;
            var text = value.GetReferenceText(I18n);
            await InsertValueAsync(text);
        }
    }
}
