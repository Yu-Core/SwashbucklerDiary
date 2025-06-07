using Masa.Blazor;
using Masa.Blazor.Core;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using SwashbucklerDiary.Rcl.Services;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class MyFloatButton : IDisposable
    {
        private MButton? mButton;

        [Inject]
        private BreakpointService BreakpointService { get; set; } = default!;

        [Inject]
        private MasaBlazor MasaBlazor { get; set; } = default!;

        [Parameter(CaptureUnmatchedValues = true)]
        public virtual IDictionary<string, object> Attributes { get; set; } = new Dictionary<string, object>();

        [Parameter]
        public string? Class { get; set; }

        [Parameter]
        public string? Icon { get; set; }

        [Parameter]
        public bool Show { get; set; } = true;

        [Parameter]
        public StringNumber Elevation { get; set; } = 2;

        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

        public ElementReference? Ref => mButton?.Ref;

        public void Dispose()
        {
            BreakpointService.BreakpointChanged -= HandleBreakpointChange;
            GC.SuppressFinalize(this);
        }

        protected string InternalClass => new CssBuilder()
            .Add(Class)
            .Add("surface-container")
            .ToString();

        protected virtual StringNumber? Size { get; }

        protected bool Desktop => BreakpointService.Breakpoint.MdAndUp;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            BreakpointService.BreakpointChanged += HandleBreakpointChange;
        }

        private void HandleBreakpointChange(object? sender, MyBreakpointChangedEventArgs e)
        {
            if (!e.MdAndUpChanged)
            {
                return;
            }

            InvokeAsync(StateHasChanged);
        }
    }
}
