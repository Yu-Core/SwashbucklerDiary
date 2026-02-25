using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Web.Essentials
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

            string fileName = Path.GetFileName(path);
            string mimeType = StaticContentProvider.GetResponseContentTypeOrDefault(fileName);
            bool isSuccess = await _jsModule.ShareFileAsync(title, path, fileName, mimeType).ConfigureAwait(false);

            if (!isSuccess)
            {
                await _alertService.ErrorAsync("Share fail", "Please use the latest browser or access in HTTPS mode");
            }
        }

        public async Task ShareTextAsync(string title, string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            bool isSuccess = await _jsModule.ShareTextAsync(title, text).ConfigureAwait(false);

            if (!isSuccess)
            {
                await _alertService.ErrorAsync("Share fail", "Please use the latest browser or access in HTTPS mode");
            }
        }
    }
}
