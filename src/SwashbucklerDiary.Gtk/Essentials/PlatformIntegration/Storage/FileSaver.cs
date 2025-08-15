using Gtk;

namespace SwashbucklerDiary.Gtk.Essentials
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
            var isSuccess = await SaveFileAsync(name, stream);
            return isSuccess;
        }

        private static async Task<bool> SaveFileAsync(string name, Stream stream)
        {
            using var fileDialog = FileDialog.New();
            fileDialog.Modal = true;
            fileDialog.InitialName = name;

            Gio.File? file = null;
            try
            {
                file = await fileDialog.SaveAsync();
            }
            catch (Exception) { }

            string? filePath = file?.GetPath();
            if (string.IsNullOrEmpty(filePath))
            {
                return false;
            }

            using var fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write);
            await stream.CopyToAsync(fileStream);
            return true;
        }
    }
}
