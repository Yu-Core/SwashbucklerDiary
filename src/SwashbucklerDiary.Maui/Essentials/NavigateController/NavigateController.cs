using Microsoft.AspNetCore.Components.Routing;
using SwashbucklerDiary.Rcl.Layout;
using System.Reflection;

namespace SwashbucklerDiary.Maui.Essentials
{
    public class NavigateController : Rcl.Essentials.NavigateController
    {
        protected override Assembly[] Assemblies => [typeof(MainLayoutBase).Assembly, typeof(Routes).Assembly];

        protected override Task HandleNavigateToStackBottomPath(LocationChangingContext context)
        {
            context.PreventNavigation();
#if ANDROID
            NavigationButtonHandler.QuitApp();
#else
            Microsoft.Maui.Controls.Application.Current!.Quit();
#endif
            return Task.CompletedTask;
        }
    }
}
