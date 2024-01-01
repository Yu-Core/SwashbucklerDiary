namespace SwashbucklerDiary.WebAssembly.Essentials
{
    public partial class PlatformIntegration
    {
        public Task<bool> OpenBrowser(string? url)
            => OpenUri(url, true);

        private Task<bool> OpenLauncher(string? uri)
            => OpenUri(uri, true);

        private async Task<bool> OpenUri(string? uri, bool blank)
        {
            if (string.IsNullOrEmpty(uri))
            {
                return false;
            }

            var module = await Module;
            return module.Invoke<bool>("openUri", uri, blank);
        }
    }
}
