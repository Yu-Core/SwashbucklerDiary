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

        private static Task<bool> SaveFileAsync(string name, Stream stream)
        {
            // Create a new GtkFileChooserNative object and set its properties
            using var fileChooser = FileChooserNative.New("Save File",
                    null,
                    FileChooserAction.Save,
                    "Save",
                    "Cancel");

            fileChooser.Modal = true;
            fileChooser.SetCurrentName(name);

            var tcs = new TaskCompletionSource<bool>();
            fileChooser.OnResponse += async (_, e) =>
            {
                if (e.ResponseId != (int)global::Gtk.ResponseType.Accept)
                {
                    tcs.SetResult(false);
                    return;
                }

                string? filePath = fileChooser.GetFile()?.GetPath();

                if (!string.IsNullOrEmpty(filePath))
                {
                    try
                    {
                        using var fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write);
                        await stream.CopyToAsync(fileStream);
                        tcs.SetResult(true);
                        return;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error saving file: " + ex.Message);
                    }
                }

                tcs.SetResult(false);
            };
            fileChooser.Show();

            return tcs.Task;
        }
    }
}
