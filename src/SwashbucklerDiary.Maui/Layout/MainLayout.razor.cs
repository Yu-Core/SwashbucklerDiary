using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Extensions;

namespace SwashbucklerDiary.Maui.Layout
{
    public partial class MainLayout : Rcl.Hybird.Layout.MainLayoutBase
    {
        protected override void OnInitialized()
        {
            base.OnInitialized();

#if IOS || MACCATALYST
            // IOS || MACCATALYST OpenUrl runs after BlazorWebView Initializ,so can only check ActivationArguments here
            var relativePath = NavigationManager.GetBaseRelativePath().ToLowerInvariant();
            if (relativePath == "welcome" || relativePath == "applock")
            {
                return;
            }

            var args = AppLifecycle.ActivationArguments;
            AppLifecycle.ActivationArguments = null;
            if (args is not null && args.Kind != AppActivationKind.Launch)
            {
                AppLifecycle.Activate(args);
            }
#endif
        }
    }
}
