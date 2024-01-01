namespace SwashbucklerDiary.WebAssembly.Essentials
{
    public partial class PlatformIntegration
    {
        public Task<string?> PickPhotoAsync()
            => PickFileAsync("image/*");
    }
}
