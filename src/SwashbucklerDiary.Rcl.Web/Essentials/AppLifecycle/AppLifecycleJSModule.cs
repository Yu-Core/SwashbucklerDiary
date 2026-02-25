using Masa.Blazor.JSInterop;
using Microsoft.JSInterop;

namespace SwashbucklerDiary.Rcl.Web.Essentials
{
    public class AppLifecycleJSModule : JSModule
    {
        private readonly DotNetObjectReference<object>? _dotNetObjectReference;
        public event Action? Resumed;
        public event Action? Stopped;

        public AppLifecycleJSModule(IJSRuntime js) : base(js, $"./_content/SwashbucklerDiary.Rcl.Web/js/appLifecycle.js")
        {
            _dotNetObjectReference = DotNetObjectReference.Create<object>(this);
        }

        [JSInvokable]
        public void OnResumed() => Resumed?.Invoke();

        [JSInvokable]
        public void OnStopped() => Stopped?.Invoke();

        public ValueTask Init()
        {
            return InvokeVoidAsync("init", _dotNetObjectReference);
        }

        public ValueTask Quit()
        {
            return InvokeVoidAsync("quit");
        }

        protected override async ValueTask DisposeAsyncCore()
        {
            await base.DisposeAsyncCore();

            _dotNetObjectReference?.Dispose();
        }
    }
}
