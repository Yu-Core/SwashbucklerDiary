#if MACCATALYST
using UniformTypeIdentifiers;
#endif

using MauiBlazorToolkit.Essentials;

namespace SwashbucklerDiary.Maui.Essentials
{
    public partial class PlatformIntegration
    {
#if MACCATALYST || WINDOWS || TIZEN
        static readonly string[] videoFileExtensions = [".mp4", ".m4v", ".mpg", ".mpeg", ".mp2", ".mov", ".avi", ".mkv", ".flv", ".gifv", ".qt"];
#if MACCATALYST
        static readonly string[] videoTypes = [.. ConvertFileExtensionsToUTTypeIdentifiers(videoFileExtensions)];
#elif WINDOWS
        static readonly string[] videoTypes = videoFileExtensions;
#elif TIZEN
        static readonly string[] videoTypes = ["video/*"];
#endif

#endif
        public async Task<string?> PickVideoAsync()
        {
#if MACCATALYST
            return await PickFileAsync(videoTypes, videoFileExtensions);
#elif ANDROID || IOS
            FileResult? fileResult = await MediaFilePicker.Default.PickVideoAsync();
            return fileResult?.FullPath;
#else
            FileResult? fileResult = await MediaPicker.Default.PickVideoAsync();
            return fileResult?.FullPath;
#endif
        }

        public async Task<IEnumerable<string>?> PickMultipleVideoAsync()
        {
#if MACCATALYST || WINDOWS || TIZEN
            return await PickMultipleFileAsync(videoTypes, videoFileExtensions);
#elif ANDROID || IOS
            var fileResults = await MediaFilePicker.Default.PickMultipleVideoAsync();
            if (fileResults is null)
            {
                return null;
            }

            return fileResults.Select(it => it.FullPath);
#endif
        }
    }
}
