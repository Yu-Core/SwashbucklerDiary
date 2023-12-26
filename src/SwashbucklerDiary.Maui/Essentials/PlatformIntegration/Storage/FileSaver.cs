using CommunityToolkit.Maui.Storage;
using Microsoft.Extensions.Logging;

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
            var isSuccess = await SaveFileAsync(name, stream);
            return isSuccess;
        }

        public async Task<bool> SaveFileAsync(string name, Stream stream)
        {
            try
            {
                FileSaverResult? fileSaverResult = await FileSaver.Default.SaveAsync(name, stream, default);

                return fileSaverResult.IsSuccessful;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "SaveFileAsync fail");
            }

            return false;
        }
    }
}
