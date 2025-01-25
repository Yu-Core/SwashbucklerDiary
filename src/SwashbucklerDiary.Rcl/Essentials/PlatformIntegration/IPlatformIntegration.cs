using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Essentials
{
    public interface IPlatformIntegration
    {
        AppDevicePlatform CurrentPlatform { get; }

        string AppVersionString { get; }

        /// <summary>
        /// 复制到粘贴板
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        Task SetClipboardAsync(string text);
        /// <summary>
        /// 分享文本
        /// </summary>
        /// <param name="title"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        Task ShareTextAsync(string title, string text);

        /// <summary>
        /// 分享文件
        /// </summary>
        /// <param name="title"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        Task ShareFileAsync(string title, string path);

        /// <summary>
        /// 选取图片文件
        /// </summary>
        /// <returns>文件路径</returns>
        Task<string?> PickPhotoAsync();

        Task<IEnumerable<string>?> PickMultiplePhotoAsync();

        /// <summary>
        /// 选取音频文件
        /// </summary>
        /// <returns>文件路径</returns>
        Task<string?> PickAudioAsync();

        Task<IEnumerable<string>?> PickMultipleAudioAsync();

        /// <summary>
        /// 选取视频文件
        /// </summary>
        /// <returns>文件路径</returns>
        Task<string?> PickVideoAsync();

        Task<IEnumerable<string>?> PickMultipleVideoAsync();
        /// <summary>
        /// 是否支持摄像头
        /// </summary>
        /// <returns></returns>
        ValueTask<bool> IsCaptureSupported();

        /// <summary>
        /// 拍照
        /// </summary>
        /// <returns>图片路径</returns>
        Task<string?> CapturePhotoAsync();

        /// <summary>
        /// 发送电子邮件
        /// </summary>
        /// <param name="subject">主题</param>
        /// <param name="body">正文</param>
        /// <param name="recipients">联系人</param>
        /// <returns></returns>
        Task<bool> SendEmail(string? subject, string? body, List<string>? recipients);

        /// <summary>
        /// 打开浏览器
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        Task<bool> OpenBrowser(string? url);

        /// <summary>
        /// 检查摄像头权限
        /// </summary>
        /// <returns></returns>
        Task<bool> CheckCameraPermission();

        /// <summary>
        /// 检查外部存储写入权限
        /// </summary>
        /// <returns></returns>
        Task<bool> CheckStorageWritePermission();

        /// <summary>
        /// 检查摄像头权限,如未开启，尝试请求
        /// </summary>
        /// <returns></returns>
        Task<bool> TryCameraPermission();

        /// <summary>
        /// 检查外部存储写入权限,如未开启，尝试请求
        /// </summary>
        /// <returns></returns>
        Task<bool> TryStorageWritePermission();

        /// <summary>
        /// 选取Zip文件
        /// </summary>
        /// <returns></returns>
        Task<string?> PickZipFileAsync();

        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="sourceFilePath"></param>
        /// <returns></returns>
        Task<bool> SaveFileAsync(string sourceFilePath);

        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="name"></param>
        /// <param name="sourceFilePath"></param>
        /// <returns></returns>
        Task<bool> SaveFileAsync(string name, string sourceFilePath);

        /// <summary>
        /// 打开平台设置
        /// </summary>
        Task ShowSettingsUI();
    }
}
