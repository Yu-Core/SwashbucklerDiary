using BlazorComponent.I18n;
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
        private I18n I18n { get; set; } = default!;
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
            string Language = await SettingsService.Get(SettingType.Language);
            Language = Language.Replace("-", "_");
            _options = new()
            {
                {"mode","ir" },
                //{"counter",new{enable = true,type = "type"}},
                //{"minHeight",240},
                {"toolbar",new List<string>(){"headings", "bold", "italic", "strike", "line", "quote","list", "ordered-list" , "check", "outdent", "indent","code","inline-code","link","emoji","edit-mode"}},
                {"placeholder",I18n.T("Write.ContentPlace")! },
                {"cdn","npm/vditor/3.9.0" },
                {"lang",Language },
                {"icon","material" },
                { "theme", ThemeService.Dark?"dark":"" }
            };
            base.OnInitialized();
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
            await Task.Delay(1000);
            await module!.InvokeVoidAsync("PreventInputLoseFocus", null);
        }
    }
}
