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

            try
            {
                return await Browser.Default.OpenAsync(url, BrowserLaunchMode.External);
            }
            catch (Exception)
            {
                return await Browser.Default.OpenAsync(url, BrowserLaunchMode.SystemPreferred);
            }
        }
    }
}
