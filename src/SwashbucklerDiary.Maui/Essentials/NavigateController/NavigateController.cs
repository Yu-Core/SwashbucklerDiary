using Microsoft.AspNetCore.Components.Routing;
using System.Reflection;

namespace SwashbucklerDiary.Maui.Essentials
{
    public class NavigateController : Rcl.Essentials.NavigateController
    {
        protected override Assembly[] Assemblies { get; } = [Routes.Assembly, .. Routes.AdditionalAssemblies];

        protected override Task HandleNavigateToStackBottomPath(LocationChangingContext context)
        {
            context.PreventNavigation();
#if ANDROID
            BackPressHelper.QuitApp();
#else
            Microsoft.Maui.Controls.Application.Current!.Quit();
#endif
            return Task.CompletedTask;
        }
    }
}
