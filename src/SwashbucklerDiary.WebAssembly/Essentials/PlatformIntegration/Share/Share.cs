using Microsoft.JSInterop;

namespace SwashbucklerDiary.WebAssembly.Essentials
{
    public partial class PlatformIntegration
    {
        public async Task SetClipboard(string text)
        {
            var module = await Module;
            await module.InvokeVoidAsync("setClipboard", text);
        }

        public async Task ShareFileAsync(string title, string path)
        {
            if (!File.Exists(path))
            {
                return;
            }

            var module = await Module;
            await module.InvokeVoidAsync("shareFileAsync", title, path);
        }

        public async Task ShareTextAsync(string title, string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            var module = await Module;
            await module.InvokeVoidAsync("shareTextAsync", title, text);
        }
    }
}
