using CommunityToolkit.Maui.Storage;

namespace SwashbucklerDiary.Services
{
    public partial class PlatformService
    {
        public Task<string?> SaveFileAsync(string name, Stream stream)
        {
            return SaveFileAsync(string.Empty, name, stream);
        }
        public async Task<string?> SaveFileAsync(string? path, string name, Stream stream)
        {
            try
            {
                FileSaverResult? fileSaverResult;
                if (string.IsNullOrEmpty(path))
                {
                    fileSaverResult = await FileSaver.Default.SaveAsync(name, stream, default);
                }
                else
                {
                    fileSaverResult = await FileSaver.Default.SaveAsync(path, name, stream, default);
                }

                if (fileSaverResult.IsSuccessful)
                {
                    return fileSaverResult.FilePath;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
