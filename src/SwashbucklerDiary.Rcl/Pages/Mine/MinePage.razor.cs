using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using SwashbucklerDiary.Rcl.Components;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Extensions;
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

        private string privacyModeEntrancePassword = string.Empty;

        private string? privacyModeEntrancePasswordSalt;

        private bool showLanguage;

        private bool showTheme;

        private bool showFeedback;

        private bool showPreviewImage;

        private bool privacyMode;

        private bool hidePrivacyModeEntrance;

        private bool showprivacyModeEntrancePasswordDialog;

        private int diaryCount;

        private long wordCount;

        private int activeDayCount;

        private int imageCount;

        private int audioCount;

        private int videoCount;

        private MediaResourcePath? avatarResourceInfo;

        private ScrollContainer scrollContainer = default!;

        private static readonly Dictionary<string, Theme> themeItems = new()
        {
            { "Follow system", Theme.System },
            { "Light", Theme.Light },
            { "Dark", Theme.Dark },
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

        [Inject]
        protected IResourceService ResourceService { get; set; } = default!;

        [Inject]
        protected IGlobalConfiguration GlobalConfiguration { get; set; } = default!;

        [Inject]
        private IMediaResourceManager MediaResourceManager { get; set; } = default!;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            LoadView();
            I18n.CultureChanged += HandleCultureChanged;
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

        protected override async ValueTask DisposeAsyncCore()
        {
            await base.DisposeAsyncCore();

            I18n.CultureChanged -= HandleCultureChanged;
        }

        protected override void ReadSettings()
        {
            base.ReadSettings();

            language = SettingService.Get(s => s.Language);
            userName = SettingService.Get(s => s.NickName, null);
            sign = SettingService.Get(s => s.Sign, null);
            theme = (Theme)SettingService.Get(s => s.Theme);
            string? avatar = SettingService.Get(s => s.Avatar);
            avatarResourceInfo = MediaResourceManager.ToMediaResourcePath(NavigationManager, avatar);
            privacyMode = SettingService.GetTemp(s => s.PrivacyMode);
            hidePrivacyModeEntrance = SettingService.Get(s => s.HidePrivacyModeEntrance);
            privacyModeEntrancePassword = SettingService.Get(s => s.PrivacyModeEntrancePassword);
            privacyModeEntrancePasswordSalt = SettingService.Get(s => s.PrivacyModeEntrancePasswordSalt);
        }

        private void LoadView()
        {
            ViewLists = new()
            {
                {
                    "Data",
                    new()
                    {
                        new(this, "Backup","mdi:mdi-folder-sync-outline",() => To("backups")),
                        new(this, "Export","mdi:mdi-export",() => To("export")),
                        new(this, "Privacy mode","mdi:mdi-hexagon-slice-3",TryToPrivacyMode,()=>!hidePrivacyModeEntrance || privacyMode),
                    }
                },
                {
                    "Setting",
                    new()
                    {
                        new(this,"Setting","mdi:mdi-cog-outline",() => To("setting")),
                        new(this,"Language","language",() => showLanguage = true),
                        new(this,"Night mode","dark_mode",() => showTheme = true),
                    }
                },
                {
                    "Feature",
                    new()
                    {
                        new(this, "Achievement","trophy",() => To("achievement")),
                        new(this, "Location","location_on",() => To("locationSetting")),
                    }
                },
                {
                    "Other",
                    new()
                    {
                        new(this,"Fix Tool","build",() => To("fixTool")),
                        new(this,"Contact us","mail",() => showFeedback = true),
                        new(this,"About","info",() => To("about")),
                    }
                }
            };
            FeedbackTypes =
            [
                new(this, "Email","mail",SendMail),
                new(this, "Github","mdi:mdi-github",ToGithub),
                new(this, "QQ Group","mdi:mdi-qqchat",OpenQQGroup),
            ];
        }

        private async Task LoadViewAsync()
        {
            FeedbackTypeDatas = await StaticWebAssets.ReadJsonAsync<Dictionary<string, string>>("json/feedback-type/feedback-type.json");
        }

        private async Task LanguageChanged(string value)
        {
            I18n.SetCulture(new(value));
            await SettingService.SetAsync(s => s.Language, value);
        }

        private async Task SendMail()
        {
            var mail = FeedbackTypeDatas["Email"];
            try
            {
                bool isSuccess = await PlatformIntegration.SendEmail(I18n.T("Swashbuckler diary feedback"), null, [mail]);
                if (isSuccess)
                {
                    return;
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e, "SendMailFail");
            }

            await PlatformIntegration.SetClipboardAsync(mail);
            await AlertService.SuccessAsync(I18n.T("The email address has been copied to the clipboard."));
        }

        private async Task ToGithub()
        {
            var githubUrl = FeedbackTypeDatas["Github"];
            await PlatformIntegration.OpenBrowser(githubUrl);
        }

        private async Task ThemeChanged(Theme value)
        {
            ThemeService.SetTheme(value);
            await SettingService.SetAsync(s => s.Theme, (int)value);
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

            var qqGroup = FeedbackTypeDatas["QQ Group"];
            await PlatformIntegration.SetClipboardAsync(qqGroup);
            await AlertService.SuccessAsync(I18n.T("The QQ group number has been copied to the clipboard."));
        }

        private void Search(string? value)
        {
            string? queryParameters = string.IsNullOrWhiteSpace(value) ? null : $"?query={value}";
            To($"searchAppFeatures{queryParameters}");
        }

        private async Task UpdateStatisticalDataAsync()
        {
            List<Task> tasks = [];
            tasks.Add(UpdateDiaryStatisticalDataAsync());
            tasks.Add(UpdateResourceStatisticalDataAsync());
            await Task.WhenAll(tasks);
        }

        private async Task UpdateDiaryStatisticalDataAsync()
        {
            var diries = await DiaryService.QueryDiariesAsync();
            diaryCount = diries.Count;
            wordCount = diries.GetWordCount();
            activeDayCount = diries.Select(it => DateOnly.FromDateTime(it.CreateTime)).Distinct().Count();
        }

        private async Task UpdateResourceStatisticalDataAsync()
        {
            var resources = await ResourceService.QueryAsync();
            imageCount = resources.Count(it => it.ResourceType == MediaResource.Image);
            audioCount = resources.Count(it => it.ResourceType == MediaResource.Audio);
            videoCount = resources.Count(it => it.ResourceType == MediaResource.Video);
        }

        private async void HandleCultureChanged(object? sender, EventArgs args)
        {
            var diries = await DiaryService.QueryDiariesAsync();
            wordCount = diries.GetWordCount();
            await InvokeAsync(StateHasChanged);
        }

        private void TryToPrivacyMode()
        {
            if (privacyMode || string.IsNullOrEmpty(privacyModeEntrancePassword))
            {
                ToPrivacyMode();
            }
            else
            {
                showprivacyModeEntrancePasswordDialog = true;
            }
        }

        private void ToPrivacyMode()
        {
            SettingService.SetTemp(s => s.AllowEnterPrivacyMode, true);
            To("privacyMode");
        }

        private async Task InputPassword(string value)
        {
            showprivacyModeEntrancePasswordDialog = false;
            if (string.IsNullOrWhiteSpace(value))
            {
                return;
            }

            bool isSuccess;

            string? salt = privacyModeEntrancePasswordSalt;
            if (string.IsNullOrEmpty(salt))
            {
                // Compatible with old versions
                salt = nameof(Setting.PrivacyModeEntrancePassword);
                isSuccess = privacyModeEntrancePassword == (value + salt).MD5Encrytp32();
            }
            else
            {
                isSuccess = PasswordHasher.VerifyPassword(value, privacyModeEntrancePassword, salt);
            }

            if (!isSuccess)
            {
                await AlertService.ErrorAsync(I18n.T("Password error"));
                return;
            }

            ToPrivacyMode();
        }
    }
}
