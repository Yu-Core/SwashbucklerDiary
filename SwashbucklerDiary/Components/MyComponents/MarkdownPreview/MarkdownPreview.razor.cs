using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Components
{
    public partial class MarkdownPreview
    {
        private Dictionary<string, object>? _options;

        [Inject]
        private ISettingsService SettingsService { get; set; } = default!;

        [Inject]
        private IThemeService ThemeService { get; set; } = default!;

        [Parameter]
        public string? Value { get; set; }

        [Parameter]
        public string? Class { get; set; }

        [Parameter]
        public string? Style { get; set; }

        private bool Show => !string.IsNullOrEmpty(Value) && _options is not null;

        protected override async Task OnInitializedAsync()
        {
            await SetOptions();
            await base.OnInitializedAsync();
        }

        private async Task SetOptions()
        {
            string mode = ThemeService.Dark ? "dark" : "light";
            string lang = await SettingsService.Get(SettingType.Language);
            lang = lang.Replace("-", "_");
            var theme = new Dictionary<string, object?>()
            {
                { "current", ThemeService.Dark ? "dark" : "light" },
                { "path", "npm/vditor/3.9.6/dist/css/content-theme" }
            };

            _options = new()
            {
                { "mode", mode },
                { "cdn", "npm/vditor/3.9.6" },
                { "lang", lang },
                { "theme", theme },
                { "icon", "material" },
            };
        }
    }
}
