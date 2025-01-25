namespace SwashbucklerDiary.Gtk.Essentials
{
    public partial class PlatformIntegration
    {

        static readonly string[] videoFileExtensions = [".mp4", ".m4v", ".mpg", ".mpeg", ".mp2", ".mov", ".avi", ".mkv", ".flv", ".gifv", ".qt"];

        static readonly string[] videoTypes = ["video/*"];

        public Task<string?> PickVideoAsync()
            => PickFileAsync(videoTypes, videoFileExtensions);

        public Task<IEnumerable<string>?> PickMultipleVideoAsync()
            => PickMultipleFileAsync(videoTypes, videoFileExtensions);
    }
}
