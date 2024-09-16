using Microsoft.JSInterop;

namespace SwashbucklerDiary.WebAssembly.Essentials
{
    public partial class PlatformIntegration
    {
        private Task<string?> PickFileAsync(string type, string fileExtension)
            => PickFileAsync([type], [fileExtension]);

        private Task<string?> PickFileAsync(IEnumerable<string> types, string fileExtension)
            => PickFileAsync(types, [fileExtension]);

        private Task<string?> PickFileAsync(string type, string[] fileExtensions)
            => PickFileAsync([type], fileExtensions);

        private async Task<string?> PickFileAsync(IEnumerable<string> types, string[] fileExtensions)
        {
            var results = await PickFilesAsync(types, fileExtensions, false);
            if (results is null || !results.Any())
            {
                return null;
            }

            return results.FirstOrDefault();
        }

        private Task<IEnumerable<string>?> PickFilesAsync(string type, string fileExtension)
            => PickFilesAsync([type], [fileExtension]);

        private Task<IEnumerable<string>?> PickFilesAsync(IEnumerable<string> types, string fileExtension)
            => PickFilesAsync(types, [fileExtension]);

        private Task<IEnumerable<string>?> PickFilesAsync(string type, string[] fileExtensions)
            => PickFilesAsync([type], fileExtensions);

        private async Task<IEnumerable<string>?> PickFilesAsync(IEnumerable<string> types, string[] fileExtensions, bool multiple = true)
        {
            var accept = string.Join(',', types);
            try
            {
                var module = await Module;
                var results = await module.InvokeAsync<string[]>("pickFilesAsync", accept, fileExtensions, multiple);
                if (results is not null && results.Length > 0)
                {
                    return results;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"{nameof(PickFilesAsync)} wrong, types:{accept}, fileExtensions:{string.Join(',', fileExtensions)}, multiple:{multiple}");
            }

            return null;
        }
    }
}
