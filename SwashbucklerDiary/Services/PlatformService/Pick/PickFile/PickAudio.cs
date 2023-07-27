namespace SwashbucklerDiary.Services
{
    public partial class PlatformService
    {
        public Task<string?> PickAudioAsync()
        {
            var suffixNames = new[] { ".mp3", ".wav", ".m4a", ".ogg", ".aac", ".flac" };
            var mimeTypes = new[] { "audio/mpeg", "audio/wav", "audio/mp4", "audio/ogg", "audio/aac", "audio/flac" };
            var utType = new[] { "public.mp3", "public.wav", "public.mpeg-4-audio", "public.ogg", "public.aac", "public.flac", };
            var customFileType = new FilePickerFileType(
                new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.iOS, utType }, // UTType values
                    { DevicePlatform.Android, mimeTypes }, // MIME type
                    { DevicePlatform.WinUI, suffixNames }, // file extension
                    { DevicePlatform.Tizen, new[] { "*/*" } },
                    { DevicePlatform.macOS, utType }, // UTType values
                });

            PickOptions options = new()
            {
                FileTypes = customFileType,
            };
            return PickFileAsync(options, suffixNames);
        }
    }
}
