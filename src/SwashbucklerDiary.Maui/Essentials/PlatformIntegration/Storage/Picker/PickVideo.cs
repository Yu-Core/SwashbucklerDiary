#if MACCATALYST
using MobileCoreServices;
#endif

namespace SwashbucklerDiary.Maui.Essentials
{
    public partial class PlatformIntegration
    {
        public async Task<string?> PickVideoAsync()
        {
#if MACCATALYST
            string[] videoSuffixName = [".mp4", ".m4v", ".mpg", ".mpeg", ".mp2", ".mov", ".avi", ".mkv", ".flv", ".gifv", ".qt"];
#pragma warning disable CA1422 // 验证平台兼容性
            string[] types = { UTType.MPEG4, UTType.Video, UTType.AVIMovie, UTType.AppleProtectedMPEG4Video, "mp4", "m4v", "mpg", "mpeg", "mp2", "mov", "avi", "mkv", "flv", "gifv", "qt" };
#pragma warning restore CA1422 // 验证平台兼容性
            return await PickFileAsync(types, videoSuffixName);
#else
            FileResult fileResult = await MediaPicker.Default.PickVideoAsync();
            return fileResult?.FullPath;
#endif
        }
    }
}
