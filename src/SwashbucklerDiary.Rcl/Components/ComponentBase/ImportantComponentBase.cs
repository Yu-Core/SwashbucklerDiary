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
        protected string? thisPageUrl;

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

        protected bool IsThisPage => thisPageUrl is null || thisPageUrl.EqualsAbsolutePath(NavigationManager.Uri);

        protected bool Light => !Dark;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            InitializedUrl();
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

        private void InitializedUrl()
        {
            thisPageUrl = NavigationManager.Uri;
        }

        protected virtual void NavigationManagerOnLocationChanged(object? sender, LocationChangedEventArgs e)
        {
            if (thisPageUrl.EqualsAbsolutePath(NavigationManager.Uri))
            {
                _ = OnResume();
            }
        }
    }
}
