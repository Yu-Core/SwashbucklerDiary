using CommunityToolkit.Maui.Storage;

namespace SwashbucklerDiary.Maui.Essentials
{
#pragma warning disable CA1416
    public partial class PlatformIntegration
    {
        public Task<bool> SaveFileAsync(string sourceFilePath)
        {
            var fileName = Path.GetFileName(sourceFilePath);
            return SaveFileAsync(fileName, sourceFilePath);
        }

        public async Task<bool> SaveFileAsync(string name, string sourceFilePath)
        {
            if (!File.Exists(sourceFilePath))
            {
                return false;
            }

            using FileStream stream = File.OpenRead(sourceFilePath);
            //Cannot save an existing file
            //https://github.com/CommunityToolkit/Maui/issues/1049
            var isSuccess = await SaveFileAsync(name, stream).ConfigureAwait(false);
            return isSuccess;
        }

        public async Task<bool> SaveFileAsync(string name, Stream stream)
        {
            FileSaverResult? fileSaverResult = await MainThread.InvokeOnMainThreadAsync(() =>
            {
                return FileSaver.Default.SaveAsync(name, stream, default);
            }).ConfigureAwait(false);

            return fileSaverResult.IsSuccessful;
        }
    }
}
