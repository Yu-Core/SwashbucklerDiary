using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Server.Services;

namespace SwashbucklerDiary.Server.Layout
{
    public partial class MainLayout : Rcl.Web.Layout.MainLayoutBase
    {
        private bool showSetAppLockAlert;

        [Inject]
        private IApiAuthService ApiAuthService { get; set; } = default!;

        [Inject]
        private ApiAuthJSModule ApiAuthJSModule { get; set; } = default!;

        [CascadingParameter] HttpContext? HttpContext { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            AppLockService.ValidationSucceeded += HandleValidationSucceeded;
            AppLockService.LockChanged += HandleAppLockChanged;
            AppLifecycle.AfterFirstEntered += HandleAfterFirstEntered;
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            if (HttpContext is null)
            {
                await InternalOnInitializedAsync();
            }
        }

        protected override void OnDispose()
        {
            base.OnDispose();

            AppLockService.ValidationSucceeded -= HandleValidationSucceeded;
            AppLockService.LockChanged -= HandleAppLockChanged;
            AppLifecycle.AfterFirstEntered -= HandleAfterFirstEntered;
        }

        private async Task HandleValidationSucceeded(AppLockEventArgs _)
        {
            await SetCookieAsync();
        }

        private async Task SetCookieAsync()
        {
            var apiKey = ApiAuthService.GenerateApiKey();
            await ApiAuthJSModule.SetCookie(apiKey);
        }

        private void HandleAppLockChanged()
        {
            ApiAuthService.UpdateVersion();
            InvokeAsync(SetCookieAsync);
        }

        private void HandleAfterFirstEntered()
        {
            InvokeAsync(() =>
            {
                showSetAppLockAlert = true;
            });
        }
    }
}