namespace SwashbucklerDiary.WebAssembly.Essentials
{
    public partial class PlatformIntegration
    {
        static readonly string[] audioFileExtensions = [".mp3", ".wav", ".m4a", ".ogg", ".aac", ".flac"];

        static readonly string audioMime = "audio/*";

        public Task<string?> PickAudioAsync()
            => PickFileAsync(audioMime, audioFileExtensions);

        public Task<IEnumerable<string>?> PickMultipleAudioAsync()
            => PickFilesAsync(audioMime, audioFileExtensions);
    }
}
