using Masa.Blazor;
using Masa.Blazor.Popup;
using Masa.Blazor.Popup.Components;
using Masa.Blazor.Presets;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Services
{
    public class AlertService : IAlertService
    {
        private readonly IPopupService _popupService = default!;

        private readonly IPopupProvider _popupProvider = default!;

        private readonly MasaBlazorHelper _masaBlazorHelper = default!;

        private readonly ISettingService _settingService = default!;

        public AlertService(IPopupService popupService,
            IPopupProvider popupProvider,
            MasaBlazorHelper masaBlazorHelper,
            ISettingService settingService)
        {
            _popupService = popupService;
            _popupProvider = popupProvider;
            _masaBlazorHelper = masaBlazorHelper;
            _masaBlazorHelper.BreakpointChanged += OnBreakpointChanged;
            _settingService = settingService;
        }

        private void OnBreakpointChanged(object? sender, MyBreakpointChangedEventArgs e)
        {
            if (!e.SmAndUpChanged)
            {
                return;
            }

            var providerItem = _popupProvider.GetItems().FirstOrDefault(it => it.ComponentType == typeof(EnqueuedSnackbars));
            if (providerItem is not null)
            {
                _popupProvider.Remove(providerItem);
            }

            var parameters = new Dictionary<string, object?>()
            {
                {nameof(EnqueuedSnackbars.Position), _masaBlazorHelper.Breakpoint.SmAndUp ? SnackPosition.BottomCenter : SnackPosition.TopCenter}
            };

            _popupProvider.Add(typeof(EnqueuedSnackbars), parameters, _popupService, nameof(PopupService));
        }

        public Task Alert(string? message) => Alert(null, message);

        public Task Alert(string? title, string? message) => Alert(title, message, AlertTypes.None);

        public async Task Alert(string? title, string? message, AlertTypes type)
        {
            int timeout = _settingService.Get<int>(Setting.AlertTimeout);
            await _popupService.EnqueueSnackbarAsync(new()
            {
                Title = title,
                Content = message,
                Type = type,
                Timeout = timeout == 0 ? 2000 : timeout,
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
    }
}
