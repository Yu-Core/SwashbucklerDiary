using Microsoft.JSInterop;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.WebAssembly.Extensions;

namespace SwashbucklerDiary.WebAssembly.Essentials
{
    public class AppLifecycle : IAppLifecycle
    {
        public event Action<ActivationArguments>? OnActivated;

        public event Action? OnResumed;

        public event Action? OnStopped;

        private readonly Lazy<ValueTask<IJSInProcessObjectReference>> _module;

        public ActivationArguments? ActivationArguments { get; set; }

        public AppLifecycle(IJSRuntime jSRuntime)
        {
            _module = new(() => ((IJSInProcessRuntime)jSRuntime).ImportJsModule("js/appLifecycle.js"));
        }

        [JSInvokable]
        public void Resume() => OnResumed?.Invoke();

        [JSInvokable]
        public void Stop() => OnStopped?.Invoke();

        public async void QuitApp()
        {
            var module = await _module.Value;
            module.InvokeVoid("quit");
        }

        public async Task InitializedAsync()
        {
            var module = await _module.Value;
            var dotNetObject = DotNetObjectReference.Create(this);
            module.InvokeVoid("init", dotNetObject, nameof(Stop), nameof(Resume));
        }
    }
}
