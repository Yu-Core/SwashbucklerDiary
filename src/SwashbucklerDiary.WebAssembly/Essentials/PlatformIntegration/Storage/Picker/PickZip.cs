namespace SwashbucklerDiary.WebAssembly.Essentials
{
    public partial class PlatformIntegration
    {
        public Task<string?> PickZipFileAsync()
            => PickFileAsync(".zip");
    }
}
