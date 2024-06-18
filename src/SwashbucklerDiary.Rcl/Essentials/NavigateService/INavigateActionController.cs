namespace SwashbucklerDiary.Rcl.Essentials
{
    public interface INavigateActionController
    {
        bool Initialized { get; }

        void AddHistoryAction(Action action, bool preventNavigation = true);

        void AddHistoryAction(Func<Task> func, bool preventNavigation = true);

        void RemoveHistoryAction(Action action);

        void RemoveHistoryAction(Func<Task> func);
    }
}
