using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.IServices;

namespace SwashbucklerDiary.Services
{
    public class NavigateService : INavigateService
    {
        public NavigationManager Navigation { get; set; } = default!;
        private byte BackPressCounter = 0;

        public event Action? Action;

        public List<string> HistoryUrl { get; set; } = new List<string>();

        public void NavigateTo(string url)
        {
            var href = Navigation!.ToBaseRelativePath(Navigation.Uri);
            HistoryUrl.Add(href);
            Navigation.NavigateTo(url);
        }
        public void NavigateToBack()
        {
            string href = string.Empty;
            if (HistoryUrl.Count > 0)
            {
                href = HistoryUrl.Last();
            }
            Navigation.NavigateTo(href);
            if (HistoryUrl.Count > 0)
            {
                HistoryUrl.RemoveAt(HistoryUrl.Count - 1);
            }
        }

        public void OnBackButtonPressed()
        {
            if (Action != null && Action?.GetInvocationList().Length > 0)
            {
                var delegates = Action!.GetInvocationList();
                (delegates.Last() as Action)!.Invoke();
            }
            else
            {
                if (HistoryUrl.Count > 0)
                {
                    NavigateToBack();
                }
                else
                {
                    QuitApp();
                }
            }
        }

        private void QuitApp()
        {
            if (BackPressCounter == 1)
            {
#if ANDROID
                Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
#endif
            }
            else if (BackPressCounter == 0)
            {
                BackPressCounter++;
#if ANDROID
#pragma warning disable CS8602 // 解引用可能出现空引用。
                Android.Widget.Toast.MakeText(Android.App.Application.Context, "再按一次退出", Android.Widget.ToastLength.Long).Show();
                Task.Run(async () =>
                {
                    await Task.Delay(2000);
                    BackPressCounter = 0;
                });
#pragma warning restore CS8602 // 解引用可能出现空引用。
#endif
            }
        }
    }
}
