using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;
using SwashbucklerDiary.Extensions;
using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Components
{
    public class ImportantComponentBase : MyComponentBase, IDisposable
    {
        private string? Url;

        [Inject]
        protected IJSRuntime JS { get; set; } = default!;
        [Inject]
        protected IPlatformService PlatformService { get; set; } = default!;

        public void Dispose()
        {
            OnDispose();
            GC.SuppressFinalize(this);
        }

        protected bool IsCurrentPage => Url is null || Url.EqualsAbsolutePath(Navigation.Uri);

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

        protected Func<bool, Task> SettingChange(SettingType type)
        {
            return (bool value) => SettingsService.Save(type, value);
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
            Url = Navigation.Uri;
        }

        private void NavigationManagerOnLocationChanged(object? sender, LocationChangedEventArgs e)
        {
            if (Url.EqualsAbsolutePath(Navigation.Uri))
            {
                _ = OnResume();
            }
        }
    }
}
