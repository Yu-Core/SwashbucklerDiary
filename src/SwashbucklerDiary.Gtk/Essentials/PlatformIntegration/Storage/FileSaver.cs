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

        public async Task<bool> SaveFileAsync(string name, Stream stream)
        {
            // Create a new GtkFileChooserNative object and set its properties
            using var saveFileDialog = new FileChooserNative("Save File",
                    null,
                    FileChooserAction.Save,
                    "Save",
                    "Cancel");

            if (saveFileDialog.Run() == (int)ResponseType.Accept)
            {
                string filename = saveFileDialog.Filename;

                if (!string.IsNullOrEmpty(filename))
                {
                    try
                    {
                        using var fileStream = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write);
                        await stream.CopyToAsync(fileStream);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error saving file: " + ex.Message);
                    }
                }
            };

            return false;
        }
    }
}
