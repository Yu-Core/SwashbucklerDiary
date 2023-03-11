using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Storage;
using SwashbucklerDiary.IServices;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SwashbucklerDiary.Services
{
    public class SystemService : ISystemService
    {
        public async Task<string?> CapturePhotoAsync()
        {
#if WINDOWS
            FileResult? photo = await WindowsMediaPicker.CapturePhotoAsync();
#else
            FileResult photo = await MediaPicker.Default.CapturePhotoAsync();
#endif
            return photo?.FullPath;
        }

        public bool IsCaptureSupported()
        {
            return MediaPicker.Default.IsCaptureSupported;
        }

        public bool IsMailSupported()
        {
            return Email.Default.IsComposeSupported;
        }

        public async Task FileCopy(string source, string target)
        {
            if (!File.Exists(source) || string.IsNullOrEmpty(target))
            {
                return;
            }
            var file = new FileResult(source);
#if WINDOWS
            // on Windows file.OpenReadAsync() throws an exception
            using Stream sourceStream = File.OpenRead(source);
#else
            using Stream sourceStream = await file.OpenReadAsync();
#endif
            using (FileStream localFileStream = File.OpenWrite(target))
            {
                await sourceStream.CopyToAsync(localFileStream);
            };
        }

        public async Task OpenBrowser(string url)
        {
            await Browser.Default.OpenAsync(url, BrowserLaunchMode.External);
        }

        public async Task<string?> PickPhotoAsync()
        {
            FileResult photo = await MediaPicker.Default.PickPhotoAsync();
            return photo?.FullPath;
        }

        public async Task SendEmail(string? subject, string? body, List<string>? recipients)
        {
            var message = new EmailMessage
            {
                Subject = subject,
                Body = body,
                To = recipients == null ? null : new List<string>(recipients)
            };
            await Email.Default.ComposeAsync(message);
        }

        public Task SendEmail(List<string>? recipients)
        {
            //There are some problems in Windows. https://github.com/microsoft/microsoft-ui-xaml/issues/7300
#if WINDOWS
            string email = "mailto:" + recipients!.First();
            for (int i = 1; i < recipients!.Count; i++)
            {
                email += ";" + recipients[i];
            }
            return Launcher.Default.OpenAsync(email);
#else
            return SendEmail(null, null, recipients);
#endif
        }

        public async Task SetClipboard(string text)
        {
            await Clipboard.Default.SetTextAsync(text);
        }

        public async Task ShareFile(string title, string path)
        {
            await Share.Default.RequestAsync(new ShareFileRequest
            {
                Title = title,
                File = new ShareFile(path)
            });
        }

        public async Task ShareText(string title, string text)
        {
            await Share.Default.RequestAsync(new ShareTextRequest
            {
                Title = title,
                Text = text
            });
        }

        public async Task<bool> CheckCameraPermission()
        {
            return await SystemService.CheckPermission<Permissions.Camera>();
        }

        public async Task<bool> CheckStorageWritePermission()
        {
            return await SystemService.CheckPermission<Permissions.StorageWrite>();
        }

        public async Task<bool> CheckStorageReadPermission()
        {
            return await SystemService.CheckPermission<Permissions.StorageRead>();
        }

        private static async Task<bool> CheckPermission<T>() where T : Permissions.BasePermission, new()
        {
            PermissionStatus status = await Permissions.CheckStatusAsync<T>();

            if (status == PermissionStatus.Granted)
                return true;

            if (status == PermissionStatus.Denied)
            {
                if (DeviceInfo.Platform == DevicePlatform.iOS ||
                    DeviceInfo.Platform == DevicePlatform.macOS ||
                    DeviceInfo.Platform == DevicePlatform.MacCatalyst)
                    return false;
            }

            status = await Permissions.RequestAsync<T>();

            if (status == PermissionStatus.Granted)
                return true;

            if (status == PermissionStatus.Denied)
            {
                return false;
            }

            return true;
        }

        public string GetAppVersion()
        {
            return VersionTracking.Default.CurrentVersion.ToString();
        }

        //打开本应用的应用商店详情页
        public Task<bool> OpenStoreAppDetails()
        {
            return OpenStoreAppDetails(AppInfo.PackageName);
        }

        public async Task<bool> OpenStoreAppDetails(string appId)
        {
            string uri = string.Empty;
#if WINDOWS
            uri = $"ms-windows-store://pdp/?ProductId={appId}";
#elif ANDROID
            uri = $"market://details?id={appId}";
#elif IOS || MACCATALYST
            uri = $"itms-apps://itunes.apple.com/app/id{appId}?action=write-review";
#endif
            return await Launcher.Default.TryOpenAsync(uri);
        }

        public async Task<string> ReadMarkdown(string path)
        {
            bool exist = await FileSystem.AppPackageFileExistsAsync(path);
            if (exist)
            {
                using var stream = await FileSystem.OpenAppPackageFileAsync(path);
                using var sr = new StreamReader(stream);
                var content = sr.ReadToEnd();
                return content;
            }
            else
            {
                using var httpClient = new HttpClient();
                await using var stream = await httpClient.GetStreamAsync(path);
                using StreamReader sr = new(stream);
                var content = sr.ReadToEnd();
                return content;
            }
        }

        public bool IsFirstLaunch()
        {
            return VersionTracking.Default.IsFirstLaunchEver;
        }

        public async Task<string?> PickFolderAsync()
        {
            try
            {
                var folder = await FolderPicker.Default.PickAsync(default);
                if (folder.IsSuccessful)
                {
                    return folder.Folder.Path;
                }
                else
                { 
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<string?> PickFileAsync(PickOptions options)
        {
            try
            {
                var result = await FilePicker.Default.PickAsync(options);
                if (result != null)
                {
                    return result.FullPath;
                }
            }
            catch (Exception)
            {

                throw;
            }

            return null;
        }

        public async Task<string?> PickDBFileAsync()
        {
            var customFileType = new FilePickerFileType(
                new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.iOS, new[] { "public.database" } }, // UTType values
                    { DevicePlatform.Android, new[] { "application/x-sqlite3", "application/vnd.sqlite3", "application/octet-stream" } }, // MIME type
                    { DevicePlatform.WinUI, new[] { ".db3" } }, // file extension
                    { DevicePlatform.Tizen, new[] { "*/*" } },
                    { DevicePlatform.macOS, new[] { "public.database" } }, // UTType values
                });

            PickOptions options = new()
            {
                FileTypes = customFileType,
            };
            try
            {
                var result = await FilePicker.Default.PickAsync(options);
                if (result != null)
                {
                    return result.FullPath;
                }
            }
            catch (Exception)
            {

                throw;
            }

            return null;
        }

        public Task<string?> SaveFileAsync(string name, Stream stream)
        {
            return SaveFileAsync(string.Empty, name, stream);
        }
        public async Task<string?> SaveFileAsync(string? path, string name, Stream stream)
        {
            try
            {
                FileSaverResult? fileSaverResult;
                if (string.IsNullOrEmpty(path))
                {
                    fileSaverResult = await FileSaver.Default.SaveAsync(name, stream, default);
                }
                else
                {
                    fileSaverResult = await FileSaver.Default.SaveAsync(path, name, stream, default);
                }
                
                if (fileSaverResult.IsSuccessful)
                {
                    return fileSaverResult.FilePath;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
