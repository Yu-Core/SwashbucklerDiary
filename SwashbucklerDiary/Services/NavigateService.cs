using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.IServices;

namespace SwashbucklerDiary.Services
{
    public class NavigateService : INavigateService
    {
        public NavigationManager Navigation { get; set; } = default!;
        public event Func<string>? CurrentUrl;
        public event Action? Action;
        public event Func<Task>? NavBtnAction;

        public void Initialize(NavigationManager navigation)
        {
            Navigation = navigation;
        }

        public List<string> HistoryUrl { get; set; } = new List<string>();

        public void NavigateTo(string url)
        {
            string oldUrl = string.Empty;
            if(CurrentUrl == null)
            {
                oldUrl = Navigation.ToBaseRelativePath(Navigation.Uri);
            }
            else
            {
                oldUrl = CurrentUrl.Invoke();
                CurrentUrl = null;
            }
            HistoryUrl.Add(oldUrl);
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

        public bool OnBackButtonPressed()
        {
            if (Action != null && Action?.GetInvocationList().Length > 0)
            {
                var delegates = Action!.GetInvocationList();
                (delegates.Last() as Action)!.Invoke();
                return true;
            }

            if (HistoryUrl.Count > 0)
            {
                NavigateToBack();
                return true;
            }

            return false;
        }

        public Task NavBtnClick()
        {
            if(NavBtnAction != null)
            {
                return NavBtnAction.Invoke();
            }
            return Task.CompletedTask;
        }
    }
}
