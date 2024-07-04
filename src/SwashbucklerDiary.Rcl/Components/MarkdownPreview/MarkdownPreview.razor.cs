using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class MarkdownPreview
    {
        private bool imageLazy;

        private bool firstLineIndent;

        private bool codeLineNumber;

        private bool showPreviewImage;

        private string? previewImageSrc;

        private VditorMarkdownPreview vditorMarkdownPreview = default!;

        private Dictionary<string, object>? _options;

        [Inject]
        protected II18nService I18n { get; set; } = default!;

        [Inject]
        private IAlertService AlertService { get; set; } = default!;

        [Inject]
        private ISettingService SettingService { get; set; } = default!;

        [Inject]
        private MarkdownPreviewJSModule MarkdownPreviewJSModule { get; set; } = default!;

        [Inject]
        private NavigationManager NavigationManager { get; set; } = default!;

        [CascadingParameter(Name = "Culture")]
        public string? Culture { get; set; }

        [CascadingParameter(Name = "IsDark")]
        public bool Dark { get; set; }

        [Parameter]
        public string? Value { get; set; }

        [Parameter]
        public string? Class { get; set; }

        [Parameter]
        public string? Style { get; set; }

        [JSInvokable]
        public async Task Copy()
        {
            await AlertService.Success(I18n.T("Share.CopySuccess"));
        }

        [JSInvokable]
        public async Task PreviewImage(string src)
        {
            previewImageSrc = src;
            showPreviewImage = true;
            await InvokeAsync(StateHasChanged);
        }

        [JSInvokable]
        public void NavigateToReplace(string url)
        {
            NavigationManager.NavigateTo(url, replace: true);
        }

        public async Task RenderLazyLoadingImage()
        {
            if (vditorMarkdownPreview is null || !imageLazy)
            {
                return;
            }

            await vditorMarkdownPreview.RenderLazyLoadingImage();
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            ReadSettings();
            SetOptions();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                var dotNetObjectReference = DotNetObjectReference.Create<object>(this);
                await MarkdownPreviewJSModule.After(dotNetObjectReference, vditorMarkdownPreview.Ref);
            }
        }

        private void ReadSettings()
        {
            imageLazy = SettingService.Get<bool>(Setting.ImageLazy);
            firstLineIndent = SettingService.Get<bool>(Setting.FirstLineIndent);
            codeLineNumber = SettingService.Get<bool>(Setting.CodeLineNumber);
        }

        private void SetOptions()
        {
            string lang = I18n.Culture.Name.Replace("-", "_");
            string mode = Dark ? "dark" : "light";
            var theme = new Dictionary<string, object?>()
            {
                { "current", mode },
                { "path", $"_content/{StaticWebAssets.RclAssemblyName}/npm/vditor/3.10.4/dist/css/content-theme" }
            };
            var hljs = new Dictionary<string, object>()
            {
                { "lineNumber", codeLineNumber }
            };
            var markdown = new Dictionary<string, object>()
            {
                { "toc", true },
                { "mark", true },
            };

            _options = new()
            {
                { "mode", mode },
                { "cdn", $"_content/{StaticWebAssets.RclAssemblyName}/npm/vditor/3.10.4" },
                { "lang", lang },
                { "theme", theme },
                { "icon", "material" },
                { "hljs", hljs },
                { "markdown", markdown },
            };

            if (imageLazy)
            {
                _options.Add("lazyLoadImage", $"_content/{StaticWebAssets.RclAssemblyName}/img/loading.gif");
            }
        }
    }
}
