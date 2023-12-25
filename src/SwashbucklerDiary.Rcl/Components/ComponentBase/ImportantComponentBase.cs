using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Components
{
    public class ImportantComponentBase : MyComponentBase, IDisposable
    {
        private string? thisPageUrl;

        [Inject]
        protected IJSRuntime JS { get; set; } = default!;

        [Inject]
        protected IStaticWebAssets StaticWebAssets { get; set; } = default!;

        [Inject]
        protected IPlatformIntegration PlatformIntegration { get; set; } = default!;

        public void Dispose()
        {
            OnDispose();
            GC.SuppressFinalize(this);
        }

        protected bool IsCurrentPage => thisPageUrl is null || thisPageUrl.EqualsAbsolutePath(Navigation.Uri);

        protected override void OnInitialized()
        {
            InitializedUrl();
            Navigation.LocationChanged += NavigationManagerOnLocationChanged;
            base.OnInitialized();
        }

        protected virtual Task NavigateToBack()
        {
            return NavigateService.PopAsync();
        }

        protected Func<bool, Task> SettingChange(Setting type)
        {
            return (bool value) => Preferences.Set(type, value);
        }

        protected string? MSwitchTrackColor(bool value)
        {
            return value && Light ? "black" : null;
        }

        protected virtual void OnDispose()
        {
            Navigation.LocationChanged -= NavigationManagerOnLocationChanged;
        }

        protected virtual async Task OnResume()
        {
            await InvokeAsync(StateHasChanged);
        }

        private void InitializedUrl()
        {
            thisPageUrl = Navigation.Uri;
        }

        private void NavigationManagerOnLocationChanged(object? sender, LocationChangedEventArgs e)
        {
            if (thisPageUrl.EqualsAbsolutePath(Navigation.Uri))
            {
                _ = OnResume();
            }
        }
    }
}
