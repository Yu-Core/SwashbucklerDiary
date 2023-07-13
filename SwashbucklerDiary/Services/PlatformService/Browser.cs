namespace SwashbucklerDiary.Services
{
    public partial class PlatformService
    {
        public async Task OpenBrowser(string? url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return;
            }

            await Browser.Default.OpenAsync(url, BrowserLaunchMode.External);
        }
    }
}
