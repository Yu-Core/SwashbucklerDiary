using Masa.Blazor;
using Masa.Blazor.Popup.Components;
using Masa.Blazor.Presets;
using SwashbucklerDiary.Rcl.Essentials;

namespace SwashbucklerDiary.Rcl.Services
{
    public class AlertService : IAlertService
    {
        private readonly IPopupService _popupService = default!;

        private readonly BreakpointService _breakpointService = default!;

        private readonly ISettingService _settingService = default!;

        private readonly INavigateController _navigateController = default!;

        public AlertService(IPopupService popupService,
            BreakpointService breakpointService,
            ISettingService settingService,
            INavigateController navigateController)
        {
            _popupService = popupService;
            _breakpointService = breakpointService;
            _breakpointService.BreakpointChanged += OnBreakpointChanged;
            _settingService = settingService;
            _navigateController = navigateController;
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
            }).ConfigureAwait(false);
        }

        public Task ErrorAsync(string? message) => ErrorAsync(null, message);

        public Task ErrorAsync(string? title, string? message) => EnqueueSnackbarAsync(title, message, AlertTypes.Error);

        public Task InfoAsync(string? message) => InfoAsync(null, message);

        public Task InfoAsync(string? title, string? message) => EnqueueSnackbarAsync(title, message, AlertTypes.Info);

        public Task SuccessAsync(string? message) => SuccessAsync(null, message);

        public Task SuccessAsync(string? title, string? message) => EnqueueSnackbarAsync(title, message, AlertTypes.Success);

        public Task WarningAsync(string? message) => WarningAsync(null, message);

        public Task WarningAsync(string? title, string? message) => EnqueueSnackbarAsync(title, message, AlertTypes.Warning);

        public void StartLoading(bool opacity = true)
        {
            _navigateController.DisableNavigate = true;
            _popupService.ShowProgressCircular(options =>
            {
                options.Size = 48;
                if (!opacity)
                {
                    options.BackgroundOpacity = 0;
                    options.Color = null;
                }
            });
        }

        public void StopLoading()
        {
            _navigateController.DisableNavigate = false;
            _popupService.HideProgressCircular();
        }
    }
}
