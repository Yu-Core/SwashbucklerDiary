
using SwashbucklerDiary.Rcl.Layout;
using System.Reflection;

namespace SwashbucklerDiary.Maui.Essentials
{
    public class NavigateService : Rcl.Essentials.NavigateService
    {
        protected override Assembly[] Assemblies => [typeof(MainLayoutBase).Assembly, typeof(Routes).Assembly];

        protected override Task HandleNavigateToStackBottomUri()
        {
#if ANDROID
            NavigationButtonHandler.QuitApp();
#else
            Microsoft.Maui.Controls.Application.Current!.Quit();
#endif
            return Task.CompletedTask;
        }
    }
}
