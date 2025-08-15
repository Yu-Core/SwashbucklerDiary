#if MACCATALYST
using UniformTypeIdentifiers;
#endif

#if ANDROID || IOS
using MauiBlazorToolkit.Essentials;
#endif

using SwashbucklerDiary.Rcl.Essentials;

namespace SwashbucklerDiary.Maui.Essentials
{
    public partial class PlatformIntegration
    {
#if MACCATALYST
        static readonly string[] imageTypes = [.. GetUTTypeIdentifiers(PlatformIntegrationHelper.ImageFileExtensions)];
#elif WINDOWS
        static readonly string[] imageTypes = PlatformIntegrationHelper.ImageFileExtensions;
#elif TIZEN
        static readonly string[] imageTypes = PlatformIntegrationHelper.ImageMimeTypes;
#endif
        public async Task<string?> PickPhotoAsync()
        {
#if MACCATALYST || WINDOWS
            return await PickFileAsync(imageTypes, PlatformIntegrationHelper.ImageFileExtensions).ConfigureAwait(false);
#elif ANDROID || IOS
            FileResult? fileResult = await MainThread.InvokeOnMainThreadAsync(() =>
            {
                return MediaFilePicker.Default.PickPhotoAsync();
            }).ConfigureAwait(false);
            return fileResult?.FullPath;
#else
            FileResult? fileResult = await MediaPicker.Default.PickPhotoAsync().ConfigureAwait(false);
            return fileResult?.FullPath;
#endif
        }

        public async Task<IEnumerable<string>?> PickMultiplePhotoAsync()
        {
#if MACCATALYST || WINDOWS || TIZEN
            return await PickMultipleFileAsync(imageTypes, PlatformIntegrationHelper.ImageFileExtensions).ConfigureAwait(false);
#elif ANDROID || IOS
            var fileResults = await MainThread.InvokeOnMainThreadAsync(() =>
            {
                return MediaFilePicker.Default.PickMultiplePhotoAsync();
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
