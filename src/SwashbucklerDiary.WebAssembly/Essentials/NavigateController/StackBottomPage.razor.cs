using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Extensions;
using SwashbucklerDiary.Rcl.Services;

namespace SwashbucklerDiary.WebAssembly.Essentials
{
    [Route($"/{NavigateControllerHelper.StackBottomRelativePath}")]
    public partial class StackBottomPage
    {
        [Inject]
        private II18nService I18n { get; set; } = default!;

        [Inject]
        private IJSRuntime JSRuntime { get; set; } = default!;

        [CascadingParameter(Name = "Culture")]
        public string? Culture { get; set; }

        private async Task HandleOnCancle()
        {
            await JSRuntime.HistoryGo(1);
        }
    }
}