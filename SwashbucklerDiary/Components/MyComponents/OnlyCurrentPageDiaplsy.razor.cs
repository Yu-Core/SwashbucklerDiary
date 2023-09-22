using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace SwashbucklerDiary.Components
{
    public partial class OnlyCurrentPageDiaplsy : IDisposable
    {
        private string? CurrentUrl;
        private bool IsCurrentPage = true;

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

        private void InitializedCurrentUrl()
        {
            CurrentUrl = new Uri(Navigation.Uri).AbsolutePath;
        }

        private void NavigationManagerOnLocationChanged(object? sender, LocationChangedEventArgs e)
        {
            IsCurrentPage = CurrentUrl == new Uri(Navigation.Uri).AbsolutePath;

            InvokeAsync(StateHasChanged);
        }
    }
}
