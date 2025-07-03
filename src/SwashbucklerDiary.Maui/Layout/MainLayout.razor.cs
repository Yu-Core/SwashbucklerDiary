using SwashbucklerDiary.Rcl.Essentials;

namespace SwashbucklerDiary.Maui.Layout
{
    public partial class MainLayout : Rcl.Hybird.Layout.MainLayoutBase
    {
        protected override void OnInitialized()
        {
            base.OnInitialized();

#if IOS || MACCATALYST
            // IOS || MACCATALYST OpenUrl runs after BlazorWebView Initializ,so can only check ActivationArguments here
            if (AppLifecycle.ActivationArguments != null)
            {
                if (AppLifecycle.ActivationArguments.Kind == AppActivationKind.Scheme)
                {
                    HandleScheme(AppLifecycle.ActivationArguments);
                }
                else
                {
                    QuickRecord();
                }

                AppLifecycle.ActivationArguments = null;
            }

            void QuickRecord()
            {
                var quickRecord = SettingService.Get(it => it.QuickRecord);
                if (quickRecord)
                {
                    NavigationManager.NavigateTo("write");
                }
            }
#endif
        }
    }
}
