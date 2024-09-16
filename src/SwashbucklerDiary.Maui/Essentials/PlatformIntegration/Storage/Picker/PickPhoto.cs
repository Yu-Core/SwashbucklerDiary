#if MACCATALYST
using UniformTypeIdentifiers;
#endif

#if ANDROID || IOS
using MauiBlazorToolkit.Essentials;
#endif

namespace SwashbucklerDiary.Maui.Essentials
{
    public partial class PlatformIntegration
    {
#if MACCATALYST || WINDOWS || TIZEN
        static readonly string[] imageFileExtensions = [".jpg", ".jpeg", ".png", ".gif", ".svg", ".webp", ".jfif"];
#if MACCATALYST
        static readonly string[] imageTypes = imageFileExtensions.Select(it => UTType.CreateFromExtension(it)?.Identifier ?? string.Empty).ToArray();
#elif WINDOWS
        static readonly string[] imageTypes = imageFileExtensions;
#elif TIZEN
        static readonly string[] imageTypes = ["image/*"];
#endif
#endif
        public async Task<string?> PickPhotoAsync()
        {
#if MACCATALYST || WINDOWS
            return await PickFileAsync(imageTypes, imageFileExtensions);
#elif ANDROID || IOS
            FileResult? fileResult = await MediaFilePicker.Default.PickPhotoAsync();
            return fileResult?.FullPath;
#else
            FileResult? fileResult = await MediaPicker.Default.PickPhotoAsync();
            return fileResult?.FullPath;
#endif
        }

        public async Task<IEnumerable<string>?> PickMultiplePhotoAsync()
        {
#if MACCATALYST || WINDOWS || TIZEN
            return await PickMultipleFileAsync(imageTypes, imageFileExtensions);
#elif ANDROID || IOS
            var fileResults = await MediaFilePicker.Default.PickMultiplePhotoAsync();
            if (fileResults is null)
            {
                return null;
            }

            return fileResults.Select(it => it.FullPath);
#endif
        }
    }
}
