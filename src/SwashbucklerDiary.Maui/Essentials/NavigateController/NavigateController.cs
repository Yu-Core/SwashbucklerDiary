using Microsoft.AspNetCore.Components.Routing;

namespace SwashbucklerDiary.Maui.Essentials
{
    public class NavigateController : Rcl.Essentials.NavigateController
    {
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
