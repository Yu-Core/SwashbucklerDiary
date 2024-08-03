using SwashbucklerDiary.Rcl.Extensions;

namespace SwashbucklerDiary.WebAssembly.Essentials
{
    public class NavigateController : Rcl.Essentials.NavigateController
    {
        protected override async Task HandleNavigateToStackBottomPath()
        {
            var length = await _jSRuntime.EvaluateJavascript<int>("history.length");
            if (length > 2)
            {
                await _jSRuntime.HistoryGo(-2);
            }
        }
    }
}
