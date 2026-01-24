using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Extensions;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

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
        public Dictionary<string, object>? Options { get; set; }

        [Parameter]
        public bool Optimize { get; set; } = true;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            SetOptions();
        }

        private void SetOptions()
        {
            string mode = ThemeService.RealTheme == Shared.Theme.Dark ? "dark" : "light";

            _options = new()
            {
                ["mode"] = mode,
                ["cdn"] = $"_content/{StaticWebAssets.RclAssemblyName}/npm/vditor@3.11.2",
                ["lang"] = I18n.Culture.ToVditorLang(),
                ["theme"] = new Dictionary<string, object?>()
                {
                    { "current", mode },
                    { "path", $"_content/{StaticWebAssets.RclAssemblyName}/npm/vditor@3.11.2/dist/css/content-theme" }
                },
                ["markdown"] = new Dictionary<string, object?>()
                {
                    ["toc"] = true,
                    ["mark"] = true,
                    ["sup"] = true,
                    ["sub"] = true,
                },
                ["render"] = new Dictionary<string, object?>()
                {
                    ["media"] = new Dictionary<string, object?>()
                    {
                        ["enable"] = false,
                    }
                },
            };

            if (Options is not null)
            {
                _options = _options.DeepMerge(Options);
            }
        }
    }
}