namespace SwashbucklerDiary.Maui.Essentials
{
    public partial class PlatformIntegration
    {
        public async Task<string?> PickVideoAsync()
        {
            FileResult photo = await MediaPicker.Default.PickVideoAsync();
            return photo?.FullPath;
        }
    }
}
