namespace SwashbucklerDiary.WebAssembly.Essentials
{
    public partial class PlatformIntegration
    {
        public async Task SetClipboardAsync(string text)
        {
            await _jsModule.SetClipboard(text).ConfigureAwait(false);
        }

        public async Task ShareFileAsync(string title, string path)
        {
            if (!File.Exists(path))
            {
                return;
            }

            await _jsModule.ShareFileAsync(title, path).ConfigureAwait(false);
        }

        public async Task ShareTextAsync(string title, string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            await _jsModule.ShareTextAsync(title, text).ConfigureAwait(false);
        }
    }
}
