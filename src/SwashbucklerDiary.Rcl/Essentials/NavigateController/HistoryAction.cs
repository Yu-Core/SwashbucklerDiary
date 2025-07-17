namespace SwashbucklerDiary.Rcl.Essentials
{
    public class HistoryAction
    {
        public PathString? Path { get; set; }

        public bool PreventNavigation { get; set; } = true;

        public Action? Action { get; set; }

        public Func<Task>? Func { get; set; }
    }
}
