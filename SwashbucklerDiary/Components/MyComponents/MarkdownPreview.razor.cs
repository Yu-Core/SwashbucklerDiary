using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Components
{
    public partial class MarkdownPreview
    {
        private ElementReference element;
        private string? _value;
        private Dictionary<string, object> _options = new();
        private bool AfterFirstRender;

        [Inject]
        private II18nService I18n { get; set; } = default!;
        [Inject]
        private ISettingsService SettingsService { get; set; } = default!;
        [Inject]
        private IJSRuntime JS { get; set; } = default!;
        [Inject]
        private IThemeService ThemeService { get; set; } = default!;

        [Parameter]
        public string? Value
        {
            get => _value;
            set => SetValue(value);
        }
        [Parameter]
        public string? Class { get; set; }
        [Parameter]
        public string? Style { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await SetOptions();
            await base.OnInitializedAsync();
        }

        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                AfterFirstRender = true;
                RenderingMarkdown(Value);
            }
            base.OnAfterRender(firstRender);
        }

        private void SetValue(string? value)
        {
            if (_value != value)
            {
                _value = value;
                if(AfterFirstRender)
                {
                    RenderingMarkdown(value);
                }
            }
        }

        private async void RenderingMarkdown(string? value)
        {
            if(element.Context == null)
            {
                return;
            }

            if(string.IsNullOrEmpty(value))
            {
                return;
            }

            await JS.InvokeVoidAsync("Vditor.preview", new object[3] { element, value, _options });
        }

        private async Task SetOptions()
        {
            string Language = await SettingsService.Get(SettingType.Language);
            Language = Language.Replace("-", "_");
            Dictionary<string, object?> themeOptions = new()
            {
                {"current",ThemeService.Dark?"dark":"light" },
                {"path","npm/vditor/3.9.0/dist/css/content-theme" }
            };
            _options = new()
            {
                {"mode",ThemeService.Dark?"dark":"light" },
                {"cdn","npm/vditor/3.9.0" },
                {"lang",Language },
                {"theme",themeOptions }
            };
        }
    }
}
