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
                {"hotkey","⌘⌥+I" },
                {"name","image" },
                {"tipPosition","n" },
                {"tip","添加图片" },
                {"className","" },
                {"icon","<svg><use xlink:href=\"#vditor-icon-image\"></use></svg>" },
            };

            _options = new()
            {
                { "mode", "ir" },
                { "toolbar", new object[]{"headings", "bold", "italic", "strike", "line", "quote","list", "ordered-list" , "check", "outdent", "indent","code","inline-code","link","emoji",btnImage}},
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
                await this.ImageRender();
            }
        }

        private async Task AfterMarkdownRender()
        {
            await Task.Delay(1000);
            await PreventInputLoseFocus();
            await this.ImageRender();
        }

        private async Task PreventInputLoseFocus()
        {
            //点击工具栏不会丢失焦点
            await module!.InvokeVoidAsync("PreventInputLoseFocus", null);
        }

        private async Task AddImage(string btnName)
        {
            if (btnName == "image")
            {
                string? path = await PlatformService.PickPhotoAsync();
                if (path == null)
                {
                    return;
                }
                string url = await AppDataService.CreateAppDataImageFileAsync(path);
                string html = $"![]({url})";
                if (path != null)
                {
                    await MMarkdown!.InsertValueAsync(html);
                }
            }
        }
    }
}
