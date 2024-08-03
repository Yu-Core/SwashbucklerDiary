namespace SwashbucklerDiary.Maui.Essentials
{
    public class NavigateController : Rcl.Essentials.NavigateController
    {
        protected override Task HandleNavigateToStackBottomPath()
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
