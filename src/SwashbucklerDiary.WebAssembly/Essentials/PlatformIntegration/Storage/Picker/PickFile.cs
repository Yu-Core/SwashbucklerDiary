namespace SwashbucklerDiary.WebAssembly.Essentials
{
    public partial class PlatformIntegration
    {
        private async Task<string?> PickFileAsync(IEnumerable<string> types, string[] fileExtensions)
        {
            var results = await PickFilesAsync(types, fileExtensions, false);
            if (results is null || !results.Any())
            {
                return null;
            }

            return results.FirstOrDefault();
        }

        private async Task<IEnumerable<string>?> PickFilesAsync(IEnumerable<string> types, string[] fileExtensions, bool multiple = true)
        {
            var accept = string.Join(',', types);
            try
            {
                var results = await _jsModule.PickFilesAsync(accept, fileExtensions, multiple);
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
