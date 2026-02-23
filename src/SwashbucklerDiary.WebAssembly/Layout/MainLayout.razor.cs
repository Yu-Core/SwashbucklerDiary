using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.WebAssembly.Essentials;

namespace SwashbucklerDiary.WebAssembly.Layout
{
    public partial class MainLayout : Rcl.Web.Layout.MainLayoutBase
    {
        [Inject]
        private IVersionTracking VersionTracking { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            await InternalOnInitializedAsync();
        }

        protected override async Task InitVersionUpdate()
        {
            await ((VersionTracking)VersionTracking).Track();
            await base.InitVersionUpdate();
        }

        protected override async Task InitConfigAsync()
        {
            await ((Services.SettingService)SettingService).InitializeAsync();
            await base.InitConfigAsync();
        }

        private async Task RefreshToSkipWaiting()
        {
            await JSRuntime.InvokeVoidAsync("swSkipWaiting");
        }
    }
}
