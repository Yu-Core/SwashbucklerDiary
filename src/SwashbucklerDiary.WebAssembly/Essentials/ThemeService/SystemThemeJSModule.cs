using Masa.Blazor.JSInterop;
using Microsoft.JSInterop;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.WebAssembly.Essentials
{
    public class SystemThemeJSModule : JSModule
    {
        public Theme SystemTheme { get; set; }

        public event Func<Theme, Task>? SystemThemeChanged;

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
        public Task OnSystemThemeChanged(bool isDarkTheme)
        {
            SystemTheme = isDarkTheme ? Theme.Dark : Theme.Light;
            return SystemThemeChanged?.Invoke(SystemTheme) ?? Task.CompletedTask;
        }
    }
}
