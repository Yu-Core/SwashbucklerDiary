namespace SwashbucklerDiary.Gtk.Essentials
{
    public partial class PlatformIntegration
    {

        static readonly string[] imageFileExtensions = [".jpg", ".jpeg", ".png", ".gif", ".svg", ".webp", ".jfif"];

        static readonly string[] imageTypes = ["image/*"];

        public Task<string?> PickPhotoAsync()
            => PickFileAsync(imageTypes, imageFileExtensions);

        public Task<IEnumerable<string>?> PickMultiplePhotoAsync()
            => PickMultipleFileAsync(imageTypes, imageFileExtensions);
    }
}
