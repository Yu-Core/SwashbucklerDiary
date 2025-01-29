using Microsoft.Extensions.Logging;
using SwashbucklerDiary.Rcl.Essentials;

#if ANDROID
using MauiBlazorToolkit.Essentials;
#endif

namespace SwashbucklerDiary.Maui.Essentials
{
    public partial class PlatformIntegration
    {
        private async Task<string?> PickFileAsync(IEnumerable<string> types, string[] fileExtensions)
        {
            PickOptions options = GetPickOptions(types);

            try
            {
#if ANDROID
                var fileResult = await AndroidFilePicker.Default.PickAsync(options);
#else
                var fileResult = await FilePicker.Default.PickAsync(options);
#endif
                if (fileResult is not null)
                {
                    if (PlatformIntegrationHelper.ValidFileExtension(fileResult.FileName, fileExtensions))
                    {
                        return fileResult.FullPath;
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"{nameof(PickFileAsync)} wrong");
            }

            return null;
        }

        private async Task<IEnumerable<string>?> PickMultipleFileAsync(IEnumerable<string> types, string[] fileExtensions)
        {
            PickOptions options = GetPickOptions(types);

#if ANDROID
            var fileResults = await AndroidFilePicker.Default.PickMultipleAsync(options);
#else
            var fileResults = await FilePicker.Default.PickMultipleAsync(options);
#endif
            if (fileResults is null)
            {
                return null;
            }

            var filePaths = new List<string>();
            try
            {
                foreach (var fileResult in fileResults)
                {
                    if (fileResult is not null)
                    {
                        if (PlatformIntegrationHelper.ValidFileExtension(fileResult.FileName, fileExtensions))
                        {
                            filePaths.Add(fileResult.FullPath);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"{nameof(PickMultipleFileAsync)} wrong");
            }

            return filePaths;
        }

        private static PickOptions GetPickOptions(IEnumerable<string> types)
        {
            var fileTypes = new Dictionary<DevicePlatform, IEnumerable<string>>()
            {
                { DeviceInfo.Current.Platform, types }
            };
            return new()
            {
                FileTypes = new(fileTypes)
            };
        }

#if IOS || MACCATALYST
        private static List<string> GetUTTypeIdentifiers(IEnumerable<string> fileExtensions)
        {
            var identifiers = new List<string>();
            foreach (var ext in fileExtensions)
            {
                var trimmedExt = ext.TrimStart('.');
                var identifier = UniformTypeIdentifiers.UTType.CreateFromExtension(trimmedExt)?.Identifier;
                if (!string.IsNullOrEmpty(identifier))
                {
                    identifiers.Add(identifier);
                }
            }

            return identifiers;
        }
#endif
    }
}
