
using SwashbucklerDiary.Rcl.Extensions;

namespace SwashbucklerDiary.WebAssembly.Essentials
{
    public class NavigateService : Rcl.Essentials.NavigateService
    {
        protected override async Task HandleNavigateToStackBottomUri()
        {
            var length = await _jSRuntime.HistoryLength();
            if (length > 2)
            {
                await _jSRuntime.HistoryGo(-2);
            }
        }
    }
}
