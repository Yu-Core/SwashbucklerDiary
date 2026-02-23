namespace SwashbucklerDiary.Rcl.Services
{
    public interface IAppLockService
    {
        event Func<Task>? ValidationSucceeded;
        event Action? LockChanged;
        Task OnValidationSucceededAsync();
        void OnLockChanged();
    }
}
