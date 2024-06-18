using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Extensions;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Components
{
    public abstract class ImportantComponentBase : MyComponentBase, IDisposable
    {
        private string? previousPath;

        protected string? thisPagePath;

        [Inject]
        protected IJSRuntime JS { get; set; } = default!;

        [Inject]
        protected IStaticWebAssets StaticWebAssets { get; set; } = default!;

        [Inject]
        protected IPlatformIntegration PlatformIntegration { get; set; } = default!;

        [CascadingParameter(Name = "IsDark")]
        public bool Dark { get; set; }

        public void Dispose()
        {
            OnDispose();
            GC.SuppressFinalize(this);
        }

        protected bool IsThisPage => thisPagePath is null || thisPagePath == NavigationManager.GetAbsolutePath();

        protected bool Light => !Dark;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            InitializedPath();
            ReadSettings();
            NavigationManager.LocationChanged += NavigationManagerOnLocationChanged;
        }

        protected virtual async Task NavigateToBack()
        {
            await JS.HistoryBack();
        }

        protected Func<bool, Task> SettingChange(Setting type)
        {
            return (bool value) => SettingService.Set(type, value);
        }

        protected virtual void OnDispose()
        {
            NavigationManager.LocationChanged -= NavigationManagerOnLocationChanged;
        }

        protected virtual void ReadSettings()
        {
        }

        protected virtual async Task OnResume()
        {
            ReadSettings();
            await InvokeAsync(StateHasChanged);
        }

        private void InitializedPath()
        {
            previousPath = thisPagePath = NavigationManager.GetAbsolutePath();
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
