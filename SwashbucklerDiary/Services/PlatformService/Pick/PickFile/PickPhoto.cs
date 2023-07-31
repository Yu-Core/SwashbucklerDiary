namespace SwashbucklerDiary.Services
{
    public partial class PlatformService
    {
        public async Task<string?> PickPhotoAsync()
        {
            FileResult photo = await MediaPicker.Default.PickPhotoAsync();
            var path = photo?.FullPath;
            return path;
        }
    }
}
