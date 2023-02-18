using BlazorComponent.I18n;
using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.IServices;

namespace SwashbucklerDiary.Components
{
    public partial class MyMarkdown
    {
        private Dictionary<string, object> _options = new();

        [Inject]
        private I18n I18n { get; set; } = default!;
        [Inject]
        private ISettingsService SettingsService { get; set; } = default!;

        [Parameter]
        public string? Value { get; set; }
        [Parameter]
        public EventCallback<string> ValueChanged { get; set; }
        [Parameter]
        public string? Class { get; set; }

        protected async override Task OnInitializedAsync()
        {
            var Language = await SettingsService.GetLanguage();
            Language = Language.Replace("-", "_");
            _options = new()
            {
                {"mode","ir" },
                {"counter",new{enable = true,type = "type"}},
                {"minHeight",240},
                {"toolbar",new List<string>(){"headings", "bold", "italic", "strike", "line", "quote","list", "ordered-list" , "check", "indent","code","inline-code","link","emoji","edit-mode"}},
                {"placeholder",I18n!.T("Write.ContentPlace")! },
                {"cdn","npm/vditor/3.9.0" },
                {"lang",Language }
            };
            base.OnInitialized();
        }
    }
}
