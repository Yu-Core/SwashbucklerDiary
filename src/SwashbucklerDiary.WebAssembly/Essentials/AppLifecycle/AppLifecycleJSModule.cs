using Masa.Blazor.JSInterop;
using Microsoft.JSInterop;

namespace SwashbucklerDiary.WebAssembly.Essentials
{
    public class AppLifecycleJSModule : JSModule
    {
        private readonly DotNetObjectReference<object>? _dotNetObjectReference;
        public event Action? OnResumed;
        public event Action? OnStopped;

        public AppLifecycleJSModule(IJSRuntime js) : base(js, "./js/appLifecycle.js")
        {
            _dotNetObjectReference = DotNetObjectReference.Create<object>(this);
        }

        [JSInvokable]
        public void Resume() => OnResumed?.Invoke();

        [JSInvokable]
        public void Stop() => OnStopped?.Invoke();

        public ValueTask<string?> Init()
        {
            return InvokeAsync<string?>("init", _dotNetObjectReference);
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
