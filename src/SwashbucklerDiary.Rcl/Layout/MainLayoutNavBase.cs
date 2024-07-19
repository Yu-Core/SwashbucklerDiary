using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Models;
using SwashbucklerDiary.Rcl.Services;

namespace SwashbucklerDiary.Rcl.Layout
{
    public abstract class MainLayoutNavBase : ComponentBase, IDisposable
    {
        [Inject]
        protected MasaBlazorHelper MasaBlazorHelper { get; set; } = default!;

        [CascadingParameter(Name = "Culture")]
        public string? Culture { get; set; }

        [Parameter]
        public List<NavigationButton> Items { get; set; } = [];

        [Parameter]
        public bool IsPermanentPath { get; set; }

        public void Dispose()
        {
            MasaBlazorHelper.BreakpointChanged -= InvokeStateHasChanged;
            GC.SuppressFinalize(this);
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            MasaBlazorHelper.BreakpointChanged += InvokeStateHasChanged;
        }

        private void InvokeStateHasChanged(object? sender, MyBreakpointChangedEventArgs e)
        {
            if (!e.XsChanged)
            {
                return;
            }

            StateHasChanged();
        }
    }
}
