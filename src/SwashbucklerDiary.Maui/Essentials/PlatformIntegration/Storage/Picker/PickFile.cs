using Microsoft.Extensions.Logging;

namespace SwashbucklerDiary.Maui.Essentials
{
    public partial class PlatformIntegration
    {
        private Task<string?> PickFileAsync(IEnumerable<string> types, string suffixName)
        {
            string[] suffixNames = [suffixName];
            return PickFileAsync(types, suffixNames);
        }

        private async Task<string?> PickFileAsync(IEnumerable<string> types, string[] suffixNames)
        {
            PickOptions options = new()
            {
                FileTypes = new FilePickerFileType(
                    new Dictionary<DevicePlatform, IEnumerable<string>>()
                    {
                        { DeviceInfo.Current.Platform, types }
                    })
            };

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
