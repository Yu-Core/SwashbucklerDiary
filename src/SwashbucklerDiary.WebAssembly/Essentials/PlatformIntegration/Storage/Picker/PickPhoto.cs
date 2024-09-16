namespace SwashbucklerDiary.WebAssembly.Essentials
{
    public partial class PlatformIntegration
    {
        static readonly string[] imageFileExtensions = [".jpg", ".jpeg", ".png", ".gif", ".svg", ".webp", ".jfif"];

        static readonly string imageMime = "image/*";

        public Task<string?> PickPhotoAsync()
            => PickFileAsync(imageMime, imageFileExtensions);

        public Task<IEnumerable<string>?> PickMultiplePhotoAsync()
            => PickFilesAsync(imageMime, imageFileExtensions);
    }
}
