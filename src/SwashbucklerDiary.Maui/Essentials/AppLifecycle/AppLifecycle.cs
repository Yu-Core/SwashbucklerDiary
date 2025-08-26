using SwashbucklerDiary.Rcl.Essentials;

#if ANDROID
using Android.Widget;
using Application = Android.App.Application;
using SwashbucklerDiary.Rcl.Services;
#endif

namespace SwashbucklerDiary.Maui.Essentials
{
    public class AppLifecycle : Rcl.Essentials.AppLifecycle
    {
#if ANDROID
        private byte backPressCounter;
        private readonly Lazy<II18nService> _i18n = new(() => IPlatformApplication.Current!.Services.GetRequiredService<II18nService>());
        private II18nService I18n => _i18n.Value;
#endif
        private static IAppLifecycle? defaultImplementation;

        public static IAppLifecycle Default
            => defaultImplementation ??= new AppLifecycle();

        public override void QuitApp()
        {
#if ANDROID
            string text = I18n.T("Press again to exit");

            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            if (backPressCounter == 1)
            {
                Microsoft.Maui.Controls.Application.Current!.Quit();
            }
            else if (backPressCounter == 0)
            {
                backPressCounter++;
                Toast toast = Toast.MakeText(Application.Context, text, ToastLength.Long)!;

                //定时3.5s后重置次数
                //ToastLength.Long是3.5s，ToastLength.Short是2s
                Task.Run(async () =>
                {
                    await Task.Delay(3500);
                    backPressCounter = 0;
                });

                toast.Show();
            }
#else
            Microsoft.Maui.Controls.Application.Current!.Quit();
#endif
        }


    }
}
