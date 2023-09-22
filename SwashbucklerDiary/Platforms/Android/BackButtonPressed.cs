using Android.OS;
using Android.Views;
using Android.Widget;
using SwashbucklerDiary.IServices;
using Application = Android.App.Application;

namespace SwashbucklerDiary.Platforms.Android
{
#pragma warning disable CA1416 // 验证平台兼容性
    public static class BackButtonPressed
    {
        private static II18nService I18n = default!;
        private static byte BackPressCounter;

        public static bool OnBackButtonPressed(KeyEvent e)
        {
            if (e.KeyCode == Keycode.Back)
            {
                if (e.Action == KeyEventActions.Down)
                {
                    var service = MauiApplication.Current.Services.GetRequiredService<INavigateService>();
                    bool flag = service!.OnBackButtonPressed();
                    if (!flag)
                    {
                        QuitApp();
                    }
                }

                return true;
            }

            return false;
        }

        public static void QuitApp()
        {
            I18n ??= MauiApplication.Current.Services.GetRequiredService<II18nService>();
            string text = I18n.T("Press again to exit");
            //I18未初始化时，禁用返回键退出
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            if (BackPressCounter == 1)
            {
                App.Current!.Quit();
            }
            else if (BackPressCounter == 0)
            {
                BackPressCounter++;
                Toast toast = Toast.MakeText(Application.Context, I18n.T("Press again to exit"), ToastLength.Long)!;
                //Toast的消失回调是Android11引入的，所以Android11以前的版本，定时3.5s后重置次数
                //ToastLength.Long是3.5s，ToastLength.Short是2s
                if (Build.VERSION.SdkInt >= BuildVersionCodes.R)
                {
                    toast.AddCallback(new MyToastCallBack());
                }
                else
                {
                    Task.Run(async () =>
                    {
                        await Task.Delay(3500);
                        BackPressCounter = 0;
                    });
                }

                toast.Show();
            }
        }

        public class MyToastCallBack : Toast.Callback
        {
            public override void OnToastHidden()
            {
                BackPressCounter = 0;
                base.OnToastHidden();
            }
        }
    }
}


