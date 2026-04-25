using Masa.Blazor;
using Masa.Blazor.Core;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SwashbucklerDiary.Rcl.Extensions;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class SwiperTabItems : MItemGroup
    {
        private bool _isRendered;
        private DotNetObjectReference<object>? _dotNetObjectReference;
        private SwiperJsModule? jsModule;

        [Inject]
        private MasaBlazor MasaBlazor { get; set; } = default!;

        [Parameter]
        public Dictionary<string, object>? Options { get; set; }

        [Parameter]
        public bool Pagination { get; set; }

        [JSInvokable]
        public async Task UpdateValue(int value)
        {
            if (value == Value) return;
            await ToggleAsync(value);
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            MasaBlazor.RTLChanged += HandleRTLChanged;
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            Mandatory = true;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                _isRendered = true;
                await InitSwiperAsync();
            }
        }

        protected override async ValueTask DisposeAsyncCore()
        {
            await base.DisposeAsyncCore();

            MasaBlazor.RTLChanged -= HandleRTLChanged;
            _dotNetObjectReference?.Dispose();
            await jsModule.TryDisposeAsync();
        }

        protected override void OnInternalValuesChanged()
        {
            base.OnInternalValuesChanged();

            if (jsModule is null || Value is null) return;
            _ = jsModule.SlideToAsync(Ref, Value.ToInt32());
        }

        protected override IEnumerable<string> BuildComponentClass()
        {
            return base.BuildComponentClass().Concat(["swiper"]);
        }

        protected override IEnumerable<string?> BuildComponentStyle()
        {
            return base.BuildComponentStyle().Concat(
                StyleBuilder.Create()
                .Add("--swiper-pagination-color", "rgba(var(--m-theme-on-surface))")
                .AddIf("direction", "rtl", MasaBlazor.RTL)
                .GenerateCssStyles()
            );
        }

        private void HandleRTLChanged(object? sender, EventArgs e)
        {
            InvokeAsync(async () =>
            {
                StateHasChanged();
                await Task.Delay(16);
                await InitSwiperAsync();
            });
        }

        private async Task InitSwiperAsync()
        {
            if (!_isRendered)
            {
                return;
            }

            var options = new Dictionary<string, object>()
            {
                ["observer"] = true,
                ["observeParents"] = true,
                ["observeSlideChildren"] = true,
                //["simulateTouch"] = false,
                ["initialSlide"] = Value?.ToInt32() ?? 0,
                ["resistanceRatio"] = 0.7,
                ["speed"] = 250,
            };

            if (Pagination)
            {
                options["pagination"] = new Dictionary<string, object>()
                {
                    ["el"] = ".swiper-pagination",
                    //["dynamicBullets"] = true,
                };
            }

            if (Options is not null)
            {
                options = options.DeepMerge(Options);
            }

            jsModule ??= new(Js);
            _dotNetObjectReference ??= DotNetObjectReference.Create<object>(this);

            await jsModule.Init(_dotNetObjectReference, Ref, options);
        }

    }
}
