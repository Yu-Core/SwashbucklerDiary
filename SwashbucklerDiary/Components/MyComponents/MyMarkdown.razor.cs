using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Components
{
    public partial class MyMarkdown
    {
        private Dictionary<string, object> _options = new();
        private IJSObjectReference? module;

        [Inject]
        private II18nService I18n { get; set; } = default!;
        [Inject]
        private ISettingsService SettingsService { get; set; } = default!;
        [Inject]
        private IJSRuntime? JS { get; set; }
        [Inject]
        private IThemeService ThemeService { get; set; } = default!;

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
                await PreventInputLoseFocus();
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        private async Task PreventInputLoseFocus()
        {
            //点击工具栏不会丢失焦点
            await Task.Delay(1000);
            await module!.InvokeVoidAsync("PreventInputLoseFocus", null);
        }

        private async Task SetOptions()
        {
            string lang = await SettingsService.Get(SettingType.Language);
            lang = lang.Replace("-", "_");
            string theme = ThemeService.Dark ? "dark" : "light";
            Dictionary<string, object?> previewTheme = new()
            {
                {"current",ThemeService.Dark?"dark":"light" },
                {"path","npm/vditor/3.9.3/dist/css/content-theme" }
            };
            Dictionary<string, object?> preview = new()
            {
                {"theme",previewTheme },
            };
            Dictionary<string, object?> link = new()
            {
                {"isOpen",false }
            };

            _options = new()
            {
                {"mode","ir" },
                {"toolbar",new List<string>(){"headings", "bold", "italic", "strike", "line", "quote","list", "ordered-list" , "check", "outdent", "indent","code","inline-code","link","emoji"}},
                {"placeholder",I18n.T("Write.ContentPlace")! },
                {"cdn","npm/vditor/3.9.3" },
                {"lang",lang },
                {"icon","material" },
                {"theme",  theme},
                {"preview", preview},
                {"link",link }
            };
        }
    }
}
