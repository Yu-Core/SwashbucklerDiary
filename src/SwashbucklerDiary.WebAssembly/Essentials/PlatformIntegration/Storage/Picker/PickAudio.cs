namespace SwashbucklerDiary.WebAssembly.Essentials
{
    public partial class PlatformIntegration
    {
        public Task<string?> PickAudioAsync()
            => PickFileAsync(".mp3,.wav,.m4a,.ogg,.aac,.flac");
    }
}
