using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SwashbucklerDiary.Components;
using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Models;
using System.Globalization;

namespace SwashbucklerDiary.Pages
{
    public partial class MinePage : PageComponentBase, IAsyncDisposable
    {
        private IJSObjectReference? module;
        private const string DefaultAvatar = "./logo/logo.svg";
        private int DiaryCount;
        private long WordCount;
        private int ActiveDayCount;
        private string? Language;
        private string? UserName;
        private string? Sign;
        private string? Avatar;
        private bool ShowLanguage;
        private bool ShowFeedback;
        private readonly static Dictionary<string, string> Languages = new()
        {
            {"中文","zh-CN" },
            {"English","en-US" }
        };
        private Dictionary<string, List<ViewListItem>> ViewLists = new();
        private List<ViewListItem> FeedbackTypes = new();


        [Inject]
        private IDiaryService DiaryService { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            LoadView();
            await SetAvatar();
            await SetCount();
            await LoadSettings();
        }

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
                        new("Mine.Backups","mdi-folder-sync-outline",()=>To("/backups")),
                        new("Mine.Import","mdi-import",()=>ToDo()),
                        new("Mine.Achievement","mdi-chart-line",()=>To("/achievement")),
                    }
                },
                {
                    "Mine.Settings",
                    new()
                    {
                        new("Mine.Settings","mdi-cog-outline",()=>To("/setting")),
                        new("Mine.Languages","mdi-web",()=>ShowLanguage=true),
                        new("Mine.Night","mdi-weather-night",()=>ToDo()),
                    }
                },
                {
                    "Mine.Other",
                    new()
                    {
                        new("Mine.Feedback","mdi-email-outline",()=>ShowFeedback=true),
                        new("Mine.About","mdi-information-outline",()=>To("/about")),
                    }
                }
            };
            FeedbackTypes = new()
            {
                new("Email","mdi-email-outline",async()=>await SendMail()),
                new("Github","mdi-github",async()=>await ToGithub()),
            };
        }

        private async Task LoadSettings()
        {
            Language = await SettingsService.GetLanguage();
            UserName = await SettingsService.Get<string?>(nameof(UserName), null);
            Sign = await SettingsService.Get<string?>(nameof(Sign), null);
        }

        private async Task LanguageChanged(string value)
        {
            ShowLanguage = false;
            Language = value;
            I18n.SetCulture(new CultureInfo(value));
            await SettingsService!.Save(nameof(Language), Language);
            await SetCount();
        }

        private async Task SetAvatar()
        {
            module = await JS!.InvokeAsync<IJSObjectReference>("import", "./js/getNativeImage.js");
            bool flag = await SettingsService!.ContainsKey(nameof(Avatar));
            if (!flag)
            {
                Avatar = DefaultAvatar;
            }
            else
            {
                var avatar = await SettingsService!.Get(nameof(Avatar), DefaultAvatar);
                using var imageStream = File.OpenRead(avatar);
                var dotnetImageStream = new DotNetStreamReference(imageStream);
                Avatar = await module!.InvokeAsync<string>("streamToUrl", new object[1] { dotnetImageStream });
            }
            
        }

        private async Task SendMail()
        {
            ShowFeedback = false;
            var mail = "yu-core@qq.com";
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
                    await PopupService.ToastSuccessAsync(I18n.T("Mine.MailCopy"));
                }
            }
            catch (Exception)
            {
                throw new Exception("SendMailError");
            }
        }

        private async Task ToGithub()
        {
            ShowFeedback = false;
            var url = "https://github.com/Yu-Core/SwashbucklerDiary";
            await SystemService.OpenBrowser(url);
        }

        async ValueTask IAsyncDisposable.DisposeAsync()
        {
            if (!string.IsNullOrEmpty(Avatar) && Avatar != DefaultAvatar)
            {
                await module!.InvokeVoidAsync("revokeUrl", new object[1] { Avatar });
            }

            if (module is not null)
            {
                await module.DisposeAsync();
            }
            GC.SuppressFinalize(this);
        }
    }
}
