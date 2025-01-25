using Masa.Blazor.JSInterop;
using Microsoft.JSInterop;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.WebAssembly.Essentials
{
    public class SystemThemeJSModule : JSModule
    {
        public Theme SystemTheme { get; set; }

        public event Action<Theme>? SystemThemeChanged;

        public SystemThemeJSModule(IJSRuntime js) : base(js, "./js/systemTheme.js")
        {
        }

        public async Task InitializedAsync()
        {
            var dotNetObject = DotNetObjectReference.Create(this);
            bool dark = await InvokeAsync<bool>("registerForSystemThemeChanged", [dotNetObject, nameof(OnSystemThemeChanged)]);
            SystemTheme = dark ? Theme.Dark : Theme.Light;
        }

        [JSInvokable]
        public void OnSystemThemeChanged(bool isDarkTheme)
        {
            SystemTheme = isDarkTheme ? Theme.Dark : Theme.Light;
            SystemThemeChanged?.Invoke(SystemTheme);
        }
    }
}
