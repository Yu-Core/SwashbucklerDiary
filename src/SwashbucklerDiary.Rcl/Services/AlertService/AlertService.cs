using Masa.Blazor;
using Masa.Blazor.Popup.Components;
using Masa.Blazor.Presets;

namespace SwashbucklerDiary.Rcl.Services
{
    public class AlertService : IAlertService
    {
        private readonly IPopupService _popupService = default!;

        private readonly BreakpointService _breakpointService = default!;

        private readonly ISettingService _settingService = default!;

        public AlertService(IPopupService popupService,
            BreakpointService breakpointService,
            ISettingService settingService)
        {
            _popupService = popupService;
            _breakpointService = breakpointService;
            _breakpointService.BreakpointChanged += OnBreakpointChanged;
            _settingService = settingService;
        }

        private void OnBreakpointChanged(object? sender, MyBreakpointChangedEventArgs e)
        {
            if (!e.SmAndUpChanged)
            {
                return;
            }

            _popupService.Close(typeof(EnqueuedSnackbars));

            var parameters = new Dictionary<string, object?>()
            {
                {nameof(EnqueuedSnackbars.Position), _breakpointService.Breakpoint.SmAndUp ? SnackPosition.BottomCenter : SnackPosition.TopCenter}
            };

            _popupService.Open(typeof(EnqueuedSnackbars), parameters);
        }

        public Task EnqueueSnackbarAsync(string? message) => EnqueueSnackbarAsync(null, message);

        public Task EnqueueSnackbarAsync(string? title, string? message) => EnqueueSnackbarAsync(title, message, AlertTypes.None);

        private async Task EnqueueSnackbarAsync(string? title, string? message, AlertTypes type)
        {
            int timeout = _settingService.Get(s => s.AlertTimeout);
            await _popupService.EnqueueSnackbarAsync(new()
            {
                Title = title,
                Content = message,
                Type = type,
                Timeout = timeout < 1 ? 2000 : timeout,
                Closeable = _breakpointService.Breakpoint.SmAndUp
            });
        }

        public Task Error(string? message) => Error(null, message);

        public Task Error(string? title, string? message) => EnqueueSnackbarAsync(title, message, AlertTypes.Error);

        public Task Info(string? message) => Info(null, message);

        public Task Info(string? title, string? message) => EnqueueSnackbarAsync(title, message, AlertTypes.Info);

        public Task Success(string? message) => Success(null, message);

        public Task Success(string? title, string? message) => EnqueueSnackbarAsync(title, message, AlertTypes.Success);

        public Task Warning(string? message) => Warning(null, message);

        public Task Warning(string? title, string? message) => EnqueueSnackbarAsync(title, message, AlertTypes.Warning);

        public Task StartLoading(bool opacity = true)
        {
            _popupService.ShowProgressCircular(options =>
            {
                options.Size = 48;
                if (!opacity)
                {
                    options.BackgroundOpacity = 0;
                    options.Class = "my-progress-circular";
                }
            });
            return Task.CompletedTask;
        }

        public Task StopLoading()
        {
            _popupService.HideProgressCircular();
            return Task.CompletedTask;
        }
    }
}
