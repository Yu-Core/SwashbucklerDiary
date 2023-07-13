namespace SwashbucklerDiary.Services
{
    public partial class PlatformService
    {
        public async Task SetClipboard(string text)
        {
            await Clipboard.Default.SetTextAsync(text);
        }
    }
}
