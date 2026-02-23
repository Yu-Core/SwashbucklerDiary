namespace SwashbucklerDiary.Rcl.Essentials
{
    public class NavigateController : INavigateController
    {
        protected IAppLifecycle _appLifecycle;

        public bool DisableNavigate { get; set; }

        public event Action<HistoryAction>? OnHistoryActionAdded;
        public event Action<Func<HistoryAction, bool>>? OnHistoryActionRemoved;
        public event Action<string>? OnPageCacheRemoved;
        public event Action? OnBackPressed;

        public NavigateController(IAppLifecycle appLifecycle)
        {
            _appLifecycle = appLifecycle;
        }

        public void AddHistoryAction(Action action, bool preventNavigation = true, bool isDialog = false)
            => AddHistoryAction(action, null, preventNavigation, isDialog);

        public void AddHistoryAction(Func<Task> func, bool preventNavigation = true, bool isDialog = false)
            => AddHistoryAction(null, func, preventNavigation, isDialog);

        public void RemoveHistoryAction(Action action)
            => RemoveHistoryAction(it => it.Action == action);

        public void RemoveHistoryAction(Func<Task> func)
            => RemoveHistoryAction(it => it.Func == func);

        public void RemovePageCache(string url)
        {
            OnPageCacheRemoved?.Invoke(url);
        }

        private void AddHistoryAction(Action? action = null, Func<Task>? func = null, bool preventNavigation = true, bool isDialog = false)
        {
            var historyAction = new HistoryAction()
            {
                PreventNavigation = preventNavigation,
                IsDialog = isDialog,
                Action = action,
                Func = func
            };
            OnHistoryActionAdded?.Invoke(historyAction);
        }

        private void RemoveHistoryAction(Func<HistoryAction, bool> func)
        {
            OnHistoryActionRemoved?.Invoke(func);
        }

        public void BackPressed()
        {
            if (OnBackPressed is null)
            {
                _appLifecycle.QuitApp();
            }
            else
            {
                OnBackPressed?.Invoke();
            }
        }
    }
}
