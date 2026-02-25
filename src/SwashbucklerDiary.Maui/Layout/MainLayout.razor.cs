using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Extensions;

namespace SwashbucklerDiary.Maui.Layout
{
    public partial class MainLayout : Rcl.Hybird.Layout.MainLayoutBase
    {
#if IOS || MACCATALYST
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                // IOS || MACCATALYST OpenUrl runs after BlazorWebView Initializ,so can only check ActivationArguments here
                var route = NavigationManager.GetRoute();
                if (route == "/welcome" || route == "/appLock")
                {
                    return;
                }

                var args = AppLifecycle.ActivationArguments;
                AppLifecycle.ActivationArguments = null;
                if (args is not null && args.Kind != AppActivationKind.Launch)
                {
                    AppLifecycle.NotifyActivated(args);
                }
            }
        }
#endif

    }
}
