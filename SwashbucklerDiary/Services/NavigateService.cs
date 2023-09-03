using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.IServices;

namespace SwashbucklerDiary.Services
{
    public class NavigateService : INavigateService
    {
        private string? CurrentUrl;
        private Dictionary<string, object?> CurrentCache = new();
        public event Action? Action;
        public event Func<Task>? BeforeNavBtn;
        public event Func<Task>? BeforeNavigate;

        public void Initialize(NavigationManager navigation)
        {
            Navigation = navigation;
        }

        public NavigationManager Navigation { get; set; } = default!;
        public List<string> HistoryUrl { get; set; } = new List<string>();
        private Dictionary<string, Dictionary<string, object?>?> HistoryCache { get; set; } = new();
        private string UriKey => Navigation.ToBaseRelativePath(Navigation.Uri).Split("?")[0];

        public async void NavigateTo(string url)
        {
            RemoveCurrentHistoryCache();

            if (BeforeNavigate is not null)
            {
                await BeforeNavigate.Invoke();
            }

            string oldUrl;
            if (CurrentUrl is null)
            {
                oldUrl = Navigation.ToBaseRelativePath(Navigation.Uri);
            }
            else
            {
                oldUrl = CurrentUrl;
                ClearCurrentUrl();
            }

            HistoryUrl.Add(oldUrl);
            AddCurrentHistoryCache();

            Navigation.NavigateTo(url);
        }
        public void NavigateToBack()
        {
            ClearCurrentUrl();
            ClearCurrentCache();
            RemoveCurrentHistoryCache();
            string url = string.Empty;
            if (HistoryUrl.Count > 0)
            {
                url = HistoryUrl.Last();
                var lastIndex = HistoryUrl.Count - 1;
                //RemoveAt比Remove性能好一些
                HistoryUrl.RemoveAt(lastIndex);
            }
            Navigation.NavigateTo(url);
        }

        public bool OnBackButtonPressed()
        {
            if (Action != null && Action?.GetInvocationList().Length > 0)
            {
                var delegates = Action.GetInvocationList();
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

        public async Task NavBtnClick(string url)
        {
            if (BeforeNavBtn is not null)
            {
                await BeforeNavBtn.Invoke();
            }

            ClearCurrentUrl();
            ClearCurrentCache();
            HistoryUrl.Clear();
            HistoryCache.Clear();

            Navigation.NavigateTo(url);
        }

        public void SetCurrentUrl(string? url)
        {
            CurrentUrl = url;
        }

        public void SetCurrentCache(string key, object? value)
        {
            CurrentCache.Add(key, value);
        }

        public object? GetCurrentCache(string key)
        {
            var url = Navigation.ToBaseRelativePath(Navigation.Uri);
            return GetCache(url, key);
        }

        private object? GetCache(string url, string key)
        {
            var cacheKey = url.Split("?")[0];
            var existCache = HistoryCache.TryGetValue(cacheKey, out Dictionary<string, object?>? cache);
            if (!existCache)
            {
                return null;
            }

            if (cache == null || cache.Count == 0)
            {
                return null;
            }

            var existCurrentCache = cache.TryGetValue(key, out object? value);
            if (!existCurrentCache)
            {
                return null;
            }

            return value;
        }

        private void ClearCurrentUrl()
        {
            CurrentUrl = null;
        }

        private void ClearCurrentCache()
        {
            CurrentCache = new();
        }

        private void RemoveCurrentHistoryCache()
        {
            HistoryCache.Remove(UriKey);
        }

        private void AddCurrentHistoryCache()
        {
            if (CurrentCache.Count > 0)
            {
                if (!HistoryCache.ContainsKey(UriKey))
                {
                    HistoryCache.Add(UriKey, CurrentCache);
                }

                ClearCurrentCache();
            }
        }
    }
}
