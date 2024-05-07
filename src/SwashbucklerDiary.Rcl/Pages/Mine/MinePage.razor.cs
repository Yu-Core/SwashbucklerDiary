using BlazorComponent.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using SwashbucklerDiary.Rcl.Components;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Extensions;
using SwashbucklerDiary.Rcl.Models;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;
using System.Globalization;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class MinePage : ImportantComponentBase
    {
        private string? language;

        private Theme theme;

        private string? userName;

        private string? sign;

        private string? avatar;

        private bool showLanguage;

        private bool showTheme;

        private bool showFeedback;

        private bool showPreviewImage;

        private int diaryCount;

        private long wordCount;

        private int activeDayCount;

        private ScrollContainer scrollContainer = default!;

        private static readonly Dictionary<string, Theme> themeItems = new()
        {
            { "Theme.System", Theme.System },
            { "Theme.Light", Theme.Light },
            { "Theme.Dark", Theme.Dark },
        };

        private Dictionary<string, List<DynamicListItem>> ViewLists = [];

        private List<DynamicListItem> FeedbackTypes = [];

        private Dictionary<string, string> FeedbackTypeDatas = [];

        [Inject]
        protected IDiaryService DiaryService { get; set; } = default!;

        [Inject]
        protected IAccessExternal AccessExternal { get; set; } = default!;

        [Inject]
        protected ILogger<MinePage> Logger { get; set; } = default!;

        [Inject]
        protected IThemeService ThemeService { get; set; } = default!;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            LoadView();
            NavigateService.BeforePopToRoot += BeforePopToRoot;
            I18n.OnChanged += UpdateStatisticalData;
        }

        protected override async Task OnInitializedAsync()
        {
            await LoadViewAsync();

            await base.OnInitializedAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                await UpdateStatisticalDataAsync();

                StateHasChanged();
            }
        }

        protected override async Task OnResume()
        {
            await UpdateStatisticalDataAsync();
            await base.OnResume();
        }

        protected override void OnDispose()
        {
            NavigateService.BeforePopToRoot -= BeforePopToRoot;
            I18n.OnChanged -= UpdateStatisticalData;
            base.OnDispose();
        }

        protected override void ReadSettings()
        {
            base.ReadSettings();

            language = SettingService.Get<string>(Setting.Language);
            userName = SettingService.Get<string?>(Setting.UserName, null);
            sign = SettingService.Get<string?>(Setting.Sign, null);
            theme = (Theme)SettingService.Get<int>(Setting.Theme);
            avatar = SettingService.Get<string>(Setting.Avatar);
        }

        private WordCountStatistics WordCountType
            => (WordCountStatistics)Enum.Parse(typeof(WordCountStatistics), I18n.T("Write.WordCountType")!);


        private void LoadView()
        {
            ViewLists = new()
            {
                {
                    "Mine.Data",
                    new()
                    {
                        new(this, "Mine.Backups","mdi-folder-sync-outline",() => To("backups")),
                        new(this, "Mine.Export","mdi-export",() => To("export")),
                        new(this, "Mine.Achievement.Name","mdi-trophy-outline",() => To("achievement")),
                    }
                },
                {
                    "Mine.Settings",
                    new()
                    {
                        new(this,"Mine.Settings","mdi-cog-outline",() => To("setting")),
                        new(this,"Mine.Languages","mdi-web",() => showLanguage = true),
                        new(this,"Mine.Night","mdi-weather-night",() => showTheme = true),
                    }
                },
                {
                    "Mine.Other",
                    new()
                    {
                        new(this,"Mine.Feedback","mdi-email-outline",() => showFeedback = true),
                        new(this,"Mine.About","mdi-information-outline",() => To("about")),
                    }
                }
            };
            FeedbackTypes =
            [
                new(this, "Email","mdi-email-outline",SendMail),
                new(this, "Github","mdi-github",ToGithub),
                new(this, "QQGroup","mdi-qqchat",OpenQQGroup),
            ];
        }

        private async Task LoadViewAsync()
        {
            FeedbackTypeDatas = await StaticWebAssets.ReadJsonAsync<Dictionary<string, string>>("json/feedback-type/feedback-type.json");
        }

        private async Task LanguageChanged(string value)
        {
            I18n.SetCulture(value);
            await SettingService.Set(Setting.Language, value);
        }

        private async Task SendMail()
        {
            var mail = FeedbackTypeDatas["Email"];
            try
            {
                bool isSuccess = await PlatformIntegration.SendEmail(null, null, [mail]);
                if (isSuccess)
                {
                    return;
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e, "SendMailFail");
            }

            await PlatformIntegration.SetClipboard(mail);
            await AlertService.Success(I18n.T("Mine.MailCopy"));
        }

        private async Task ToGithub()
        {
            var githubUrl = FeedbackTypeDatas["Github"];
            await PlatformIntegration.OpenBrowser(githubUrl);
        }

        private async Task ThemeChanged(Theme value)
        {
            await Task.WhenAll(
                ThemeService.SetThemeAsync(value),
                SettingService.Set(Setting.Theme, (int)value));
        }

        private async Task OpenQQGroup()
        {
            try
            {
                bool isSuccess = await AccessExternal.JoinQQGroup();
                if (isSuccess)
                {
                    return;
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e, "JoinQQGroupError");
            }

            var qqGroup = FeedbackTypeDatas["QQGroup"];
            await PlatformIntegration.SetClipboard(qqGroup);
            await AlertService.Success(I18n.T("Mine.QQGroupCopy"));
        }

        private Task Search(string? value)
        {
            To($"searchAppFunction?query={value}");
            return Task.CompletedTask;
        }

        private async Task UpdateStatisticalDataAsync()
        {
            var diries = await DiaryService.QueryAsync(it => !it.Private);
            diaryCount = diries.Count;
            wordCount = diries.GetWordCount(WordCountType);
            activeDayCount = diries.Select(it => DateOnly.FromDateTime(it.CreateTime)).Distinct().Count();
        }

        private async void UpdateStatisticalData(CultureInfo _)
        {
            await UpdateStatisticalDataAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task BeforePopToRoot(PopEventArgs args)
        {
            if (thisPageUrl == args.PreviousUri && thisPageUrl == args.NextUri)
            {
                await JS.ScrollTo($"#{scrollContainer.Id}", 0);
            }
        }
    }
}
