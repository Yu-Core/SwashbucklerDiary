using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class GestureUnlock
    {
        private ElementReference elementReference;

        private DotNetObjectReference<object>? dotNetObjectReference;

        [Inject]
        private GestureUnlockJSModule JSModule { get; set; } = default!;

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
                await JSModule.Freeze(elementReference);
                await JSModule.SetStatus(elementReference, "error");
                await Task.Delay(600);
                await JSModule.UnFreeze(elementReference);
                await JSModule.Reset(elementReference);
            }
        }

        public void Reset()
        {
            Render();
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            MasaBlazor.OnThemeChange += HandleOnThemeChange;
        }

        private void HandleOnThemeChange(Theme theme)
        {
            Render();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                Render();
            }
        }

        protected override async ValueTask DisposeAsyncCore()
        {
            await base.DisposeAsyncCore();

            dotNetObjectReference?.Dispose();
            MasaBlazor.OnThemeChange -= HandleOnThemeChange;
        }

        private async void Render()
        {
            dotNetObjectReference ??= DotNetObjectReference.Create<object>(this);
            await JSModule.Init(dotNetObjectReference, elementReference, Options, MatrixFactoryOptions);
        }
    }
}