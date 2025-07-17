using Microsoft.AspNetCore.Components.Routing;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Extensions;
using System.Reflection;

namespace SwashbucklerDiary.WebAssembly.Essentials
{
    public class NavigateController : Rcl.Essentials.NavigateController
    {
        public NavigateController(IAppLifecycle appLifecycle) : base(appLifecycle)
        {
        }

        protected override IEnumerable<Assembly> Assemblies => App.Assemblies;

        protected override async Task HandleNavigateToStackBottomPath(LocationChangingContext context)
        {
            // 首次返回
            if (firstEnter)
            {
                firstEnter = false;
                context.PreventNavigation();
                await _jSRuntime.HistoryBack();
            }
            // 二次返回
            else
            {
                firstEnter = true;
                historyPaths.RemoveAt(historyPaths.Count - 1);
            }
        }
    }
}
