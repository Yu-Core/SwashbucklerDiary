using Android.Widget;
using SwashbucklerDiary.Maui.BlazorWebView;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Services;
using Application = Android.App.Application;

namespace SwashbucklerDiary.Maui
{
    public static class BackPressHelper
    {
        private static byte backPressCounter;

        private static readonly Lazy<II18nService> _i18n = new(() => IPlatformApplication.Current!.Services.GetRequiredService<II18nService>());

        private static readonly Lazy<INavigateController> _navigateController = new(() => IPlatformApplication.Current!.Services.GetRequiredService<INavigateController>());

        private static II18nService I18n => _i18n.Value;

        private static INavigateController NavigateController => _navigateController.Value;

        public static void BackPressed()
        {
            if (NavigateController.IsInitialized && MauiBlazorWebViewHandler.WebView is not null)
            {
                MauiBlazorWebViewHandler.WebView.EvaluateJavascript(@"
				history.back();
			    ", null);
            }
            else
            {
                QuitApp();
            }
        }

        public static void QuitApp()
        {
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
        }
    }
}
