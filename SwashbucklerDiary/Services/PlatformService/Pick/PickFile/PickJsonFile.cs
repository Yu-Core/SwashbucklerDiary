namespace SwashbucklerDiary.Services
{
    public partial class PlatformService
    {
        public Task<string?> PickJsonFileAsync()
        {
            var customFileType = new FilePickerFileType(
                new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.iOS, new[] { "public.json" } }, // UTType values
                    { DevicePlatform.Android, new[] { "application/json" } }, // MIME type
                    { DevicePlatform.WinUI, new[] { ".json" } }, // file extension
                    { DevicePlatform.Tizen, new[] { "*/*" } },
                    { DevicePlatform.macOS, new[] { "public.json" } }, // UTType values
                });

            PickOptions options = new()
            {
                FileTypes = customFileType,
            };
            return PickFileAsync(options, "json");
        }
    }
}
