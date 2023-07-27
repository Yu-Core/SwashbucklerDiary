using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.IServices
{
    public interface IPlatformService
    {
        event Action Resumed;
        event Action Stopped;
        void OnResume();
        void OnStop();
        Task SetClipboard(string text);
        Task ShareText(string title, string text);
        Task ShareFile(string title, string path);
        Task<string?> PickPhotoAsync();
        Task<string?> PickAudioAsync();
        Task<string?> PickVideoAsync();
        bool IsCaptureSupported();
        Task<string?> CapturePhotoAsync();
        bool IsMailSupported();
        Task SendEmail(List<string>? recipients);
        Task SendEmail(string? subject, string? body, List<string>? recipients);
        Task OpenBrowser(string? url);
        Task<bool> CheckCameraPermission();
        Task<bool> CheckStorageWritePermission();
        Task<bool> CheckStorageReadPermission();
        Task<bool> TryCameraPermission();
        Task<bool> TryStorageWritePermission();
        Task<bool> TryStorageReadPermission();
        string GetAppVersion();
        Task<bool> OpenStoreMyAppDetails();
        Task<string> ReadMarkdownFileAsync(string path);
        Task<T> ReadJsonFileAsync<T>(string path);
        bool IsFirstLaunch();
        Task<string?> PickFolderAsync();
        Task<string?> PickDBFileAsync();
        Task<string?> PickJsonFileAsync();
        Task<string?> SaveFileAsync(string name, Stream stream);
        Task<string?> SaveFileAsync(string? path,string name,Stream stream);
        void OpenPlatformSetting();
        void QuitApp();
        Task<bool> OpenQQGroup();
    }
}
