namespace SwashbucklerDiary.Rcl.Services
{
    public class AppLockService : IAppLockService
    {
        public event Func<AppLockEventArgs, Task>? ValidationSucceeded;
        public event Action? LockChanged;

        public void NotifyLockChanged()
        {
            LockChanged?.Invoke();
        }

        public async Task OnValidationSucceededAsync(AppLockEventArgs args)
        {
            var handler = ValidationSucceeded;
            if (handler == null) return;

            var delegates = handler.GetInvocationList()
                                .Cast<Func<AppLockEventArgs, Task>>();

            var tasks = delegates.Select(d => d(args));

            await Task.WhenAll(tasks);
        }
    }
}
