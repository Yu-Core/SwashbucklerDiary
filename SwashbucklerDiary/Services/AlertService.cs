using BlazorComponent;
using Masa.Blazor;
using SwashbucklerDiary.IServices;

namespace SwashbucklerDiary.Services
{
    public class AlertService : IAlertService
    {
        private IPopupService PopupService = default!;
        private int Timeout;

        public void Initialize(object popupService)
        {
            PopupService = (IPopupService)popupService;
        }

        public Task Alert(string? message) => Alert(null, message);

        public Task Alert(string? title, string? message) => Alert(title, message, AlertTypes.None);

        public async Task Alert(string? title, string? message, AlertTypes type)
        {
            
            await PopupService.EnqueueSnackbarAsync(new()
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
            PopupService.ShowProgressCircular(options =>
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
            PopupService.HideProgressCircular();
            return Task.CompletedTask;
        }

        public void SetTimeout(int timeout)
        {
            Timeout = timeout;
        }
    }
}
