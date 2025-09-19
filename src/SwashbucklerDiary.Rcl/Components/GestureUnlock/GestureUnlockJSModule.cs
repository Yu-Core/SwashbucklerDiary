using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace SwashbucklerDiary.Rcl.Components
{
    public class GestureUnlockJSModule : CustomJSModule
    {
        public GestureUnlockJSModule(IJSRuntime js) : base(js, "Components/GestureUnlock/GestureUnlock.razor.js")
        {
        }

        public async Task Init(DotNetObjectReference<object> dotNetRef,
            ElementReference container,
            Dictionary<string, object> options,
            Dictionary<string, object> matrixFactoryOptions)
        {
            await InvokeVoidAsync("init", dotNetRef, container, options, matrixFactoryOptions);
        }

        public async Task Reset(ElementReference container)
        {
            await InvokeVoidAsync("reset", container);
        }

        public async Task Freeze(ElementReference container)
        {
            await InvokeVoidAsync("freeze", container);
        }

        public async Task UnFreeze(ElementReference container)
        {
            await InvokeVoidAsync("unFreeze", container);
        }

        public async Task SetStatus(ElementReference container, string status)
        {
            await InvokeVoidAsync("setStatus", container, status);
        }
    }
}
