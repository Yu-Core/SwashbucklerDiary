namespace SwashbucklerDiary.Maui.Essentials
{
    public partial class PlatformIntegration
    {
        public bool IsCaptureSupported()
        {
            return MediaPicker.Default.IsCaptureSupported;
        }

        public async Task<string?> CapturePhotoAsync()
        {
            FileResult photo = await MediaPicker.Default.CapturePhotoAsync();
            return photo?.FullPath;
        }
    }
}
