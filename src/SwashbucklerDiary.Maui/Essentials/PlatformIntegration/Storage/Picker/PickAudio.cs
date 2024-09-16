#if IOS || MACCATALYST
using UniformTypeIdentifiers;
#endif

namespace SwashbucklerDiary.Maui.Essentials
{
    public partial class PlatformIntegration
    {
        static readonly string[] audioFileExtensions = [".mp3", ".wav", ".m4a", ".ogg", ".aac", ".flac"];
#if WINDOWS
        static readonly string[] audioTypes = audioFileExtensions;
#elif ANDROID || TIZEN
        static readonly string[] audioTypes = ["audio/mpeg", "audio/wav", "audio/mp4", "audio/ogg", "audio/aac", "audio/flac"];
#elif MACCATALYST || IOS
        static readonly string[] audioTypes = audioFileExtensions.Select(it => UTType.CreateFromExtension(it)?.Identifier ?? string.Empty).ToArray();
#endif
        public Task<string?> PickAudioAsync()
        {
            return PickFileAsync(audioTypes, audioFileExtensions);
        }

        public Task<IEnumerable<string>?> PickMultipleAudioAsync()
        {
            return PickMultipleFileAsync(audioTypes, audioFileExtensions);
        }
    }
}
