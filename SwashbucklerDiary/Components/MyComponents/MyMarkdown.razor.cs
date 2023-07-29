using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Components
{
    public partial class MyMarkdown : ITempCustomSchemeAssist
    {
        private Dictionary<string, object> _options = new();
        private IJSObjectReference? module;
        private MMarkdown? MMarkdown;

        [Inject]
        private II18nService I18n { get; set; } = default!;
        [Inject]
        private ISettingsService SettingsService { get; set; } = default!;
        [Inject]
        public IJSRuntime JS { get; set; } = default!;
        [Inject]
        private IThemeService ThemeService { get; set; } = default!;
        [Inject]
        private IPlatformService PlatformService { get; set; } = default!;
        [Inject]
        private IAlertService AlertService { get; set; } = default!;
        [Inject]
        private IAppDataService AppDataService { get; set; } = default!;

        [Parameter]
        public string? Value { get; set; }
        [Parameter]
        public EventCallback<string> ValueChanged { get; set; }
        [Parameter]
        public string? Class { get; set; }
        [Parameter]
        public string? WrapClass { get; set; }

        protected async override Task OnInitializedAsync()
        {
            module = await JS!.InvokeAsync<IJSObjectReference>("import", "./js/vditor-helper.js");
            await SetOptions();
            await base.OnInitializedAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await AfterMarkdownRender();
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        private async Task SetOptions()
        {
            string lang = await SettingsService.Get(SettingType.Language);
            lang = lang.Replace("-", "_");
            string theme = ThemeService.Dark ? "dark" : "light";
            var previewTheme = new Dictionary<string, object?>()
            {
                { "current", ThemeService.Dark ? "dark" : "light" },
                { "path", "npm/vditor/3.9.3/dist/css/content-theme" }
            };
            var preview = new Dictionary<string, object?>()
            {
                { "theme", previewTheme },
            };
            var link = new Dictionary<string, object?>()
            {
                { "isOpen", false }
            };
            var btnImage = new Dictionary<string, object?>()
            {
                {"hotkey","⇧⌘I" },
                {"name","image" },
                {"tipPosition","n" },
                {"tip","添加图片" },
                {"className","" },
                {"icon","<svg><use xlink:href=\"#vditor-icon-image\"></use></svg>" },
            };

            //只有Windows能显示本机音视频，Android、ios、mac无法支持，所以砍去
            //var btnAudio = new Dictionary<string, object?>()
            //{
            //    {"hotkey","⇧⌘A" },
            //    {"name","audio" },
            //    {"tipPosition","n" },
            //    {"tip","添加音频" },
            //    {"className","" },
            //    {"icon","<svg><use xlink:href=\"#vditor-icon-audio\"></use></svg>" },
            //};
            //var btnVideo = new Dictionary<string, object?>()
            //{
            //    {"hotkey","⇧⌘V" },
            //    {"name","video" },
            //    {"tipPosition","n" },
            //    {"tip","添加视频" },
            //    {"className","" },
            //    {"icon","<svg><use xlink:href=\"#vditor-icon-video\"></use></svg>" },
            //};

            _options = new()
            {
                { "mode", "ir" },
                { "toolbar", new object[]{"headings", "bold", "italic", "strike", "line", "quote","list", "ordered-list" , "check", "code","inline-code","link","emoji",btnImage,/*btnAudio,btnVideo*/}},
                { "placeholder", I18n.T("Write.ContentPlace")! },
                { "cdn", "npm/vditor/3.9.3" },
                { "lang", lang },
                { "icon","material" },
                { "theme", theme },
                { "preview", preview },
                { "link", link }
            };
        }

        private async Task InternalValueChanged(string value)
        {
            if (ValueChanged.HasDelegate)
            {
                await ValueChanged.InvokeAsync(value);
                await this.CustomSchemeRender();
            }
        }

        private async Task AfterMarkdownRender()
        {
            await Task.Delay(1000);
            await PreventInputLoseFocus();
            await this.CustomSchemeRender();
        }

        private async Task PreventInputLoseFocus()
        {
            //点击工具栏不会丢失焦点
            await module!.InvokeVoidAsync("PreventInputLoseFocus", null);
        }

        private async Task HandleToolbarButtonClick(string btnName)
        {
            if (btnName == "image")
            {
                await AddImageAsync();
            }

            if(btnName == "audio")
            {
                await AddAudioAsync();
            }

            if(btnName == "video")
            {
                await AddVideoAsync();
            }
        }

        private async Task AddImageAsync()
        {
            string? path = await PlatformService.PickPhotoAsync();
            if (path == null)
            {
                return;
            }

            string url = await AppDataService.CreateAppDataImageFileAsync(path);
            string html = $"![]({url})";
            await InsertOrSetValueAsync(html);
        }

        private async Task AddAudioAsync()
        {
            string? path = await PlatformService.PickAudioAsync();
            if (path == null)
            {
                return;
            }

            string url = await AppDataService.CreateAppDataAudioFileAsync(path);
            string html = $"<audio src=\"{url}\" controls ></audio>";
            await InsertOrSetValueAsync(html);
        }

        private async Task AddVideoAsync()
        {
            string? path = await PlatformService.PickVideoAsync();
            if (path == null)
            {
                return;
            }

            string url = await AppDataService.CreateAppDataVideoFileAsync(path);
            string html = $"<video src=\"{url}\" controls autoplay ></video>";
            await InsertOrSetValueAsync(html);
        }

        private async Task InsertOrSetValueAsync(string value)
        {
            if (string.IsNullOrEmpty(Value))
            {
                await MMarkdown!.SetValueAsync(value);
                Value = value;
                if(ValueChanged.HasDelegate)
                {
                    await ValueChanged.InvokeAsync(value);
                }
                await this.CustomSchemeRender();
            }
            else
            {
                await MMarkdown!.InsertValueAsync(value);
            }
        }
    }
}
