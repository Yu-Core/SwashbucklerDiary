using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class MarkdownPreview
    {
        private Dictionary<string, object>? _options;

        [Inject]
        private IPreferences Preferences { get; set; } = default!;

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
            var themeKind = await ThemeService.GetThemeAsync();
            string mode = themeKind == Theme.Dark ? "dark" : "light";
            string lang = await Preferences.Get<string>(Setting.Language);
            lang = lang.Replace("-", "_");
            var theme = new Dictionary<string, object?>()
            {
                { "current", mode },
                { "path", $"_content/{StaticWebAssets.RclAssemblyName}/npm/vditor/3.9.6/dist/css/content-theme" }
            };

            _options = new()
            {
                { "mode", mode },
                { "cdn", $"_content/{StaticWebAssets.RclAssemblyName}/npm/vditor/3.9.6" },
                { "lang", lang },
                { "theme", theme },
                { "icon", "material" },
            };
        }
    }
}
