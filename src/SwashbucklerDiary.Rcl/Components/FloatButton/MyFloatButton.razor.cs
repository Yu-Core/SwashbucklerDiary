using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using SwashbucklerDiary.Rcl.Services;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class MyFloatButton : IDisposable
    {
        private MButton? mButton;

        [Inject]
        private MasaBlazorHelper MasaBlazorHelper { get; set; } = default!;

        [CascadingParameter(Name = "IsDark")]
        public bool Dark { get; set; }

        [Parameter]
        public string? Class { get; set; }

        [Parameter]
        public string? Icon { get; set; }

        [Parameter]
        public bool Show { get; set; } = true;

        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

        public ElementReference? Ref => mButton?.Ref;

        public void Dispose()
        {
            MasaBlazorHelper.BreakpointChanged -= HandleBreakpointChange;
            GC.SuppressFinalize(this);
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            MasaBlazorHelper.BreakpointChanged += HandleBreakpointChange;
        }

        private bool Desktop => MasaBlazorHelper.Breakpoint.MdAndUp;

        private string? Color => Dark ? null : "white";

        private string? InternalClass => $"elevation-2 {Class}";

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
