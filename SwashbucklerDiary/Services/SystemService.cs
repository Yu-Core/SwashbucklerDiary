using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Storage;
using MauiBlazorToolkit;
using MauiBlazorToolkit.Essentials;
using MauiBlazorToolkit.Platform;
using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Services
{
    public class SystemService : ISystemService
    {
        public event Action? Resumed;
        public event Action? Stopped;
        public void OnResume()
        {
            Resumed?.Invoke();
        }
        public void OnStop()
        {
            Stopped?.Invoke();
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

            if (File.Exists(target))
            {
                File.Delete(target);
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

        public async Task OpenBrowser(string? url)
        {
            if(string.IsNullOrEmpty(url))
            {
                return;
            }

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
            return await CheckPermission<Permissions.Camera>();
        }

        public async Task<bool> CheckStorageWritePermission()
        {
            return await CheckPermission<Permissions.StorageWrite>();
        }

        public async Task<bool> CheckStorageReadPermission()
        {
            return await CheckPermission<Permissions.StorageRead>();
        }

        private static async Task<bool> CheckPermission<T>() where T : Permissions.BasePermission, new()
        {
            PermissionStatus status = await Permissions.CheckStatusAsync<T>();

            if (status == PermissionStatus.Granted)
                return true;
            return false;
        }

        public async Task<bool> TryCameraPermission()
        {
            return await TryPermission<Permissions.Camera>();
        }

        public async Task<bool> TryStorageWritePermission()
        {
            return await TryPermission<Permissions.StorageWrite>();
        }

        public async Task<bool> TryStorageReadPermission()
        {
            return await TryPermission<Permissions.StorageRead>();
        }

        private static async Task<bool> TryPermission<T>() where T : Permissions.BasePermission, new()
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
        public Task<bool> OpenStoreMyAppDetails()
        {
#if WINDOWS
            var id = "9P6PBVBF466L";
            return OpenStoreAppDetails(id);
#elif ANDROID
            string uri = $"coolmarket://apk/{AppInfo.PackageName}";
            try 
	        {	        
		        return Browser.Default.OpenAsync(uri);
	        }
	        catch (Exception)
	        {
                uri = $"https://www.coolapk.com/apk/{AppInfo.PackageName}";
                return Browser.Default.OpenAsync(uri);
	        }
#else
            return OpenStoreAppDetails(AppInfo.PackageName);
#endif
        }

        public Task<bool> OpenStoreAppDetails(string appId)
        {
            return AppStoreLauncher.Default.TryOpenAsync(appId);
        }

        public async Task<string> ReadMarkdownFile(string path)
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

        private static Task<string?> PickFileAsync(PickOptions options, string suffixName)
        {
            string[] suffixNames = { suffixName };
            return PickFileAsync(options, suffixNames);
        }

        private async static Task<string?> PickFileAsync(PickOptions options, string[] suffixNames)
        {
            try
            {
                var result = await FilePicker.Default.PickAsync(options);
                if (result != null)
                {
                    bool flag = false;
                    foreach (var suffixName in suffixNames)
                    {
                        if (result.FileName.EndsWith(suffixName, StringComparison.OrdinalIgnoreCase))
                        {
                            flag = true;
                            break;
                        }
                    }

                    if (flag)
                    {
                        return result.FullPath;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return null;
        }

        public Task<string?> PickDBFileAsync()
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
            return PickFileAsync(options, "db3");
        }

        public Task<string?> PickJsonFileAsync()
        {
            var customFileType = new FilePickerFileType(
                new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.iOS, new[] { "public.json" } }, // UTType values
                    { DevicePlatform.Android, new[] { "application/json" } }, // MIME type
                    { DevicePlatform.WinUI, new[] { ".json" } }, // file extension
                    { DevicePlatform.Tizen, new[] { "*/*" } },
                    { DevicePlatform.macOS, new[] { "public.json" } }, // UTType values
                });

            PickOptions options = new()
            {
                FileTypes = customFileType,
            };
            return PickFileAsync(options, "json");
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

        private static readonly Color statusBarColorLight = Color.FromRgb(247, 248, 249);
        private static readonly Color statusBarColorDark = Color.FromRgb(18, 18, 18);
#pragma warning disable CA1416
        public void SetStatusBar(ThemeState themeState)
        {
            var Dark = themeState == ThemeState.Dark; 
            Color backgroundColor = Dark ? statusBarColorDark : statusBarColorLight;
#if WINDOWS || MACCATALYST
            TitleBar.SetColor(backgroundColor);
            TitleBarStyle titleBarStyle = Dark ? TitleBarStyle.LightContent : TitleBarStyle.DarkContent;
            TitleBar.SetStyle(titleBarStyle);
#elif ANDROID || IOS14_2_OR_GREATER
            CommunityToolkit.Maui.Core.Platform.StatusBar.SetColor(backgroundColor);
            StatusBarStyle statusBarStyle = Dark ? StatusBarStyle.LightContent : StatusBarStyle.DarkContent;
            CommunityToolkit.Maui.Core.Platform.StatusBar.SetStyle(statusBarStyle);
#endif
        }

        public void OpenSystemSetting()
        {
            AppInfo.Current.ShowSettingsUI();
        }

        public void QuitApp()
        {
            App.Current!.Quit();
        }

        public Task<bool> OpenQQGroup()
        {
            string uri = string.Empty;
#if ANDROID || IOS
            uri = "mqqopensdkapi://bizAgent/qm/qr?url=http%3A%2F%2Fqm.qq.com%2Fcgi-bin%2Fqm%2Fqr%3Ffrom%3Dapp%26p%3Dandroid%26jump_from%3Dwebapi%26k%3DJM3XMjO_Zw-ub2_p1xrk6qVj0_21PeO1";
            return Launcher.Default.TryOpenAsync(uri);
#else
            uri = "http://qm.qq.com/cgi-bin/qm/qr?_wv=1027&k=gmVF7NgQwU16rrjrTLW37nY9SfDAqKNI&authKey=0tgO1ht368hGRMR0UlEi21vZxKfZdu3h1GmmDyRh4o5qPkDVt0X3RSHoSCiPwkjl&noverify=0&group_code=139864402";
            return Browser.Default.OpenAsync(uri, BrowserLaunchMode.External);
#endif
        }

        public void ClearFolder(string folderPath)
        {
            DirectoryInfo directory = new DirectoryInfo(folderPath);

            // 删除文件夹中的所有文件
            foreach (FileInfo file in directory.GetFiles())
            {
                file.Delete();
            }

            // 删除文件夹中的所有子文件夹
            foreach (DirectoryInfo subDirectory in directory.GetDirectories())
            {
                ClearFolder(subDirectory.FullName);
                subDirectory.Delete();
            }
        }
    }
}
