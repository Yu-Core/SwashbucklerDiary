namespace SwashbucklerDiary.Rcl.Essentials
{
    public interface INavigateController
    {
        bool DisableNavigate { get; set; }
        RouteMatcher RouteMatcher { get; }

        event Action<HistoryAction>? OnHistoryActionAdded;
        event Action<Func<HistoryAction, bool>>? OnHistoryActionRemoved;
        event Action<string>? OnPageCacheRemoved;
        event Action? OnBackPressed;

        void RemovePageCache(string url);

        void AddHistoryAction(Action action, bool preventNavigation = true, bool isDialog = false);

        void AddHistoryAction(Func<Task> func, bool preventNavigation = true, bool isDialog = false);

        void RemoveHistoryAction(Action action);

        void RemoveHistoryAction(Func<Task> func);

        void BackPressed();
    }
}
