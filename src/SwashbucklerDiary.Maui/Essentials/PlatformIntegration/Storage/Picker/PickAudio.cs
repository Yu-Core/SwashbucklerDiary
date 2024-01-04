namespace SwashbucklerDiary.Maui.Essentials
{
    public partial class PlatformIntegration
    {
        private readonly string[] audioSuffixNames = [".mp3", ".wav", ".m4a", ".ogg", ".aac", ".flac"];

        public Task<string?> PickAudioAsync()
        {
#if WINDOWS
            var types = suffixNames;
#elif ANDROID
            var types = new[] { "audio/mpeg", "audio/wav", "audio/mp4", "audio/ogg", "audio/aac", "audio/flac" };
#elif MACCATALYST || IOS
            var types = new[] { "public.mp3", "public.wav", "public.mpeg-4-audio", "public.ogg", "public.aac", "public.flac", };
#elif TIZEN
            var types = new[] { "*/*" };
#endif
            return PickFileAsync(types, audioSuffixNames);
        }
    }
}
