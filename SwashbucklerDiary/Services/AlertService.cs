using BlazorComponent;
using Masa.Blazor;
using SwashbucklerDiary.IServices;

namespace SwashbucklerDiary.Services
{
    public class AlertService : IAlertService
    {
        private IPopupService PopupService = default!;

        public void Initialize(IPopupService popupService)
        {
            PopupService = popupService;
        }

        public Task Alert(string? message) => Alert(null, message);

        public Task Alert(string? title, string? message) => Alert(title, message, AlertTypes.None);

        public async Task Alert(string? title, string? message,AlertTypes type)
        {
            await PopupService.EnqueueSnackbarAsync(new()
            {
                Title = title,
                Content = message,
                Type = AlertTypes.None
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
    }
}
