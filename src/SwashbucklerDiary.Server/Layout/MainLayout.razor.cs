using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Server.Services;

namespace SwashbucklerDiary.Server.Layout
{
    public partial class MainLayout : Rcl.Web.Layout.MainLayoutBase
    {
        [Inject]
        private IAppLockService AppLockService { get; set; } = default!;

        [Inject]
        private IApiAuthService ApiAuthService { get; set; } = default!;

        [Inject]
        private ApiAuthJSModule ApiAuthJSModule { get; set; } = default!;

        [CascadingParameter] HttpContext? HttpContext { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            AppLockService.ValidationSucceeded += SetCookieAsync;
            AppLockService.LockChanged += HandleAppLockChanged;
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

            AppLockService.ValidationSucceeded -= SetCookieAsync;
            AppLockService.LockChanged -= HandleAppLockChanged;
        }
    }
}