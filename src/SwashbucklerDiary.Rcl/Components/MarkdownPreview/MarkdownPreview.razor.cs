using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Extensions;
using SwashbucklerDiary.Rcl.Services;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class MarkdownPreview
    {
        private bool autoPlay;

        private bool codeLineNumber;

        private bool imageLazy;

        private bool linkCard;

        private bool showPreviewImage;

        private string? previewImageSrc;

        private VditorMarkdownPreview vditorMarkdownPreview = default!;

        private Dictionary<string, object>? _options;

        private ElementReference? moblieOutlineElement;

        private DotNetObjectReference<object>? _dotNetObjectReference;

        private bool previousOutline;

        private MarkdownPreviewJSModule? jSModule;

        [Inject]
        private IThemeService ThemeService { get; set; } = default!;

        [Inject]
        private IMediaResourceManager MediaResourceManager { get; set; } = default!;

        [Inject]
        private IProxyService ProxyService { get; set; } = default!;

        [Parameter]
        public string? Value { get; set; }

        [Parameter]
        public string? Class { get; set; }

        [Parameter]
        public string? Style { get; set; }

        [Parameter]
        public bool FirstLineIndent { get; set; }

        [Parameter]
        public bool TaskListLineThrough { get; set; }

        [Parameter]
        public bool Outline { get; set; }

        [Parameter]
        public bool RightOutline { get; set; }

        [Parameter]
        public bool? MobileOutline { get; set; }

        [Parameter]
        public ElementReference? OutlineElement { get; set; }

        [Parameter]
        public EventCallback<bool?> MobileOutlineChanged { get; set; }

        [Parameter]
        public EventCallback OnAfter { get; set; }

        [Parameter]
        public EventCallback<MouseEventArgs> OnDblClick { get; set; }

        [JSInvokable]
        public async Task Copy()
        {
            await AlertService.SuccessAsync(I18n.T("Copy successfully"));
        }

        [JSInvokable]
        public async Task PreviewImage(string src)
        {
            previewImageSrc = src;
            showPreviewImage = true;
            await InvokeAsync(StateHasChanged);
        }

        [JSInvokable]
        public async Task CloseMobileOutline()
        {
            await InternalMobileOutlineChanged(false);
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

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);

            _dotNetObjectReference = DotNetObjectReference.Create<object>(this);
            jSModule = new(JS);
        }

        protected override async ValueTask DisposeAsyncCore()
        {
            await base.DisposeAsyncCore();

            _dotNetObjectReference?.Dispose();
            await jSModule.TryDisposeAsync();
        }

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();

            if (previousOutline != Outline)
            {
                previousOutline = Outline;
                await RenderOutline();
            }
        }

        private void ReadSettings()
        {
            autoPlay = SettingService.Get(s => s.AutoPlay);
            codeLineNumber = SettingService.Get(s => s.CodeLineNumber);
            imageLazy = SettingService.Get(s => s.ImageLazy);
            linkCard = SettingService.Get(s => s.LinkCard);
        }

        private void SetOptions()
        {
            var mode = ThemeService.RealTheme == Shared.Theme.Dark ? "dark" : "light";

            _options = new()
            {
                ["mode"] = mode,
                ["anchor"] = 2,
                ["cdn"] = $"_content/{StaticWebAssets.RclAssemblyName}/npm/vditor@3.11.2",
                ["lang"] = I18n.Culture.Name.Replace("-", "_"),
                ["theme"] = new Dictionary<string, object?>()
                {
                    { "current", mode },
                    { "path", $"_content/{StaticWebAssets.RclAssemblyName}/npm/vditor@3.11.2/dist/css/content-theme" }
                },
                ["icon"] = "material",
                ["hljs"] = new Dictionary<string, object>()
                {
                    ["lineNumber"] = codeLineNumber
                },
                ["markdown"] = new Dictionary<string, object?>()
                {
                    ["toc"] = true,
                    ["mark"] = true,
                    ["linkBase"] = MediaResourceManager.LinkBase,
                    ["sup"] = true,
                    ["sub"] = true,
                    ["imgPathAllowSpace"] = true
                },
                ["render"] = new Dictionary<string, object?>()
                {
                    ["media"] = new Dictionary<string, object?>()
                    {
                        ["enable"] = false,
                    }
                },
            };

            if (imageLazy)
            {
                _options.Add("lazyLoadImage", $"_content/{StaticWebAssets.RclAssemblyName}/img/loading.gif");
            }
        }

        private async Task HandleOnAfter()
        {
            if (IsDisposed || jSModule is null || _dotNetObjectReference is null)
            {
                return;
            }

            await jSModule.AfterMarkdown(_dotNetObjectReference, vditorMarkdownPreview.Ref, new()
            {
                OutlineElement = OutlineElement,
                MoblieOutlineElement = moblieOutlineElement,
                AutoPlay = autoPlay,
                LinkBase = MediaResourceManager.LinkBase,
                ProxyUrl = ProxyService.ProxyUrl,
                LinkCard = linkCard
            });

            if (OnAfter.HasDelegate)
            {
                await OnAfter.InvokeAsync();
            }
        }

        private async Task InternalMobileOutlineChanged(bool? value)
        {
            MobileOutline = value;
            if (MobileOutlineChanged.HasDelegate)
            {
                await MobileOutlineChanged.InvokeAsync(value);
            }
        }

        private async Task RenderOutline()
        {
            if (IsDisposed || !Outline || jSModule is null)
            {
                return;
            }

            await jSModule.RenderOutline(vditorMarkdownPreview.Ref, OutlineElement);
        }
    }
}
