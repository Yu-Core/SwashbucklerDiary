using Masa.Blazor.JSInterop;
using Microsoft.JSInterop;

namespace SwashbucklerDiary.WebAssembly.Essentials
{
    public class PlatformIntegrationJsModule : JSModule
    {
        public PlatformIntegrationJsModule(IJSRuntime js) : base(js, "./js/platformIntegration.js")
        {
        }

        public ValueTask<bool> OpenUri(string uri)
        {
            return InvokeAsync<bool>("openUri", uri);
        }

        public ValueTask SetClipboard(string text)
        {
            return InvokeVoidAsync("setClipboard", text);
        }

        public ValueTask ShareTextAsync(string title, string text)
        {
            return InvokeVoidAsync("shareTextAsync", title, text);
        }

        public ValueTask ShareFileAsync(string title, string path)
        {
            return InvokeVoidAsync("shareFileAsync", title, path);
        }

        public ValueTask SaveFileAsync(string fileName, string filePath)
        {
            return InvokeVoidAsync("saveFileAsync", fileName, filePath);
        }

        public ValueTask<string[]?> PickFilesAsync(string accept,
            string[] fileExtensions,
            bool multiple = true)
        {
            return InvokeAsync<string[]?>("pickFilesAsync", accept, fileExtensions, multiple);
        }

        public ValueTask<bool> IsBiometricSupported()
        {
            return InvokeAsync<bool>("isBiometricSupported");
        }

        public ValueTask<bool> BiometricAuthenticateAsync()
        {
            return InvokeAsync<bool>("biometricAuthenticateAsync");
        }
    }
}
