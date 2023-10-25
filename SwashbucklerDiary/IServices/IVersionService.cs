namespace SwashbucklerDiary.IServices
{
    public interface IVersionService
    {
        public event Func<Task>? AfterFirstLauch;

        public event Func<Task>? AfterUpdateVersion;

        Task NotifyFirstLauchChanged();

        Task UpdateVersion();
    }
}
