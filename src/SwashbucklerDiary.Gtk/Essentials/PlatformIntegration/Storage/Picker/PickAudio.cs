namespace SwashbucklerDiary.Gtk.Essentials
{
    public partial class PlatformIntegration
    {
        static readonly string[] audioFileExtensions = [".mp3", ".wav", ".m4a", ".ogg", ".aac", ".flac"];

        static readonly string[] audioTypes = ["audio/mpeg", "audio/wav", "audio/mp4", "audio/ogg", "audio/aac", "audio/flac"];

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
