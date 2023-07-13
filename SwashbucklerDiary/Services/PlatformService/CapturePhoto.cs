namespace SwashbucklerDiary.Services
{
    public partial class PlatformService
    {
        public bool IsCaptureSupported()
        {
            return MediaPicker.Default.IsCaptureSupported;
        }

        public async Task<string?> CapturePhotoAsync()
        {
#if WINDOWS
            FileResult? photo = await WindowsMediaPicker.CapturePhotoAsync();
#else
            FileResult photo = await MediaPicker.Default.CapturePhotoAsync();
#endif
            return photo?.FullPath;
        }
    }
}
