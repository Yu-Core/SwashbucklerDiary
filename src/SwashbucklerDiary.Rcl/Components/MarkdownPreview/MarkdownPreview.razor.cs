using Masa.Blazor.Core;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Services;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class MarkdownPreview
    {
        private bool autoPlay;

        private bool codeLineNumber;

        private bool imageLazy;

        private bool showPreviewImage;

        private string? previewImageSrc;

        private VditorMarkdownPreview vditorMarkdownPreview = default!;

        private Dictionary<string, object>? _options;

        private ElementReference moblieOutlineContainerElement;

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
        public EventCallback<bool?> MobileOutlineChanged { get; set; }

        [Parameter]
        public EventCallback OnAfter { get; set; }

        [JSInvokable]
        public async Task Copy()
        {
            await AlertService.Success(I18n.T("Copy successfully"));
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

        private string? InternalClass => new CssBuilder()
            .Add(Class)
            .Add("first-line-indent", FirstLineIndent)
            .Add("task-list-line-through", TaskListLineThrough)
            .ToString();

        private void ReadSettings()
        {
            autoPlay = SettingService.Get(s => s.AutoPlay);
            codeLineNumber = SettingService.Get(s => s.CodeLineNumber);
            imageLazy = SettingService.Get(s => s.ImageLazy);
        }

        private void SetOptions()
        {
            string lang = I18n.Culture.Name.Replace("-", "_");
            string mode = Dark ? "dark" : "light";
            var theme = new Dictionary<string, object?>()
            {
                { "current", mode },
                { "path", $"_content/{StaticWebAssets.RclAssemblyName}/npm/vditor/3.10.7/dist/css/content-theme" }
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
                { "cdn", $"_content/{StaticWebAssets.RclAssemblyName}/npm/vditor/3.10.7" },
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

        private async Task HandleOnAfter()
        {
            var dotNetObjectReference = DotNetObjectReference.Create<object>(this);
            await MarkdownPreviewJSModule.AfterMarkdown(dotNetObjectReference,vditorMarkdownPreview.Ref, autoPlay, moblieOutlineContainerElement);

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
    }
}
