using SwashbucklerDiary.Rcl.Essentials;

namespace SwashbucklerDiary.Gtk.Essentials
{
    public partial class PlatformIntegration
    {
        const string audioFilefilterName = "Audio file";

        const string audioFilesfilterName = "Audio files";

        static readonly string[] audioPatterns = GetPatterns(PlatformIntegrationHelper.AudioFileExtensions);

        public Task<string?> PickAudioAsync()
            => PickFileAsync(audioFilefilterName, audioPatterns, PlatformIntegrationHelper.AudioFileExtensions);

        public Task<IEnumerable<string>?> PickMultipleAudioAsync()
            => PickMultipleFileAsync(audioFilesfilterName, audioPatterns, PlatformIntegrationHelper.AudioFileExtensions);
    }
}
