using Masa.Blazor.JSInterop;
using Microsoft.JSInterop;

namespace SwashbucklerDiary.Rcl.Web.Essentials
{
    public abstract class PlatformIntegrationJSModule : JSModule
    {
        public PlatformIntegrationJSModule(IJSRuntime js, string moduleUrl) : base(js, moduleUrl)
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
            return InvokeVoidAsync("shareText", title, text);
        }

        public ValueTask ShareFileAsync(string title, string path, string fileName, string mimeType)
        {
            return InvokeVoidAsync("shareFile", title, path, fileName, mimeType);
        }

        public ValueTask SaveFileAsync(string fileName, string filePath)
        {
            return InvokeVoidAsync("downloadFile", fileName, filePath);
        }

        public abstract ValueTask<string[]?> PickFilesAsync(
            string accept,
            string[] fileExtensions,
            bool multiple = true);

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
