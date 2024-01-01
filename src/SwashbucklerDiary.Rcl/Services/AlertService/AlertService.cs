using BlazorComponent;
using Masa.Blazor;
using SwashbucklerDiary.Rcl.Services;

namespace SwashbucklerDiary.Services
{
    public class AlertService : IAlertService
    {
        private IPopupService _popupService = default!;

        private int Timeout;

        public void Initialize(object popupService)
        {
            _popupService = (IPopupService)popupService;
        }

        public AlertService(IPopupService popupService)
        {
            _popupService = popupService;
        }

        public Task Alert(string? message) => Alert(null, message);

        public Task Alert(string? title, string? message) => Alert(title, message, AlertTypes.None);

        public async Task Alert(string? title, string? message, AlertTypes type)
        {
            
            await _popupService.EnqueueSnackbarAsync(new()
            {
                Title = title,
                Content = message,
                Type = type,
                Timeout = Timeout
            });
        }

        public Task Error(string? message) => Error(null, message);

        public Task Error(string? title, string? message) => Alert(title, message, AlertTypes.Error);

        public Task Info(string? message) => Info(null, message);

        public Task Info(string? title, string? message) => Alert(title, message, AlertTypes.Info);

        public Task Success(string? message) => Success(null, message);

        public Task Success(string? title, string? message) => Alert(title, message, AlertTypes.Success);

        public Task Warning(string? message) => Warning(null, message);

        public Task Warning(string? title, string? message) => Alert(title, message, AlertTypes.Warning);

        public Task StartLoading(bool opacity = true)
        {
            _popupService.ShowProgressCircular(options =>
            {
                options.Size = 48;
                if (!opacity)
                {
                    options.BackgroundOpacity = 0;
                    options.Class = "my-alert-loading";
                }
            });
            return Task.CompletedTask;
        }

        public Task StopLoading()
        {
            _popupService.HideProgressCircular();
            return Task.CompletedTask;
        }

        public void SetTimeout(int timeout)
        {
            Timeout = timeout;
        }
    }
}
