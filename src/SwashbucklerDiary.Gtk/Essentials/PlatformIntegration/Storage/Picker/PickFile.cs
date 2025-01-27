using Gtk;

namespace SwashbucklerDiary.Gtk.Essentials
{
    public partial class PlatformIntegration
    {
        private static Task<string?> PickFileAsync(IEnumerable<string> types, string fileExtension)
        {
            string[] fileExtensions = [fileExtension];
            return PickFileAsync(types, fileExtensions);
        }

        private static Task<string?> PickFileAsync(IEnumerable<string> types, string[] fileExtensions)
        {
            using var fileChooser = FileChooserNative.New("Select a File",
                    null,
                    FileChooserAction.Open,
                    "Open",
                    "Cancel");
            fileChooser.AddFilter(types);

            var tcs = new TaskCompletionSource<string?>();
            fileChooser.OnResponse += (_, e) =>
            {
                if (e.ResponseId != (int)global::Gtk.ResponseType.Accept)
                {
                    tcs.SetResult(null);
                    return;
                }

                var filePath = fileChooser.GetFile()?.GetPath();
                if (!string.IsNullOrEmpty(filePath) && fileExtensions.Contains(Path.GetExtension(filePath)))
                {
                    tcs.SetResult(filePath);
                    return;
                }

                tcs.SetResult(null);
            };

            fileChooser.Show();
            return tcs.Task;
        }

        private static Task<IEnumerable<string>?> PickMultipleFileAsync(IEnumerable<string> types, string[] fileExtensions)
        {
            List<string> filePaths = [];
            using var fileChooser = FileChooserNative.New("Select multiple Files",
                    null,
                    FileChooserAction.Open,
                    "Open",
                    "Cancel");
            fileChooser.Modal = true;
            fileChooser.SelectMultiple = true;
            fileChooser.AddFilter(types);

            var tcs = new TaskCompletionSource<IEnumerable<string>?>();
            fileChooser.OnResponse += (_, e) =>
            {
                if (e.ResponseId != (int)global::Gtk.ResponseType.Accept)
                {
                    tcs.SetResult(filePaths);
                    return;
                }

                var files = fileChooser.GetFiles();
                if (files is not null)
                {
                    uint fileCount = files.GetNItems();
                    for (uint i = 0; i < fileCount; i++)
                    {
                        nint fileValue = files.GetItem(i);
                        var file = new Gio.FileHelper(fileValue, true);
                        string? filePath = file.GetPath();

                        if (!string.IsNullOrEmpty(filePath) && fileExtensions.Contains(Path.GetExtension(filePath)))
                        {
                            filePaths.Add(filePath);
                        }
                    }
                }

                tcs.SetResult(filePaths);
            };

            fileChooser.Show();
            return tcs.Task;
        }
    }

    public static class FileChooserNativeUtils
    {
        public static void AddFilter(this FileChooserNative fileChooserNative, IEnumerable<string> mimeTypes)
        {
            foreach (var type in mimeTypes)
            {
                var fileFilter = FileFilter.New();
                fileFilter.AddMimeType(type);
                fileChooserNative.AddFilter(fileFilter);
            }
        }
    }
}
