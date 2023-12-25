namespace SwashbucklerDiary.Maui.Essentials
{
    public partial class PlatformIntegration
    {
        public async Task<bool> OpenBrowser(string? url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return false;
            }

            return await Browser.Default.OpenAsync(url, BrowserLaunchMode.External);
        }
    }
}
