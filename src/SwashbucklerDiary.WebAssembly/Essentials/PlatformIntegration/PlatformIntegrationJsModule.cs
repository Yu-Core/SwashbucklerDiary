using Microsoft.JSInterop;

namespace SwashbucklerDiary.WebAssembly.Essentials
{
    public class PlatformIntegrationJSModule : Rcl.Web.Essentials.PlatformIntegrationJSModule
    {
        private readonly DotNetObjectReference<object>? _dotNetObjectReference;

        public PlatformIntegrationJSModule(IJSRuntime js) : base(js, "./js/platformIntegration.js")
        {
            _dotNetObjectReference = DotNetObjectReference.Create<object>(this);
        }

        [JSInvokable]
        public string RandomName()
        {
            return Guid.NewGuid().ToString("N");
        }

        public override ValueTask<string[]?> PickFilesAsync(
            string accept,
            string[] fileExtensions,
            bool multiple = true)
        {
            return InvokeAsync<string[]?>("chooseFiles", _dotNetObjectReference, accept, fileExtensions, multiple);
        }

        protected override async ValueTask DisposeAsyncCore()
        {
            await base.DisposeAsyncCore();

            _dotNetObjectReference?.Dispose();
        }
    }
}
