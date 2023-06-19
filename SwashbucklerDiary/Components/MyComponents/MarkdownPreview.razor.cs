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
        private IJSObjectReference? module;

        [Inject]
        private II18nService I18n { get; set; } = default!;
        [Inject]
        private ISettingsService SettingsService { get; set; } = default!;
        [Inject]
        private IJSRuntime JS { get; set; } = default!;
        [Inject]
        private IThemeService ThemeService { get; set; } = default!;

        [Inject]
        private IAlertService AlertService { get; set; } = default!;

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

        [JSInvokable]
        public async Task CopySuccess()
        {
            await AlertService.Success(I18n.T("Share.CopySuccess"));
        }

        protected override async Task OnInitializedAsync()
        {
            module = await JS!.InvokeAsync<IJSObjectReference>("import", "./js/vditor-helper.js");
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
            await Copy();
        }

        private async Task SetOptions()
        {
            string mode = ThemeService.Dark ? "dark" : "light";
            string lang = await SettingsService.Get(SettingType.Language);
            lang = lang.Replace("-", "_");
            var theme = new Dictionary<string, object?>() 
            { 
                { "current", ThemeService.Dark ? "dark" : "light" },
                { "path", "npm/vditor/3.9.3/dist/css/content-theme" } 
            };
            var markdown = new Dictionary<string, object?>() 
            { 
                { "linkBase", "https://" } 
            };

            _options = new()
            {
                { "mode", mode },
                { "cdn", "npm/vditor/3.9.3" },
                { "lang", lang },
                { "theme", theme },
                { "markdown" ,markdown }
            };
        }

        private async Task Copy()
        {
            await Task.Delay(1000);
            //点击复制按钮提示复制成功
            var dotNetCallbackRef = DotNetObjectReference.Create(this);
            await module!.InvokeVoidAsync("Copy", new object[2] { dotNetCallbackRef, "CopySuccess" });
        }
    }
}
