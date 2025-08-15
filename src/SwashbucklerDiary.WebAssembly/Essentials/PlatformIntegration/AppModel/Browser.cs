namespace SwashbucklerDiary.WebAssembly.Essentials
{
    public partial class PlatformIntegration
    {
        public Task<bool> OpenBrowser(string? url)
            => OpenUri(url);

        private Task<bool> OpenLauncher(string? uri)
            => OpenUri(uri);

        private async Task<bool> OpenUri(string? uri)
        {
            if (string.IsNullOrEmpty(uri))
            {
                return false;
            }

            return await _jsModule.OpenUri(uri).ConfigureAwait(false);
        }
    }
}
