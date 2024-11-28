using Microsoft.Extensions.Logging;

#if ANDROID
using MauiBlazorToolkit.Essentials;
#endif

namespace SwashbucklerDiary.Maui.Essentials
{
    public partial class PlatformIntegration
    {
        private Task<string?> PickFileAsync(IEnumerable<string> types, string fileExtension)
        {
            string[] fileExtensions = [fileExtension];
            return PickFileAsync(types, fileExtensions);
        }

        private async Task<string?> PickFileAsync(IEnumerable<string> types, string[] fileExtensions)
        {
            PickOptions options = GetPickOptions(types);

            try
            {
#if ANDROID
                var result = await AndroidFilePicker.Default.PickAsync(options);
#else
                var result = await FilePicker.Default.PickAsync(options);
#endif
                if (result is not null)
                {
                    var fileExtension = Path.GetExtension(result.FileName);
                    if (fileExtensions.Contains(fileExtension))
                    {
                        return result.FullPath;
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
                        var fileExtension = Path.GetExtension(fileResult.FileName);
                        if (fileExtensions.Contains(fileExtension))
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
        private static List<string> ConvertFileExtensionsToUTTypeIdentifiers(IEnumerable<string> fileExtensions)
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
