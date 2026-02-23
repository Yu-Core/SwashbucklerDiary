namespace SwashbucklerDiary.Rcl.Services
{
    public class AppLockService : IAppLockService
    {
        public event Func<Task>? ValidationSucceeded;
        public event Action? LockChanged;

        public void OnLockChanged()
        {
            LockChanged?.Invoke();
        }

        public async Task OnValidationSucceededAsync()
        {
            var handler = ValidationSucceeded;
            if (handler == null) return;

            var delegates = handler.GetInvocationList()
                                .Cast<Func<Task>>();

            var tasks = delegates.Select(d => d());

            await Task.WhenAll(tasks);
        }
    }
}
