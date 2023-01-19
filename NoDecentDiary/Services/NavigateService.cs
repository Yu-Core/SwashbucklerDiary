using Microsoft.AspNetCore.Components;
using NoDecentDiary.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoDecentDiary.Services
{
    public class NavigateService : INavigateService
    {
        public NavigationManager? Navigation { get; set; }
        private byte BackPressCounter;

        public event Action? Action;

        public List<string> HistoryHref { get; set; } = new List<string>();

        public void NavigateTo(string url)
        {
            var href = Navigation!.ToBaseRelativePath(Navigation.Uri);
            HistoryHref.Add(href);
            Navigation.NavigateTo(url);
        }
        public void NavigateToBack()
        {
            string href = string.Empty;
            if (HistoryHref.Count > 0)
            {
                href = HistoryHref.Last();
            }
            Navigation!.NavigateTo(href);
            if (HistoryHref.Count > 0)
            {
                HistoryHref.RemoveAt(HistoryHref.Count - 1);
            }
        }

        public void OnBackButtonPressed()
        {
            if (Action != null && Action?.GetInvocationList().Length> 0)
            {
                Action?.Invoke();
                foreach (var item in Action!.GetInvocationList())
                {
                    Action -= item as Action;
                }
            }
            else
            {
                if (HistoryHref.Count > 0)
                {
                    NavigateToBack();
                }
                else
                {
                    QuitApp();
                }
            }
        }

        public void UpdateLastHistoryHref(string href)
        {
            if (HistoryHref.Count > 0)
            {
                HistoryHref.RemoveAt(HistoryHref.Count - 1);
            }
            HistoryHref.Add(href);
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

#if ANDROID
                BackPressCounter++;
                Android.Widget.Toast.MakeText(Android.App.Application.Context, "再按一次退出", Android.Widget.ToastLength.Long).Show();
                Task.Run(async ()=>{
                    await Task.Delay(2000);
                    BackPressCounter = 0;
                });
#endif
            }
        }
    }
}
