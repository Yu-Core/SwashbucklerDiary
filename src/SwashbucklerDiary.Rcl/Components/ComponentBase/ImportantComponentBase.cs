using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;
using SwashbucklerDiary.Rcl.Essentials;
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

        [Inject]
        protected NavigationManager Navigation { get; set; } = default!;

        [CascadingParameter(Name = "IsDark")]
        public bool Dark { get; set; }

        public void Dispose()
        {
            OnDispose();
            GC.SuppressFinalize(this);
        }

        protected bool IsCurrentPage => thisPageUrl is null || thisPageUrl.EqualsAbsolutePath(Navigation.Uri);

        protected bool Light => !Dark;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            InitializedUrl();
            Navigation.LocationChanged += NavigationManagerOnLocationChanged;
        }

        protected virtual Task NavigateToBack()
        {
            return NavigateService.PopAsync();
        }

        protected Func<bool, Task> SettingChange(Setting type)
        {
            return (bool value) => SettingService.Set(type, value);
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
