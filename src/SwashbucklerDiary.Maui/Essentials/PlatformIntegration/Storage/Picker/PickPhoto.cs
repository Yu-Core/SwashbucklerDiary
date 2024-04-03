#if MACCATALYST
using MobileCoreServices;
#endif

namespace SwashbucklerDiary.Maui.Essentials
{
    public partial class PlatformIntegration
    {
        public async Task<string?> PickPhotoAsync()
        {
#if MACCATALYST
            string[] videoSuffixName = [".jpg", ".jpeg", ".png", ".gif", ".svg", ".webp"];
#pragma warning disable CA1422 // 验证平台兼容性
            string[] types = { UTType.JPEG, UTType.PNG, UTType.GIF, UTType.ScalableVectorGraphics, "webp" };
#pragma warning restore CA1422 // 验证平台兼容性
            return await PickFileAsync(types, videoSuffixName);
#else
            FileResult? fileResult = await MediaPicker.Default.PickPhotoAsync();
            return fileResult?.FullPath;
#endif
        }
    }
}
