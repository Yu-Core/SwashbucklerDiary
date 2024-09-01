using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Extensions;

namespace SwashbucklerDiary.Rcl.Components
{
    public abstract class ImportantComponentBase : MyComponentBase
    {
        private string? previousPath;

        protected string? thisPagePath;

        [Inject]
        protected IStaticWebAssets StaticWebAssets { get; set; } = default!;

        [Inject]
        protected IPlatformIntegration PlatformIntegration { get; set; } = default!;

        [CascadingParameter(Name = "IsDark")]
        public bool Dark { get; set; }

        protected bool IsThisPage => thisPagePath is null || thisPagePath == NavigationManager.GetAbsolutePath();

        protected bool Light => !Dark;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            ReadSettings();
            previousPath = thisPagePath = NavigationManager.GetAbsolutePath();
            NavigationManager.LocationChanged += NavigationManagerOnLocationChanged;
        }

        protected override async ValueTask DisposeAsyncCore()
        {
            await base.DisposeAsyncCore();

            NavigationManager.LocationChanged -= NavigationManagerOnLocationChanged;
        }

        protected virtual async Task NavigateToBack()
        {
            await JS.HistoryBack();
        }

        protected virtual void ReadSettings()
        {
        }

        protected virtual async Task OnResume()
        {
            ReadSettings();
            await InvokeAsync(StateHasChanged);
        }

        protected virtual void NavigationManagerOnLocationChanged(object? sender, LocationChangedEventArgs e)
        {
            var currentPath = NavigationManager.GetAbsolutePath();
            if (previousPath == currentPath)
            {
                return;
            }

            previousPath = currentPath;
            if (thisPagePath == currentPath)
            {
                _ = OnResume();
            }
        }
    }
}
