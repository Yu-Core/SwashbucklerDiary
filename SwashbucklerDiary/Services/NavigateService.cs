using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.IServices;

namespace SwashbucklerDiary.Services
{
    public class NavigateService : INavigateService
    {
        private string? _currentUrl;
        private Func<string?>? _funcCurrentUrl;
        private object? _currentCache;
        private Func<object?>? _funcCurrentCache;
        private string? CurrentUrl
        {
            get => _currentUrl ?? _funcCurrentUrl?.Invoke();
        }
        private object? CurrentCache
        {
            get => _currentCache ?? _funcCurrentCache?.Invoke();
        }

        public NavigationManager Navigation { get; set; } = default!;
        public event Action? Action;
        public event Func<Task>? NavBtnAction;

        public void Initialize(NavigationManager navigation)
        {
            Navigation = navigation;
        }

        public List<string> HistoryUrl { get; set; } = new List<string>();
        public Dictionary<string, object?> HistoryCache { get; set; } = new();

        public void NavigateTo(string url)
        {
            string oldUrl;
            if (CurrentUrl == null)
            {
                oldUrl = Navigation.ToBaseRelativePath(Navigation.Uri);
            }
            else
            {
                oldUrl = CurrentUrl;
                ClearCurrentUrl();
            }

            HistoryUrl.Add(oldUrl);
            if (CurrentCache != null)
            {
                var cacheKey = oldUrl.Split("?")[0];
                if (!HistoryCache.ContainsKey(cacheKey))
                {
                    HistoryCache.Add(cacheKey, CurrentCache);
                }

                ClearCurrentCache();
            }

            Navigation.NavigateTo(url);
        }
        public void NavigateToBack()
        {
            ClearCurrentUrl();
            ClearCurrentCache();
            string cacheKey = Navigation.ToBaseRelativePath(Navigation.Uri).Split("?")[0]; ;
            HistoryCache.Remove(cacheKey);
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

        public async Task NavBtnClick(string url)
        {
            if (NavBtnAction != null)
            {
                await NavBtnAction.Invoke();
            }
            ClearCurrentUrl();
            ClearCurrentCache();
            HistoryUrl.Clear();
            HistoryCache.Clear();

            Navigation.NavigateTo(url);
        }

        public void SetCurrentUrl(string? url)
        {
            _currentUrl = url;
        }

        public void SetCurrentUrl(Func<string>? func)
        {
            _funcCurrentUrl = func;
        }

        public void SetCurrentCache(object? value)
        {
            _currentCache = value;
        }

        public void SetCurrentCache(Func<object?>? func)
        {
            _funcCurrentCache = func;
        }

        public object? GetCurrentCache()
        {
            var url = Navigation.ToBaseRelativePath(Navigation.Uri);
            return GetCache(url);
        }

        private object? GetCache(string url)
        {
            var cacheKey = url.Split("?")[0];
            if (HistoryCache.ContainsKey(cacheKey))
            {
                return HistoryCache[cacheKey];
            }

            return null;
        }

        private void ClearCurrentUrl()
        {
            _currentUrl = null;
            _funcCurrentUrl = null;
        }

        private void ClearCurrentCache()
        {
            _currentCache = null;
            _funcCurrentCache = null;
        }
    }
}
