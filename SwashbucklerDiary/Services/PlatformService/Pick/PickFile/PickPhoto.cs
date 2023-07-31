namespace SwashbucklerDiary.Services
{
    public partial class PlatformService
    {
        public async Task<string?> PickPhotoAsync()
        {
            FileResult photo = await MediaPicker.Default.PickPhotoAsync();
            var path = photo?.FullPath;
#if WINDOWS
            FileInfo fileInfo = new FileInfo(path);
            long maxLength = 8 * 1024 * 1024;
            if(fileInfo.Length > maxLength)
            {
                return null;
            }
#endif
            return path;
        }
    }
}
