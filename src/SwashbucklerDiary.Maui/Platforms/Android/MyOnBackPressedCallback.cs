using Android.Widget;
using AndroidX.Activity;
using SwashbucklerDiary.Maui.BlazorWebView;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Services;
using Application = Android.App.Application;

namespace SwashbucklerDiary.Maui
{
    public class MyOnBackPressedCallback : OnBackPressedCallback
    {
        private byte backPressCounter;

        private readonly Lazy<II18nService> _i18n = new(() => IPlatformApplication.Current!.Services.GetRequiredService<II18nService>());

        private readonly Lazy<INavigateController> _navigateController = new(() => IPlatformApplication.Current!.Services.GetRequiredService<INavigateController>());

        private II18nService I18n => _i18n.Value;

        private INavigateController NavigateController => _navigateController.Value;

        public MyOnBackPressedCallback(bool enabled) : base(enabled)
        {
        }

        public override void HandleOnBackPressed()
        {
            if (NavigateController.IsInitialized && MauiBlazorWebViewHandler.WebView is not null)
            {
                MauiBlazorWebViewHandler.WebView.EvaluateJavascript(@"
				history.back();
			    ", null);
            }
            else
            {
                Quit();
            }
        }

        public void Quit()
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
