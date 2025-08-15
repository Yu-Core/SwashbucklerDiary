#if MACCATALYST
using UniformTypeIdentifiers;
#endif

using MauiBlazorToolkit.Essentials;
using SwashbucklerDiary.Rcl.Essentials;

namespace SwashbucklerDiary.Maui.Essentials
{
    public partial class PlatformIntegration
    {
#if MACCATALYST
        static readonly string[] videoTypes = [.. GetUTTypeIdentifiers(PlatformIntegrationHelper.VideoFileExtensions)];
#elif WINDOWS
        static readonly string[] videoTypes = PlatformIntegrationHelper.VideoFileExtensions;
#elif TIZEN
        static readonly string[] videoTypes = PlatformIntegrationHelper.VideoMimeTypes;
#endif
        public async Task<string?> PickVideoAsync()
        {
#if MACCATALYST
            return await PickFileAsync(videoTypes, PlatformIntegrationHelper.VideoFileExtensions);
#elif ANDROID || IOS
            FileResult? fileResult = await MainThread.InvokeOnMainThreadAsync(() =>
            {
                return MediaFilePicker.Default.PickVideoAsync();
            });
            return fileResult?.FullPath;
#else
            FileResult? fileResult = await MainThread.InvokeOnMainThreadAsync(() =>
            {
                return MediaPicker.Default.PickVideoAsync();
            });
            return fileResult?.FullPath;
#endif
        }

        public async Task<IEnumerable<string>?> PickMultipleVideoAsync()
        {
#if MACCATALYST || WINDOWS || TIZEN
            return await PickMultipleFileAsync(videoTypes, PlatformIntegrationHelper.VideoFileExtensions).ConfigureAwait(false);
#elif ANDROID || IOS
            var fileResults = await MainThread.InvokeOnMainThreadAsync(() =>
            {
                return MediaFilePicker.Default.PickMultipleVideoAsync();
            }).ConfigureAwait(false);
            if (fileResults is null)
            {
                return null;
            }

            return fileResults.Select(it => it.FullPath);
#endif
        }
    }
}
