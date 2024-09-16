namespace SwashbucklerDiary.WebAssembly.Essentials
{
    public partial class PlatformIntegration
    {
        static readonly string[] videoFileExtensions = [".mp4", ".m4v", ".mpg", ".mpeg", ".mp2", ".mov", ".avi", ".mkv", ".flv", ".gifv", ".qt"];

        static readonly string videoMime = "video/*";
        public Task<string?> PickVideoAsync()
            => PickFileAsync(videoMime, videoFileExtensions);

        public Task<IEnumerable<string>?> PickMultipleVideoAsync()
            => PickFilesAsync(videoMime, videoFileExtensions);
    }
}
