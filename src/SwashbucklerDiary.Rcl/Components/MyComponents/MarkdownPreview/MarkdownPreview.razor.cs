using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Extensions;
using SwashbucklerDiary.Rcl.Services;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class MarkdownPreview : IAsyncDisposable
    {
        private bool showPreviewImage;

        private string? previewImageSrc;

        private IJSObjectReference module = default!;

        private VditorMarkdownPreview vditorMarkdownPreview = default!;

        private Dictionary<string, object>? _options;

        [Inject]
        protected II18nService I18n { get; set; } = default!;

        [Inject]
        private IAlertService AlertService { get; set; } = default!;

        [Inject]
        public IJSRuntime JS { get; set; } = default!;

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
        public async Task CopySuccess()
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

        public async ValueTask DisposeAsync()
        {
            if (module is not null)
            {
                await module.DisposeAsync();
            }

            GC.SuppressFinalize(this);
        }

        public async Task RenderLazyLoadingImage()
        {
            if (vditorMarkdownPreview is null)
            {
                return;
            }

            await vditorMarkdownPreview.RenderLazyLoadingImage();
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            SetOptions();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                module = await JS.ImportRclJsModule("js/markdown-preview-helper.js");
                var dotNetCallbackRef = DotNetObjectReference.Create(this);

                //点击复制按钮提示复制成功
                await module.InvokeVoidAsync("copy", [dotNetCallbackRef, nameof(CopySuccess), vditorMarkdownPreview.Ref]);
                //图片预览
                await module.InvokeVoidAsync("previewImage", [dotNetCallbackRef, nameof(PreviewImage), vditorMarkdownPreview.Ref]);
            }
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
                { "lazyLoadImage", $"_content/{StaticWebAssets.RclAssemblyName}/img/loading.gif" }
            };
        }
    }
}
