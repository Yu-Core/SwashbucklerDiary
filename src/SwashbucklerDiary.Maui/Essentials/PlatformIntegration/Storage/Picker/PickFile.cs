using Microsoft.Extensions.Logging;

namespace SwashbucklerDiary.Maui.Essentials
{
    public partial class PlatformIntegration
    {
        private Task<string?> PickFileAsync(PickOptions options, string suffixName)
        {
            string[] suffixNames = { suffixName };
            return PickFileAsync(options, suffixNames);
        }

        private async Task<string?> PickFileAsync(PickOptions options, string[] suffixNames)
        {
            try
            {
                var result = await FilePicker.Default.PickAsync(options);
                if (result != null)
                {
                    foreach (var suffixName in suffixNames)
                    {
                        if (result.FileName.EndsWith(suffixName, StringComparison.OrdinalIgnoreCase))
                        {
                            return result.FullPath;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"{nameof(PickFileAsync)} wrong");
            }
            
            return null;
        }
    }
}
