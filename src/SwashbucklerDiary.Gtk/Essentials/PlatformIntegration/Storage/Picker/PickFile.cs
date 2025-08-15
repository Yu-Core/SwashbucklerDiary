using Gtk;
using SwashbucklerDiary.Rcl.Essentials;

namespace SwashbucklerDiary.Gtk.Essentials
{
    public partial class PlatformIntegration
    {
        private static async Task<string?> PickFileAsync(string filterName, IEnumerable<string> patterns, string[] fileExtensions)
        {
            using var fileDialog = FileDialog.New();
            fileDialog.Modal = true;
            fileDialog.AddFilter(filterName, patterns);

            try
            {
                var file = await fileDialog.OpenAsync().ConfigureAwait(false);
                var filePath = file?.GetPath();
                if (PlatformIntegrationHelper.ValidFileExtension(filePath, fileExtensions))
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
                var files = await fileDialog.OpenMultipleAsync().ConfigureAwait(false);
                if (files is not null)
                {
                    uint fileCount = files.GetNItems();
                    for (uint i = 0; i < fileCount; i++)
                    {
                        nint fileValue = files.GetItem(i);
                        var file = new Gio.FileHelper(new(fileValue, true));
                        string? filePath = file.GetPath();

                        if (PlatformIntegrationHelper.ValidFileExtension(filePath, fileExtensions))
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
