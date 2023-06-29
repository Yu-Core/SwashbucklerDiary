using BlazorComponent;
using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Components
{
    public class MainNavigationCompontentBase : ComponentBase, IDisposable
    {
        [Inject]
        protected MasaBlazor MasaBlazor { get; set; } = default!;
        [Inject]
        protected II18nService I18n { get; set; } = default!;
        [Inject]
        protected NavigationManager Navigation { get; set; } = default!;

        [Parameter]
        public StringNumber Value { get; set; } = 0;
        [Parameter]
        public EventCallback<StringNumber> ValueChanged { get; set; }
        [Parameter]
        public List<NavigationButton> Items { get; set; } = new();

        public void Dispose()
        {
            MasaBlazor.BreakpointChanged -= InvokeStateHasChangedAsync;
            GC.SuppressFinalize(this);
        }

        protected override Task OnInitializedAsync()
        {
            MasaBlazor.BreakpointChanged += InvokeStateHasChangedAsync;
            return base.OnInitializedAsync();
        }

        private void InvokeStateHasChangedAsync(object? sender, BreakpointChangedEventArgs e)
        {
            StateHasChanged();
        }
    }
}
