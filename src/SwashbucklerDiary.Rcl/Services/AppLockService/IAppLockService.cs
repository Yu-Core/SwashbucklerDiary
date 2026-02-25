namespace SwashbucklerDiary.Rcl.Services
{
    public interface IAppLockService
    {
        event Func<AppLockEventArgs, Task>? ValidationSucceeded;
        event Action? LockChanged;
        Task OnValidationSucceededAsync(AppLockEventArgs args);
        void NotifyLockChanged();
    }
}
