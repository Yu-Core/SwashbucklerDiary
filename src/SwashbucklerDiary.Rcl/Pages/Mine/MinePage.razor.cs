using BlazorComponent.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using SwashbucklerDiary.Rcl.Components;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Models;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

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
                await UpdateData();

                StateHasChanged();
            }
        }

        protected override async Task OnResume()
        {
            await UpdateData();
            await base.OnResume();
        }

        protected override void OnDispose()
        {
            NavigateService.BeforePopToRoot -= BeforePopToRoot;
            base.OnDispose();
        }

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
            FeedbackTypes = new()
            {
                new(this, "Email","mdi-email-outline",SendMail),
                new(this, "Github","mdi-github",ToGithub),
                new(this, "QQGroup","mdi-qqchat",OpenQQGroup),
            };
        }

        private async Task LoadViewAsync()
        {
            FeedbackTypeDatas = await StaticWebAssets.ReadJsonAsync<Dictionary<string, string>>("json/feedback-type/feedback-type.json");
        }

        private async Task LanguageChanged(string value)
        {
            I18n.SetCulture(value);
            await Preferences.Set(Setting.Language, value);
            userName = await Preferences.Get(Setting.UserName, I18n.T("AppName"));
            sign = await Preferences.Get(Setting.Sign, I18n.T("Mine.Sign"));
        }

        private async Task SendMail()
        {
            var mail = FeedbackTypeDatas["Email"];
            try
            {
                bool isMailSupported = await PlatformIntegration.IsMailSupported();
                if (isMailSupported)
                {
                    List<string> recipients = new() { mail };

                    await PlatformIntegration.SendEmail(null, null, recipients);
                }
                else
                {
                    await PlatformIntegration.SetClipboard(mail);
                    await AlertService.Success(I18n.T("Mine.MailCopy"));
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e, "SendMailFail");
                await AlertService.Error(I18n.T("Mine.SendMailFail"));
            }
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
                Preferences.Set(Setting.Theme, (int)value));
        }

        private async Task OpenQQGroup()
        {
            var qqGroup = FeedbackTypeDatas["QQGroup"];
            try
            {
                bool flag = await AccessExternal.JoinQQGroup();
                if (!flag)
                {
                    await PlatformIntegration.SetClipboard(qqGroup);
                    await AlertService.Success(I18n.T("Mine.QQGroupCopy"));
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e, "JoinQQGroupError");
                await AlertService.Error(I18n.T("Mine.QQGroupError"));
            }
        }

        private Task Search(string? value)
        {
            To($"searchAppFunction?query={value}");
            return Task.CompletedTask;
        }

        private int GetWordCount(List<DiaryModel> diaries)
        {
            var type = (WordCountStatistics)Enum.Parse(typeof(WordCountStatistics), I18n.T("Write.WordCountType")!);
            return DiaryService.GetWordCount(diaries, type);
        }

        private async Task UpdateSettings()
        {
            var languageTask = Preferences.Get<string>(Setting.Language);
            var userNameTask = Preferences.Get(Setting.UserName, I18n.T("AppName"));
            var signTask = Preferences.Get(Setting.Sign, I18n.T("Mine.Sign"));
            var themeTask = Preferences.Get<int>(Setting.Theme);
            var avatarTask = Preferences.Get<string>(Setting.Avatar);

            await Task.WhenAll(
                languageTask,
                userNameTask,
                signTask,
                themeTask,
                avatarTask);

            language = languageTask.Result;
            userName = userNameTask.Result;
            sign = signTask.Result;
            theme = (Theme)themeTask.Result;
            avatar = avatarTask.Result;
        }

        private async Task UpdateStatisticalData()
        {
            var diries = await DiaryService.QueryAsync(it => !it.Private);
            diaryCount = diries.Count;
            wordCount = GetWordCount(diries);
            activeDayCount = diries.Select(it => DateOnly.FromDateTime(it.CreateTime)).Distinct().Count();
        }

        private Task UpdateData()
        {
            return Task.WhenAll(
                UpdateSettings(),
                UpdateStatisticalData());
        }

        private async Task BeforePopToRoot(PopEventArgs args)
        {
            if (thisPageUrl == args.PreviousUri && thisPageUrl == args.NextUri)
            {
                await JS.ScrollTo(scrollContainer.Ref, 0);
            }
        }
    }
}
