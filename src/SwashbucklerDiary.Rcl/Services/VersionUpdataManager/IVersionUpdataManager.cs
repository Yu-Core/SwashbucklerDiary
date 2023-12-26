namespace SwashbucklerDiary.Rcl.Services
{
    public interface IVersionUpdataManager
    {
        public event Func<Task>? AfterFirstEnter;

        public event Func<Task>? AfterUpdateVersion;

        Task FirstEnter();

        Task UpdateVersion();
    }
}
