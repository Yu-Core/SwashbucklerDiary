using BlazorComponent.I18n;
using NoDecentDiary.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoDecentDiary.Services
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
            return SendEmail(null, null, recipients);
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

        private async Task<bool> CheckPermission<T>() where T : Permissions.BasePermission, new()
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


    }
}
