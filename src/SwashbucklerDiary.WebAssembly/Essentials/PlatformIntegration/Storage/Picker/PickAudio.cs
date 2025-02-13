using SwashbucklerDiary.Rcl.Essentials;

namespace SwashbucklerDiary.WebAssembly.Essentials
{
    public partial class PlatformIntegration
    {
        public Task<string?> PickAudioAsync()
            => PickFileAsync(PlatformIntegrationHelper.AudioMimeTypes, PlatformIntegrationHelper.AudioFileExtensions);

        public Task<IEnumerable<string>?> PickMultipleAudioAsync()
            => PickFilesAsync(PlatformIntegrationHelper.AudioMimeTypes, PlatformIntegrationHelper.AudioFileExtensions);
    }
}
