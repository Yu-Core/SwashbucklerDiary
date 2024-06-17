using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Models;

namespace SwashbucklerDiary.Rcl.Layout
{
    public abstract class MainLayoutNavBase : ComponentBase, IDisposable
    {
        [Inject]
        protected MasaBlazor MasaBlazor { get; set; } = default!;

        [CascadingParameter(Name = "Culture")]
        public string? Culture { get; set; }

        [Parameter]
        public List<NavigationButton> Items { get; set; } = [];

        [Parameter]
        public bool IsPermanentPath { get; set; }

        public void Dispose()
        {
            MasaBlazor.BreakpointChanged -= InvokeStateHasChanged;
            GC.SuppressFinalize(this);
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            MasaBlazor.BreakpointChanged += InvokeStateHasChanged;
        }

        private void InvokeStateHasChanged(object? sender, BreakpointChangedEventArgs e)
        {
            StateHasChanged();
        }
    }
}
