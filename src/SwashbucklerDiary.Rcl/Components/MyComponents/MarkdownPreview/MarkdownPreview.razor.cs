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

        private bool showPreviewImage;

        private string? previewImageSrc;

        private VditorMarkdownPreview vditorMarkdownPreview = default!;

        private Dictionary<string, object>? _options;

        [Inject]
        protected II18nService I18n { get; set; } = default!;

        [Inject]
        private IAlertService AlertService { get; set; } = default!;

        [Inject]
        private ISettingService SettingsService { get; set; } = default!;

        [Inject]
        public MarkdownPreviewJSModule MarkdownPreviewJSModule { get; set; } = default!;

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
            imageLazy = SettingsService.Get<bool>(Setting.ImageLazy);
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
            var markdown = new Dictionary<string, object>()
            {
                { "mark", true }
            };

            _options = new()
            {
                { "mode", mode },
                { "cdn", $"_content/{StaticWebAssets.RclAssemblyName}/npm/vditor/3.10.4" },
                { "lang", lang },
                { "theme", theme },
                { "icon", "material" },
                { "markdown", markdown },
            };

            if (imageLazy)
            {
                _options.Add("lazyLoadImage", $"_content/{StaticWebAssets.RclAssemblyName}/img/loading.gif");
            }
        }
    }
}
