using Masa.Blazor.JSInterop;
using Microsoft.JSInterop;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Web.Essentials
{
    public class SystemThemeJSModule : JSModule
    {
        private readonly DotNetObjectReference<object>? _dotNetObjectReference;

        public Theme SystemTheme { get; set; }

        public event Action<Theme>? SystemThemeChanged;

        public SystemThemeJSModule(IJSRuntime js) : base(js, "./_content/SwashbucklerDiary.Rcl.Web/js/systemTheme.js")
        {
            _dotNetObjectReference = DotNetObjectReference.Create<object>(this);
        }

        [JSInvokable]
        public void OnSystemThemeChanged(bool isDarkTheme)
        {
            SystemTheme = isDarkTheme ? Theme.Dark : Theme.Light;
            SystemThemeChanged?.Invoke(SystemTheme);
        }

        public async Task Init()
        {
            bool dark = await InvokeAsync<bool>("init", _dotNetObjectReference);
            SystemTheme = dark ? Theme.Dark : Theme.Light;
        }

        protected override async ValueTask DisposeAsyncCore()
        {
            await base.DisposeAsyncCore();

            _dotNetObjectReference?.Dispose();
        }
    }
}
