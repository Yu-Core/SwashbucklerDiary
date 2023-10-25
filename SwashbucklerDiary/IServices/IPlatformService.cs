using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.IServices
{
    public interface IPlatformService
    {
        Color LightColor { get; set; }

        Color DarkColor { get; set; }

        event Action Resumed;

        event Action Stopped;

        void OnResume();

        void OnStop();

        /// <summary>
        /// 复制到粘贴板
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        Task SetClipboard(string text);

        /// <summary>
        /// 分享文本
        /// </summary>
        /// <param name="title"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        Task ShareText(string title, string text);

        /// <summary>
        /// 分享文件
        /// </summary>
        /// <param name="title"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        Task ShareFile(string title, string path);

        /// <summary>
        /// 选取图片文件
        /// </summary>
        /// <returns>文件路径</returns>
        Task<string?> PickPhotoAsync();

        /// <summary>
        /// 选取音频文件
        /// </summary>
        /// <returns>文件路径</returns>
        Task<string?> PickAudioAsync();

        /// <summary>
        /// 选取视频文件
        /// </summary>
        /// <returns>文件路径</returns>
        Task<string?> PickVideoAsync();

        /// <summary>
        /// 是否支持摄像头
        /// </summary>
        /// <returns></returns>
        bool IsCaptureSupported();

        /// <summary>
        /// 拍照
        /// </summary>
        /// <returns>图片路径</returns>
        Task<string?> CapturePhotoAsync();

        /// <summary>
        /// 是否支持电子邮件
        /// </summary>
        /// <returns></returns>
        bool IsMailSupported();

        /// <summary>
        /// 发送电子邮件
        /// </summary>
        /// <param name="recipients">联系人</param>
        /// <returns></returns>
        Task SendEmail(List<string>? recipients);

        /// <summary>
        /// 发送电子邮件
        /// </summary>
        /// <param name="subject">主题</param>
        /// <param name="body">正文</param>
        /// <param name="recipients">联系人</param>
        /// <returns></returns>
        Task SendEmail(string? subject, string? body, List<string>? recipients);

        /// <summary>
        /// 打开浏览器
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        Task OpenBrowser(string? url);

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
       /// 获得App版本
       /// </summary>
       /// <returns></returns>
        string GetAppVersion();

        /// <summary>
        /// 打开我的App应用商店详情页
        /// </summary>
        /// <returns></returns>
        Task<bool> OpenStoreMyAppDetails();

        /// <summary>
        /// 读取Markdown文件
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns></returns>
        Task<string> ReadMarkdownFileAsync(string path);

        /// <summary>
        /// 读取Json文件
        /// </summary>
        /// <typeparam name="T">需要转化成的对象类型</typeparam>
        /// <param name="path">文件路径</param>
        /// <returns></returns>
        Task<T> ReadJsonFileAsync<T>(string path);

        /// <summary>
        /// 是否是第一次启动
        /// </summary>
        /// <returns></returns>
        bool IsFirstLaunch();

        /// <summary>
        /// 选取Zip文件
        /// </summary>
        /// <returns></returns>
        Task<string?> PickZipFileAsync();

        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="name"></param>
        /// <param name="sourceFilePath"></param>
        /// <returns></returns>
        Task<string?> SaveFileAsync(string name, string sourceFilePath);

        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="name"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        Task<string?> SaveFileAsync(string name, Stream stream);

        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="name"></param>
        /// <param name="sourceFilePath"></param>
        /// <returns></returns>
        Task<string?> SaveFileAsync(string? path, string name, string sourceFilePath);

        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="name"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        Task<string?> SaveFileAsync(string? path, string name, Stream stream);

        /// <summary>
        /// 打开平台设置
        /// </summary>
        void OpenPlatformSetting();

        /// <summary>
        /// 退出应用
        /// </summary>
        void QuitApp();

        /// <summary>
        /// 打开我的QQ群
        /// </summary>
        /// <returns></returns>
        Task<bool> OpenQQGroup();

        /// <summary>
        /// 设置标题栏或状态栏颜色
        /// </summary>
        /// <param name="themeState"></param>
        void SetTitleBarOrStatusBar(ThemeState themeState);

        /// <summary>
        /// 设置标题栏或状态栏颜色
        /// </summary>
        /// <param name="themeState"></param>
        /// <param name="backgroundColor"></param>
        void SetTitleBarOrStatusBar(ThemeState themeState, Color backgroundColor);

        DevicePlatformType GetDevicePlatformType();

        string GetDevicePlatformTypeIcon(DevicePlatformType platformType);
    }
}
