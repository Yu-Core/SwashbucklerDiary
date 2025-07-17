using SwashbucklerDiary.Rcl.Essentials;

namespace SwashbucklerDiary.Maui.Essentials
{
    public class AppLifecycle : Rcl.Essentials.AppLifecycle
    {
        private static IAppLifecycle? defaultImplementation;

        public static IAppLifecycle Default
            => defaultImplementation ??= new AppLifecycle();

        public override void QuitApp()
        {
#if ANDROID
            BackPressHelper.QuitApp();
#else
            Microsoft.Maui.Controls.Application.Current!.Quit();
#endif
        }


    }
}
