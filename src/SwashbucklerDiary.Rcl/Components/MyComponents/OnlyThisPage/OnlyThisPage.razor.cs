using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class OnlyThisPage : IDisposable
    {
        private string? Url;

        [Inject]
        private NavigationManager Navigation { get; set; } = default!;

        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            InitializedUrl();
            Navigation.LocationChanged += NavigationManagerOnLocationChanged;
        }

        public void Dispose()
        {
            Navigation.LocationChanged -= NavigationManagerOnLocationChanged;
            GC.SuppressFinalize(this);
        }

        private bool IsThisPage => Url is null || Url.EqualsAbsolutePath(Navigation.Uri);

        private void InitializedUrl()
        {
            Url = Navigation.Uri;
        }

        private void NavigationManagerOnLocationChanged(object? sender, LocationChangedEventArgs e)
        {
            InvokeAsync(StateHasChanged);
        }
    }
}
