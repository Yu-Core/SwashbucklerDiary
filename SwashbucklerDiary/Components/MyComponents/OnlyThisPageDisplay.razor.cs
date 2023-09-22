using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using SwashbucklerDiary.Extensions;

namespace SwashbucklerDiary.Components
{
    public partial class OnlyThisPageDisplay : IDisposable
    {
        private string? Url;

        [Inject]
        private NavigationManager Navigation { get; set; } = default!;

        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        protected override void OnInitialized()
        {
            InitializedCurrentUrl();
            Navigation.LocationChanged += NavigationManagerOnLocationChanged;
            base.OnInitialized();
        }

        public void Dispose()
        {
            Navigation.LocationChanged -= NavigationManagerOnLocationChanged;
            GC.SuppressFinalize(this);
        }

        private bool IsCurrentPage => Url is null || Url.EqualsAbsolutePath(Navigation.Uri);

        private void InitializedCurrentUrl()
        {
            Url = Navigation.Uri;
        }

        private void NavigationManagerOnLocationChanged(object? sender, LocationChangedEventArgs e)
        {
            InvokeAsync(StateHasChanged);
        }
    }
}
