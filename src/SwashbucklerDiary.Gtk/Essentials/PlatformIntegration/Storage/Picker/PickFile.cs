using Gtk;

namespace SwashbucklerDiary.Gtk.Essentials
{
    public partial class PlatformIntegration
    {
        private Task<string?> PickFileAsync(IEnumerable<string> types, string fileExtension)
        {
            string[] fileExtensions = [fileExtension];
            return PickFileAsync(types, fileExtensions);
        }

        private Task<string?> PickFileAsync(IEnumerable<string> types, string[] fileExtensions)
        {
            string? filePath = null;
            using var openFileDialog = new FileChooserNative("Select a File",
                    null,
                    FileChooserAction.Open,
                    "Open",
                    "Cancel");
            openFileDialog.AddFilter(types);

            if (openFileDialog.Run() == (int)ResponseType.Accept)
            {
                var fileExtension = Path.GetExtension(openFileDialog.Filename);
                if (fileExtensions.Contains(fileExtension))
                {
                    filePath = openFileDialog.Filename;
                }
            };

            return Task.FromResult(filePath);
        }

        private Task<IEnumerable<string>?> PickMultipleFileAsync(IEnumerable<string> types, string[] fileExtensions)
        {
            List<string> filePaths = [];
            using var openFileDialog = new FileChooserNative("Select multiple Files",
                    null,
                    FileChooserAction.Open,
                    "Open",
                    "Cancel");
            openFileDialog.SelectMultiple = true;
            openFileDialog.AddFilter(types);

            if (openFileDialog.Run() == (int)ResponseType.Accept)
            {
                foreach (var filename in openFileDialog.Filenames)
                {
                    var fileExtension = Path.GetExtension(filename);
                    if (fileExtensions.Contains(fileExtension))
                    {
                        filePaths.Add(filename);
                    }
                }
            };

            return Task.FromResult<IEnumerable<string>?>(filePaths);
        }
    }

    public static class FileChooserNativeUtils
    {
        public static void AddFilter(this FileChooserNative fileChooserNative, IEnumerable<string> mimeTypes)
        {
            foreach (var type in mimeTypes)
            {
                var fileFilter = new FileFilter();
                fileFilter.AddMimeType(type);
                fileChooserNative.AddFilter(fileFilter);
            }
        }
    }
}
