using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;
using SwashbucklerDiary.Rcl.Components;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Extensions;

namespace SwashbucklerDiary.Rcl.Web.Essentials
{
    [Route($"/{NavigateControllerHelper.StackBottomRelativePath}")]
    public partial class StackBottomPage : ImportantComponentBase
    {
        [Inject]
        private IJSRuntime JSRuntime { get; set; } = default!;

        protected override void NavigationManagerOnLocationChanged(object? sender, LocationChangedEventArgs e)
        {
            base.NavigationManagerOnLocationChanged(sender, e);

            InvokeAsync(StateHasChanged);
        }

        private async Task HandleOnCancle()
        {
            await JSRuntime.HistoryGo(1);
        }
    }
}