using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Services;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class SimpleMarkdownPreview
    {
        private Dictionary<string, object>? _options;

        [Inject]
        protected II18nService I18n { get; set; } = default!;

        [Inject]
        private IThemeService ThemeService { get; set; } = default!;


        [Parameter]
        public string? Value { get; set; }

        [Parameter]
        public string? Class { get; set; }

        [Parameter]
        public string? Style { get; set; }

        [Parameter]
        public bool Patch { get; set; } = true;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            SetOptions();
        }

        private void SetOptions()
        {
            string lang = I18n.Culture.Name.Replace("-", "_");
            string mode = ThemeService.RealTheme == Shared.Theme.Dark ? "dark" : "light";
            var theme = new Dictionary<string, object?>()
            {
                { "current", mode },
                { "path", $"_content/{StaticWebAssets.RclAssemblyName}/npm/vditor@3.11.1/dist/css/content-theme" }
            };
            var markdown = new Dictionary<string, object>()
            {
                { "toc", true },
                { "mark", true },
            };

            _options = new()
            {
                { "mode", mode },
                { "cdn", $"_content/{StaticWebAssets.RclAssemblyName}/npm/vditor@3.11.1" },
                { "lang", lang },
                { "theme", theme },
                { "markdown", markdown },
            };
        }
    }
}