using Gtk;
using SwashbucklerDiary.Rcl.Essentials;

namespace SwashbucklerDiary.Gtk.Essentials
{
    public partial class PlatformIntegration
    {
        private static Task<string?> PickFileAsync(string filterName, IEnumerable<string> patterns, string fileExtension)
        {
            string[] fileExtensions = [fileExtension];
            return PickFileAsync(filterName, patterns, fileExtensions);
        }

        private static async Task<string?> PickFileAsync(string filterName, IEnumerable<string> patterns, string[] fileExtensions)
        {
            using var fileDialog = FileDialog.New();
            fileDialog.Modal = true;
            fileDialog.AddFilter(filterName, patterns);

            try
            {
                var file = await fileDialog.OpenAsync();
                var filePath = file?.GetPath();
                if (PlatformIntegrationHelper.ValidFileExtensions(filePath, fileExtensions))
                {
                    return filePath;
                }
            }
            catch (Exception)
            { }

            return null;
        }

        private static async Task<IEnumerable<string>?> PickMultipleFileAsync(string filterName, IEnumerable<string> patterns, string[] fileExtensions)
        {
            List<string> filePaths = [];
            using var fileDialog = FileDialog.New();
            fileDialog.Modal = true;
            fileDialog.AddFilter(filterName, patterns);

            try
            {
                var files = await fileDialog.OpenMultipleAsync();
                if (files is not null)
                {
                    uint fileCount = files.GetNItems();
                    for (uint i = 0; i < fileCount; i++)
                    {
                        nint fileValue = files.GetItem(i);
                        var file = new Gio.FileHelper(fileValue, true);
                        string? filePath = file.GetPath();

                        if (PlatformIntegrationHelper.ValidFileExtensions(filePath, fileExtensions))
                        {
                            filePaths.Add(filePath!);
                        }
                    }
                }
            }
            catch (Exception) { }

            return filePaths;
        }

        private static string[] GetPatterns(string[] fileExtensions)
        {
            List<string> patterns = [];
            foreach (var fileExtension in fileExtensions)
            {
                patterns.Add($"*{fileExtension.ToLower()}");
                patterns.Add($"*{fileExtension.ToUpper()}");
            }
            return patterns.ToArray();
        }
    }
}
