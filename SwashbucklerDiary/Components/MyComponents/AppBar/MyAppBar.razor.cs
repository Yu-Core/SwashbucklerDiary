using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;

namespace SwashbucklerDiary.Components
{
    public partial class MyAppBar : IDisposable
    {
        [Inject]
        private NavigationManager NavigationManager { get; set; } = default!;

        [Parameter]
        public string? Title { get; set; }
        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }
        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        public void Dispose()
        {
            NavigationManager.LocationChanged -= NavigationManagerOnLocationChanged;
            GC.SuppressFinalize(this);
        }

        protected override Task OnInitializedAsync()
        {
            NavigationManager.LocationChanged += NavigationManagerOnLocationChanged;
            return base.OnInitializedAsync();
        }

        private void NavigationManagerOnLocationChanged(object? sender, LocationChangedEventArgs e)
        {
            InvokeAsync(StateHasChanged);
        }

    }
}
