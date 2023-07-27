namespace SwashbucklerDiary.Services
{
    public partial class PlatformService
    {
        public async Task<string?> PickVideoAsync()
        {
            FileResult photo = await MediaPicker.Default.PickVideoAsync();
            return photo?.FullPath;
        }
    }
}
