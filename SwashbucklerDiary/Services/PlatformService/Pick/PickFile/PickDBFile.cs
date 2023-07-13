namespace SwashbucklerDiary.Services
{
    public partial class PlatformService
    {
        public Task<string?> PickDBFileAsync()
        {
            var customFileType = new FilePickerFileType(
                new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.iOS, new[] { "public.database" } }, // UTType values
                    { DevicePlatform.Android, new[] { "application/x-sqlite3", "application/vnd.sqlite3", "application/octet-stream" } }, // MIME type
                    { DevicePlatform.WinUI, new[] { ".db3" } }, // file extension
                    { DevicePlatform.Tizen, new[] { "*/*" } },
                    { DevicePlatform.macOS, new[] { "public.database" } }, // UTType values
                });

            PickOptions options = new()
            {
                FileTypes = customFileType,
            };
            return PickFileAsync(options, "db3");
        }
    }
}
