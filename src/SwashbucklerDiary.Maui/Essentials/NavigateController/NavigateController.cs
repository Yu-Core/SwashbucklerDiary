using Microsoft.AspNetCore.Components.Routing;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Extensions;
using System.Reflection;

namespace SwashbucklerDiary.Maui.Essentials
{
    public class NavigateController : Rcl.Essentials.NavigateController
    {
        public NavigateController(IAppLifecycle appLifecycle) : base(appLifecycle)
        {
        }

        protected override IEnumerable<Assembly> Assemblies => Routes.Assemblies;

        protected override Task HandleNavigateToStackBottomPath(LocationChangingContext context)
        {
            context.PreventNavigation();
            _appLifecycle.QuitApp();
            return Task.CompletedTask;
        }

        public override async Task BackPressed()
        {
            if (IsInitialized)
            {
                await _jSRuntime.HistoryBack();
            }
            else
            {
                _appLifecycle.QuitApp();
            }
        }
    }
}
