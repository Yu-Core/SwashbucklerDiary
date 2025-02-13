#if IOS || MACCATALYST
using UniformTypeIdentifiers;
#endif

using SwashbucklerDiary.Rcl.Essentials;

namespace SwashbucklerDiary.Maui.Essentials
{
    public partial class PlatformIntegration
    {
#if WINDOWS
        static readonly string[] audioTypes = PlatformIntegrationHelper.AudioFileExtensions;
#elif ANDROID || TIZEN
        static readonly string[] audioTypes = PlatformIntegrationHelper.AudioMimeTypes;
#elif MACCATALYST || IOS
        static readonly string[] audioTypes = [.. GetUTTypeIdentifiers(PlatformIntegrationHelper.AudioFileExtensions)];
#endif
        public Task<string?> PickAudioAsync()
            => PickFileAsync(audioTypes, PlatformIntegrationHelper.AudioFileExtensions);

        public Task<IEnumerable<string>?> PickMultipleAudioAsync()
            => PickMultipleFileAsync(audioTypes, PlatformIntegrationHelper.AudioFileExtensions);
    }
}
