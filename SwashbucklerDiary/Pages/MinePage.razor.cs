using Microsoft.AspNetCore.Components;
using Microsoft.Maui.ApplicationModel;
using SwashbucklerDiary.Components;
using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Pages
{
    public partial class MinePage : PageComponentBase
    {
        private int DiaryCount;
        private long WordCount;
        private int ActiveDayCount;
        private string? Language;
        private ThemeState ThemeState;
        private string? UserName;
        private string? Sign;
        private string? Avatar;
        private bool ShowLanguage;
        private bool ShowThemeState;
        private bool ShowFeedback;
        private Dictionary<string, string> Languages => I18n.Languages;
        private readonly static Dictionary<string, ThemeState> ThemeStates = new()
        {
            {"ThemeState.System",ThemeState.System },
            {"ThemeState.Light",ThemeState.Light },
            {"ThemeState.Dark",ThemeState.Dark },
        };
        private Dictionary<string, List<ListItemModel>> ViewLists = new();
        private List<ListItemModel> FeedbackTypes = new();
        private const string githubUrl = "https://github.com/Yu-Core/SwashbucklerDiary";
        private const string mail = "yu-core@qq.com";
        private const string qqGroup = "139864402";

        [Inject]
        private IDiaryService DiaryService { get; set; } = default!;
        [Inject]
        private ILocalImageService LocalImageService { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            LoadView();
            await SetCount();
            await LoadSettings();
            await SetAvatar();
        }

        private string MRadioColor => ThemeService.Dark ? "white" : "black";

        private async Task SetCount()
        {
            DiaryCount = await DiaryService.CountAsync(it=>!it.Private);
            var wordCountType = (WordCountType)Enum.Parse(typeof(WordCountType), I18n.T("Write.WordCountType")!);
            WordCount = await DiaryService.GetWordCount(wordCountType);
            var diaries = await DiaryService.QueryAsync(it => !it.Private);
            ActiveDayCount = diaries.Select(it => DateOnly.FromDateTime(it.CreateTime)).Distinct().Count();
        }

        private void LoadView()
        {
            ViewLists = new()
            {
                {
                    "Mine.Data",
                    new()
                    {
                        new("Mine.Backups","mdi-folder-sync-outline",EC(() => To("/backups"))),
                        new("Mine.Export","mdi-export",EC(() => To("/export"))),
                        new("Mine.Achievement","mdi-trophy-outline",EC(() => To("/achievement"))),
                    }
                },
                {
                    "Mine.Settings",
                    new()
                    {
                        new("Mine.Settings","mdi-cog-outline",EC(() => To("/setting"))),
                        new("Mine.Languages","mdi-web",EC(() => ShowLanguage = true)),
                        new("Mine.Night","mdi-weather-night",EC(() => ShowThemeState = true)),
                    }
                },
                {
                    "Mine.Other",
                    new()
                    {
                        new("Mine.Feedback","mdi-email-outline",EC(() => ShowFeedback = true)),
                        new("Mine.About","mdi-information-outline",EC(() => To("/about"))),
                    }
                }
            };
            FeedbackTypes = new()
            {
                new("Email","mdi-email-outline",EC(SendMail)),
                new("Github","mdi-github",EC(ToGithub)),
                new("QQGroup","mdi-qqchat",EC(OpenQQGroup)),
            };
        }

        private async Task LoadSettings()
        {
            Language = await SettingsService.Get(SettingType.Language);
            UserName = await SettingsService.Get(SettingType.UserName);
            Sign = await SettingsService.Get(SettingType.Sign);
            int themeState = await SettingsService.Get(SettingType.ThemeState);
            ThemeState = (ThemeState)themeState;
        }

        private async Task LanguageChanged(string value)
        {
            Language = value;
            I18n.SetCulture(value);
            await SettingsService.Save(SettingType.Language, Language);
            await SetCount();
        }

        private async Task SetAvatar()
        {
            string avatar = await SettingsService.Get(SettingType.Avatar);
            Avatar = await LocalImageService.ToUrl(avatar);
        }

        private async Task SendMail()
        {
            try
            {
                if (SystemService.IsMailSupported())
                {
                    List<string> recipients = new() { mail };

                    await SystemService.SendEmail(recipients);
                }
                else
                {
                    await SystemService.SetClipboard(mail);
                    await AlertService.Success(I18n.T("Mine.MailCopy"));
                }
            }
            catch (Exception)
            {
                throw new Exception("SendMailError");
            }
        }

        private async Task ToGithub()
        {
            await SystemService.OpenBrowser(githubUrl);
        }

        private async Task ThemeStateChanged(ThemeState themeState)
        {
            ThemeState = themeState;
            ThemeService.SetThemeState(ThemeState);
            await SettingsService.Save(SettingType.ThemeState, (int)ThemeState);
        }

        private async Task OpenQQGroup()
        {
            try
            {
                bool flag = await SystemService.OpenQQGroup();
                if (flag)
                {
                    await SystemService.SetClipboard(qqGroup);
                    await AlertService.Success(I18n.T("Mine.QQGroupCopy"));
                }
            }
            catch (Exception)
            {
                throw new Exception("QQGroupError");
            }
            
        }
    }
}
