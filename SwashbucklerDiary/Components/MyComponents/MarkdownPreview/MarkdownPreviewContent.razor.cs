using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SwashbucklerDiary.IServices;

namespace SwashbucklerDiary.Components
{
    public partial class MarkdownPreviewContent : IAsyncDisposable
    {
        private ElementReference element;
        private IJSObjectReference module = default!;
        private bool ShowPreviewImage;
        private string? PreviewImageSrc;

        [Inject]
        private II18nService I18n { get; set; } = default!;
        [Inject]
        private ISettingsService SettingsService { get; set; } = default!;
        [Inject]
        public IJSRuntime JS { get; set; } = default!;
        [Inject]
        private IThemeService ThemeService { get; set; } = default!;

        [Inject]
        private IAlertService AlertService { get; set; } = default!;

        [Parameter]
        public string? Value { get; set; }
        [Parameter]
        public string? Class { get; set; }
        [Parameter]
        public string? Style { get; set; }
        [Parameter]
        public Dictionary<string, object>? Options { get; set; }

        [JSInvokable]
        public async Task After()
        {
            var dotNetCallbackRef = DotNetObjectReference.Create(this);

            //点击复制按钮提示复制成功
            await module.InvokeVoidAsync("copy", new object[2] { dotNetCallbackRef, "CopySuccess" });
            //修复点击链接的一些错误
            await module.InvokeVoidAsync("fixLink", new object[1] { element });
            //图片预览
            await module.InvokeVoidAsync("previewImage", new object[3] { dotNetCallbackRef, "PreviewImage", element });
        }

        [JSInvokable]
        public async Task CopySuccess()
        {
            await AlertService.Success(I18n.T("Share.CopySuccess"));
        }

        [JSInvokable]
        public async Task PreviewImage(string src)
        {
            PreviewImageSrc = src;
            ShowPreviewImage = true;
            await InvokeAsync(StateHasChanged);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                module = await JS.InvokeAsync<IJSObjectReference>("import", "./js/vditor-preview-helper.js");
                await RenderingMarkdown(Value);
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        private async Task RenderingMarkdown(string? value)
        {
            if (element.Context == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(value))
            {
                return;
            }

            var dotNetCallbackRef = DotNetObjectReference.Create(this);
            await module.InvokeVoidAsync("previewVditor", new object?[4] { dotNetCallbackRef, element, value, Options });
        }

        async ValueTask IAsyncDisposable.DisposeAsync()
        {
            if (module is not null)
            {
                await module.DisposeAsync();
            }

            GC.SuppressFinalize(this);
        }
    }
}
