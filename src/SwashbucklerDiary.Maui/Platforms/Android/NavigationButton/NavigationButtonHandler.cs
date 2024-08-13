using Android.Views;
using Android.Widget;
using SwashbucklerDiary.Maui.BlazorWebView;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Services;
using Application = Android.App.Application;

namespace SwashbucklerDiary
{
#pragma warning disable CA1416 // 验证平台兼容性
    public static class NavigationButtonHandler
    {
        private static byte BackPressCounter;

        private static readonly Lazy<II18nService> _i18n = new(() => IPlatformApplication.Current!.Services.GetRequiredService<II18nService>());

        private static readonly Lazy<INavigateController> _navigateController = new(() => IPlatformApplication.Current!.Services.GetRequiredService<INavigateController>());

        private static II18nService I18n => _i18n.Value;

        private static INavigateController NavigateController => _navigateController.Value;

        public static bool OnBackButtonPressed(KeyEvent e)
        {
            if (e.KeyCode == Keycode.Back)
            {
                if (e.Action == KeyEventActions.Down)
                {
                    InternalOnBackButtonPressed();
                }

                return true;
            }

            return false;
        }

        private static void InternalOnBackButtonPressed()
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

            if (BackPressCounter == 1)
            {
                Microsoft.Maui.Controls.Application.Current!.Quit();
            }
            else if (BackPressCounter == 0)
            {
                BackPressCounter++;
                Toast toast = Toast.MakeText(Application.Context, I18n.T("Press again to exit"), ToastLength.Long)!;

                //定时3.5s后重置次数
                //ToastLength.Long是3.5s，ToastLength.Short是2s
                Task.Run(async () =>
                {
                    await Task.Delay(3500);
                    BackPressCounter = 0;
                });

                toast.Show();
            }
        }
    }
}
