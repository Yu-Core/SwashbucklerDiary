using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SwashbucklerDiary.Rcl.Extensions;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class GestureUnlock
    {
        private ElementReference elementReference;

        private DotNetObjectReference<object>? dotNetObjectReference;

        private GestureUnlockJSModule? jSModule;

        [Inject]
        private MasaBlazor MasaBlazor { get; set; } = default!;

        [Parameter]
        public string? Class { get; set; }

        [Parameter]
        public string? Style { get; set; }

        [Parameter]
        public string? Title { get; set; }

        [Parameter]
        public Dictionary<string, object> Options { get; set; } = [];

        [Parameter]
        public Dictionary<string, object> MatrixFactoryOptions { get; set; } = [];

        [Parameter(CaptureUnmatchedValues = true)]
        public virtual IDictionary<string, object> Attributes { get; set; } = new Dictionary<string, object>();

        [Parameter]
        public EventCallback<LockFinishArguments> OnFinish { get; set; }

        [JSInvokable]
        public async Task OnEnd(string[] value)
        {
            if (jSModule is null) return;

            LockFinishArguments args = new()
            {
                Value = string.Join("", value)
            };

            if (OnFinish.HasDelegate)
            {
                await OnFinish.InvokeAsync(args);
            }

            if (args.IsFail)
            {
                await jSModule.Freeze(elementReference);
                await jSModule.SetStatus(elementReference, "error");
                await Task.Delay(600);
                await jSModule.UnFreeze(elementReference);
                await jSModule.Reset(elementReference);
            }
        }

        public async Task Reset()
        {
            await Render();
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            MasaBlazor.OnThemeChange += HandleOnThemeChange;
        }

        private async void HandleOnThemeChange(Theme theme)
        {
            await Render();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender && !IsDisposed)
            {
                dotNetObjectReference = DotNetObjectReference.Create<object>(this);
                jSModule = new(JS);
                await Render();
            }
        }

        protected override async ValueTask DisposeAsyncCore()
        {
            await base.DisposeAsyncCore();

            dotNetObjectReference?.Dispose();
            await jSModule.TryDisposeAsync();
            MasaBlazor.OnThemeChange -= HandleOnThemeChange;
        }

        private async Task Render()
        {
            if (jSModule is null || dotNetObjectReference is null)
            {
                return;
            }

            await jSModule.Init(dotNetObjectReference, elementReference, Options, MatrixFactoryOptions);
        }
    }
}