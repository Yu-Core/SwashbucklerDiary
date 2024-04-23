#if MACCATALYST
using MobileCoreServices;
#endif

namespace SwashbucklerDiary.Maui.Essentials
{
    public partial class PlatformIntegration
    {
        public async Task<string?> PickPhotoAsync()
        {
#if MACCATALYST || WINDOWS
            string[] suffixName = [".jpg", ".jpeg", ".png", ".gif", ".svg", ".webp", ".jfif"];
#if MACCATALYST
#pragma warning disable CA1422 // 验证平台兼容性
            string[] types = { UTType.JPEG, UTType.PNG, UTType.GIF, UTType.ScalableVectorGraphics, "webp", "jfif" };
#pragma warning restore CA1422 // 验证平台兼容性
#elif WINDOWS
            string[] types = suffixName;
#endif
            return await PickFileAsync(types, suffixName);
#else
            FileResult? fileResult = await MediaPicker.Default.PickPhotoAsync();
            return fileResult?.FullPath;
#endif
        }
    }
}
