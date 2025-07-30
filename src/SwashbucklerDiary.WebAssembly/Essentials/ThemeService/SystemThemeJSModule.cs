using Masa.Blazor.JSInterop;
using Microsoft.JSInterop;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.WebAssembly.Essentials
{
    public class SystemThemeJSModule : JSModule
    {
        private readonly DotNetObjectReference<object>? _dotNetObjectReference;

        public Theme SystemTheme { get; set; }

        public event Action<Theme>? OnSystemThemeChanged;

        public SystemThemeJSModule(IJSRuntime js) : base(js, "./js/systemTheme.js")
        {
            _dotNetObjectReference = DotNetObjectReference.Create<object>(this);
        }

        [JSInvokable]
        public void SystemThemeChange(bool isDarkTheme)
        {
            SystemTheme = isDarkTheme ? Theme.Dark : Theme.Light;
            OnSystemThemeChanged?.Invoke(SystemTheme);
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
