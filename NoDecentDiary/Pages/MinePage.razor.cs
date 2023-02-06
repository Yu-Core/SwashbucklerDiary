using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using NoDecentDiary.Components;
using NoDecentDiary.IServices;
using NoDecentDiary.Shared;
using System.Globalization;

namespace NoDecentDiary.Pages
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

        [Inject]
        private IDiaryService? DiaryService { get; set; }

        [CascadingParameter]
        public Error Error { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            await SetCount();
            await LoadSettings();
        }

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
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
                    await SetAvatar(avatar);
                }
                StateHasChanged();
            }
        }

        private async Task SetCount()
        {
            DiaryCount = await DiaryService!.CountAsync();
            var diaries = await DiaryService!.QueryAsync();
            var wordCount = 0;
            if (I18n!.T("Write.Word") == "1")
            {
                foreach (var item in diaries)
                {
                    wordCount += item.Content?.Split(' ').Length ?? 0;
                }
            }

            if (I18n!.T("Write.Character") == "1")
            {
                foreach (var item in diaries)
                {
                    wordCount += item.Content?.Length ?? 0;
                }
            }
            WordCount = wordCount;
            ActiveDayCount = diaries.Select(it => DateOnly.FromDateTime(it.CreateTime)).Distinct().Count();
        }

        private async Task LoadSettings()
        {
            Language = await SettingsService!.Get(nameof(Language), Languages.First().Value);
            UserName = await SettingsService!.Get<string?>(nameof(UserName), null);
            Sign = await SettingsService!.Get<string?>(nameof(Sign), null);
        }

        private void NavigateToSearch()
        {
            NavigateService.NavigateTo("/search");
        }

        private void NavigateToUser()
        {
            NavigateService.NavigateTo("/user");
        }

        private void NavigateToAbout()
        {
            NavigateService.NavigateTo("/about");
        }

        private async Task OnChangeLanguage(string value)
        {
            ShowLanguage = false;
            Language = value;
            I18n!.SetCulture(new CultureInfo(value));
            await SettingsService!.Save(nameof(Language), Language);
            await SetCount();
        }

        private async Task SetAvatar(string path)
        {
            using var imageStream = File.OpenRead(path);
            var dotnetImageStream = new DotNetStreamReference(imageStream);
            Avatar = await module!.InvokeAsync<string>("streamToUrl", new object[1] { dotnetImageStream });
        }

        private async Task SendMail()
        {
            ShowFeedback = false;
            //There are some problems in Windows. https://github.com/microsoft/microsoft-ui-xaml/issues/7300
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
                    await PopupService.ToastSuccessAsync(I18n!.T("Mine.MailCopy"));
                }
            }
            catch (Exception ex)
            {
                Error!.ProcessError(ex);
            }
            
        }

        private async Task ToGithub()
        {
            ShowFeedback = false;
            try
            {
                Uri uri = new("https://github.com/Yu-Core/NoDecentDiary");
                await Browser.Default.OpenAsync(uri, BrowserLaunchMode.External);
            }
            catch (Exception ex)
            {
                // An unexpected error occured. No browser may be installed on the device.
                Error.ProcessError(ex);
            }
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
